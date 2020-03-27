using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BookShop2020.Models;

namespace BookShop2020.Controllers
{
    [Authorize(Roles = "manager")]
    public class BookController : Controller
    {
        private BookContext db = new BookContext();

        // GET: /Book/
        public ActionResult Index(int? categoryId)
        {
            List<Book> bookList = db.Books.ToList();
            if (categoryId != null && categoryId != 0)
            {
                var category = db.Categories.Find(categoryId);
                if (category != null)
                    bookList = category.Books.ToList();
            }

            var categoriesList = db.Categories.ToList();
            categoriesList.Insert(0, new Category() { Id = 0, Name = "все" });
            return View(bookList);
        }

        // GET: /Book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            var genre = db.Categories.Where(g => g.Id == book.CategoryId).ToList();
            book.Genre = genre[0];
            return View(book);
        }

        // GET: /Book/Create
        public ActionResult Create()
        {
            var categoriesList = db.Categories.ToList();
            SelectList categories = new SelectList(categoriesList, "Id", "Name");
            ViewBag.list = categories;
            return View();
        }

        // POST: /Book/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Author,Price,Year,CategoryId")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: /Book/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            var categoriesList = db.Categories.ToList();
            SelectList categories = new SelectList(categoriesList, "Id", "Name");
            ViewBag.list = categories;
            var genre = db.Categories.Where(g => g.Id == book.CategoryId).ToList();
            book.Genre = genre[0];
            return View(book);
        }

        // POST: /Book/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Author,Price,Year,CategoryId")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: /Book/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            var genre = db.Categories.Where(g => g.Id == book.CategoryId).ToList();
            book.Genre = genre[0];
            return View(book);
        }

        // POST: /Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var book = db.Books.Find(id);
            if (book != null) db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}