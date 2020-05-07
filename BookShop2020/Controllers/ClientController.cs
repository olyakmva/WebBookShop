using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using BookShop2020.Models;
using Microsoft.AspNet.Identity;

namespace BookShop2020.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        BookContext db = new BookContext();
        // GET: Client
        public ActionResult Index(string id)
        {
            if (id == null)
                return HttpNotFound();
            var client = db.Clients.Find(id);

            return View(client);
        }

        public ActionResult Comments( string id)
        {
            var orders = db.Orders.Where(d => d.ClientId == id)
                .OrderByDescending(d => d.Date).ToList();
            var books = new List<Book>();
            foreach (var order in orders)
            {
                var items = db.Items.Where(i => i.OrderId == order.Id)
                    .Include(b=>b.TheBook).ToList();
                books.AddRange(items.Select(item => item.TheBook));
            }
            var list = books.Distinct().ToList();
            return View(list);
        }

        public ActionResult LeaveReview(int bookId)
        {
            Book book = db.Books.Find(bookId);
            if (book == null)
            {
                return HttpNotFound();
            }
            book.Genre = db.Categories.Find(book.CategoryId);
            ViewBag.Book = book;
            var review = new Review {BookId = book.Id};
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LeaveReview([Bind(Include = "Id,BookId,Text")] Review review)
        {
            if (!ModelState.IsValid) return View();
            var clientId = User.Identity.GetUserId();
            review.ClientId = clientId;
            var client = db.Clients.Find(clientId);
            if (client != null) client.ReviewsNumber++;
            db.Reviews.Add(review);
            db.Entry(client).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Comments", "Client",new{id= clientId});
        }

    }
}