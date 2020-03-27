using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BookShop2020.Models;

namespace BookShop2020.Controllers
{
    [Authorize(Roles = "manager")]
    public class ManagerController : Controller
    {
        BookContext db = new BookContext();
        // GET: Manager
        public ActionResult BestsellerList()
        {
            var orders = db.Orders.ToList();
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
            var result = (from t in booksDictionary orderby t.Value descending select t).Take(5);

            List<Bestseller> list = new List<Bestseller>();

            foreach (var item in result)
            {
                var book = db.Books.Find(item.Key);
                if (book == null) continue;
                var zanr = db.Categories.Find(book.CategoryId);
                book.Genre = zanr;
                var bestseller = new Bestseller() { TheBook = book, Quantity = item.Value };
                list.Add(bestseller);
            }

            return View(list);
        }

        public ActionResult OrdersList(DateTime? start, DateTime? end)
        {
            var orders = db.Orders.ToList();
            if (start == null || end == null)
            {
                return View(orders);
            }
            List<Order> list = (from t in orders where (t.Date > start && t.Date <= end) select t).ToList();
            return View(list);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var items = db.Items.Where(x => x.OrderId == order.Id).ToList();
            foreach (var item in items)
            {
                var book = db.Books.Find(item.BookId);
                item.TheBook = book;
            }

            order.Items = items;

            return View(order);
        }
    }
}