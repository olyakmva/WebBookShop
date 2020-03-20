using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BookShop2020.Models;

namespace BookShop2020.Controllers
{
    public class OrderController : Controller
    {
        private BookContext db = new BookContext();

        string GetCooki(string key)
        {
            if (HttpContext.Request.Cookies.AllKeys.Length > 0 &&
                HttpContext.Request.Cookies.AllKeys.Contains(key))
            {
                return HttpContext.Request.Cookies[key].Value;
            }
            return null;
        }

        // GET: /Order/Create
        public ActionResult Create()
        {
            string cartId = GetCooki("CartId");
            if (cartId==null)
            {
                return RedirectToAction("Index","Home");
            }
            var items = db.ShoppingCarts.Where(c => c.CartId.CompareTo(cartId) == 0).ToList();
            Order newOrder = new Order();
            List<Item> goods = new List<Item>();
            decimal price = 0;
            foreach (var i in items)
            {
                var b = db.Books.Find(i.BookId);
                if (b == null) continue;
                var tm = new Item() {BookId = i.BookId, Quantity = i.Quantity, TheBook = b};
                price += b.Price * i.Quantity;
                goods.Add(tm);
            }

            newOrder.TotalPrice = price;
            newOrder.Items = goods;
            newOrder.Date = DateTime.Now;
            newOrder.Status = "не подтвержден";
            newOrder.DeliveryMethod = "Самовывоз";
            newOrder.LastName = "Введите фамилию";
            newOrder.Name = "Введите имя";
            db.Orders.Add(newOrder);
            db.SaveChanges();
            foreach (var item in goods)
            {
                item.OrderId = newOrder.Id;
                db.Items.Add(item);
                db.SaveChanges();
            }
            return View(newOrder);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,LastName,TotalPrice,Status,DeliveryMethod,Date,Address")]Order order)
        {
            if (ModelState.IsValid)
            {
                order.Status = "подтвержден";
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                string cartId = GetCooki("CartId");
                if (cartId == null) return RedirectToAction("Index", "Home");
                var carts = db.ShoppingCarts.Where(x => x.CartId == cartId);
                db.ShoppingCarts.RemoveRange(carts);
                db.SaveChanges();
                HttpContext.Request.Cookies.Remove("CartId");
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
      
        // GET: /Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var items = db.Items.Where(x => x.OrderId == order.Id);
            db.Items.RemoveRange(items);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
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
