using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Security;
using BookShop2020.Models;

namespace BookShop2020.Controllers
{
    public class HomeController : Controller
    {
        BookContext db = new BookContext();

        public ActionResult Index(int? categoryId) 
        {
            var categoriesList = db.Categories.ToList();
            categoriesList.Insert(0, new Category() { Id = 0, Name = "все" });
            SelectList categories = new SelectList(categoriesList, "Id", "Name");
            ViewBag.Genres =categories;

            var books = db.Books.Include(b=> b.Genre).OrderBy(b => b.Author);
            if (!(categoryId == null || categoryId ==0))
            {
               books = books.Where(b => b.CategoryId == categoryId).OrderBy(b=>b.Author); 
            }

            var list = books.ToList();
            var bookViewList = new List<BookViewModel>();
            foreach (var book in list)
            {
                var item = new BookViewModel {TheBook = book};
                var reviews  = db.Reviews.Count(x => x.BookId == book.Id);
                item.ReviewNumber = reviews;
                bookViewList.Add(item);
            }
            return View(bookViewList.ToList());
        }
        [HttpPost]
        public ActionResult Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
                return RedirectToAction("Index");
            var result = db.Books.Include(b => b.Genre).Where(b=>b.Name.Contains(searchString)).ToList();
            result.AddRange(db.Books.Include(b => b.Genre).Where(b => b.Author.Contains(searchString)).ToList());
            result.AddRange(db.Books.Include(b => b.Genre).Where(b => b.Genre.Name.Contains(searchString)).ToList());

            if (result.Count > 0)
            {
                ViewBag.Message = new[]{ "Результаты поиска"};
            }
            else
            {
                ViewBag.Message = new[] { "Мы ничего не нашли по вашему запросу! Что делать?",
                    "Посмотрите на книги, которые пользуются спросом у наших покупателей"};
                var orders = db.Orders.Take(20).ToList();
                var booksDictionary = new Dictionary<int, int>();
                foreach (var order in orders)
                {
                    var books = db.Items.Where(i => i.OrderId == order.Id).ToList();
                    foreach (var book in books)
                    {
                        if (booksDictionary.ContainsKey(book.BookId))
                        {
                            booksDictionary[book.BookId] += book.Quantity;
                        }
                        else booksDictionary.Add(book.BookId, book.Quantity);
                    }
                }
                var idList = (from t in booksDictionary orderby t.Value descending select t).Take(6);

                foreach (var item in idList)
                {
                    var book = db.Books.Find(item.Key);
                    if (book == null) continue;
                    var zanr = db.Categories.Find(book.CategoryId);
                    book.Genre = zanr;
                    result.Add(book);
                }
            }
            return View(result);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ViewBookReviews(int bookId)
        {
            Book book = db.Books.Find(bookId);
            if (book == null)
            {
                return HttpNotFound();
            }
            book.Genre = db.Categories.Find(book.CategoryId);
            ViewBag.Book = book;
            var reviews = db.Reviews.Where(x => x.BookId == bookId).ToList();
            var list = new List<ReviewViewModel>();
            foreach (var item in reviews)
            {
                var note = new ReviewViewModel {Text = item.Text};
                var client = db.Clients.Find(item.ClientId);
                if (client != null)
                {
                    note.Name = client.Name;
                    note.LastName = client.LastName;
                }
                list.Add(note);
            }

            return View(list);
        }
    }
}