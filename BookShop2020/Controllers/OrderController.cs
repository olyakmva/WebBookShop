using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BookShop2020.Models;
using Microsoft.AspNet.Identity;

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
                var book = db.Books.Find(i.BookId);
                if (book == null) continue;
                var tm = new Item() {BookId = i.BookId, Quantity = i.Quantity, TheBook = book};
                if (book.Number < i.Quantity)
                {
                    i.Quantity = book.Number;
                }

                book.Number -= i.Quantity;
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                price += book.Price * i.Quantity;
                goods.Add(tm);
            }
            newOrder.TotalPrice = price;
            newOrder.Items = goods;
            newOrder.Date = DateTime.Now;
            newOrder.Status = "не подтвержден";
            newOrder.DeliveryMethod = "Самовывоз";
            newOrder.LastName = "Введите фамилию";
            newOrder.Name = "Введите имя";
            ViewBag.Discount = 0;
            if (User.Identity.IsAuthenticated)
            {
                var id = User.Identity.GetUserId();
                var client = db.Clients.Find(id);
                if (client != null)
                {
                    newOrder.ClientId = id;
                    if (client.LastName != null)
                        newOrder.LastName = client.LastName;
                    if (client.Name != null)
                        newOrder.Name = client.Name;
                    if (client.Address != null)
                        newOrder.Address = client.Address;
                    if (client.CurrentDiscount > 0)
                    {
                        var discount = (client.CurrentDiscount * newOrder.TotalPrice) / 100;
                        ViewBag.Discount = discount;
                        ViewBag.Cost = newOrder.TotalPrice;
                        newOrder.TotalPrice -= discount;
                    }
                }
            }
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
        public ActionResult Create([Bind(Include = "Id,ClientId,Name,LastName,TotalPrice,Status,DeliveryMethod,Date,Address")]Order order)
        {
            if (!ModelState.IsValid) return View();
            order.Status = "подтвержден";
            if (User.Identity.IsAuthenticated)
            {
                var id = User.Identity.GetUserId();
                order.ClientId = id;
            }
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();
            RemoveCartRecords();
            ChangeDataForAuthUsers(order);
            return RedirectToAction("Index", "Home");
        }

        private void ChangeDataForAuthUsers(Order order)
        {
            if (!User.Identity.IsAuthenticated) return;
            var id = User.Identity.GetUserId();
            var client = db.Clients.Find(id);
            bool isNew = false;
            if (client == null)
            {
                client = new Client
                {
                    Id = id,
                    OrdersNumber = 0,
                    CurrentDiscount = 0,
                    TotalOrdersCost = 0,
                    ReviewsNumber = 0
                };
                isNew = true;
            }
            client.Address = order.Address;
            client.Name = order.Name;
            client.LastName = order.LastName;
            client.OrdersNumber++;
            client.TotalOrdersCost += order.TotalPrice;
            DiscountCalculator(client);
            if (isNew)
            {
                db.Clients.Add(client);
            }
            else
            {
                db.Entry(client).State = EntityState.Modified;
            }

            db.SaveChanges();
        }

        private static void DiscountCalculator(Client client)
        {
            if (client.TotalOrdersCost > 100000)
            {
                client.CurrentDiscount = 30;
                return;
            }

            if (client.TotalOrdersCost > 50000)
            {
                client.CurrentDiscount = 25;
                return;
            }
            if (client.TotalOrdersCost > 25000 || client.OrdersNumber > 25)
            {
                client.CurrentDiscount = 15;
                return;
            }
            if (client.TotalOrdersCost > 10000 || client.OrdersNumber > 10)
            {
                client.CurrentDiscount = 10;
                return;
            }

            client.CurrentDiscount = 5;
        }

        private void RemoveCartRecords()
        {
            string cartId = GetCooki("CartId");
            if (cartId == null) return ;
            var carts = db.ShoppingCarts.Where(x => x.CartId == cartId);
            db.ShoppingCarts.RemoveRange(carts);
            db.SaveChanges();
            HttpContext.Request.Cookies.Remove("CartId");
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
            var list = items.ToList();
            foreach (var item in list)
            {
                var book = db.Books.Find(item.BookId);
                if (book != null)
                {
                    book.Number += item.Quantity;
                    db.Entry(book).State = EntityState.Modified;
                }

                db.SaveChanges();
            }

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
