using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using BookShop2020.Models;

namespace BookShop2020.Controllers
{
    public class CartController : Controller
    {
        BookContext db = new BookContext();

        string GetCooki(string key)
        {
            if(HttpContext.Request.Cookies.AllKeys.Length > 0 &&
                HttpContext.Request.Cookies.AllKeys.Contains(key))
            {
                return HttpContext.Request.Cookies[key].Value;
            }
            return null;
        }
        public ActionResult Add(int bookId)
        {
            string cart = GetCooki("CartId");
            if(cart == null)
            {
                cart = Guid.NewGuid().ToString();
                HttpCookie cookie = new HttpCookie("CartId", cart);
                HttpContext.Response.Cookies.Add(cookie);
            }
            var item = db.ShoppingCarts.Where(c => c.CartId.CompareTo(cart) == 0
            && c.BookId.CompareTo(bookId) == 0);
            if(item.Any())
            {
                CartItem cartItem = item.First();
                cartItem.Quantity++;
                db.Entry(cartItem).State = EntityState.Modified;
            }
            else
            {
                var cartItem = new CartItem()
                {
                    CartId = cart,
                    BookId = bookId,
                    Quantity = 1
                };
                db.ShoppingCarts.Add(cartItem);
            }
            db.SaveChanges();
            return RedirectToAction("Index","Home");
        }
        // GET: Cart
        public ActionResult Index()
        {
            string cart = GetCooki("CartId");
            List<CartItem> items=new List<CartItem>();
            if (cart != null)
            {
                items = db.ShoppingCarts.Where(c => c.CartId.CompareTo(cart) == 0).ToList();
                decimal sum = 0;
                foreach (var item in items)
                {
                    var book = db.Books.Where(b => b.Id == item.BookId).First();
                    item.SelectBook = book;
                    sum += book.Price * item.Quantity;
                }
                ViewBag.Sum = sum;
                if(items.Count == 0)
                    ViewBag.Msg = "Ваша корзина пуста. Надо туда что-то положить :)";
                else ViewBag.Msg = "Ваши книги";
            }
            else
            {
                ViewBag.Msg = "Ваша корзина пуста. Надо туда что-то положить :)";
            }
            return View(items);
        }
       
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id != null)
            {
                var cartItem = db.ShoppingCarts.Find(id);
                cartItem.SelectBook = db.Books.Find(cartItem.BookId);
                return View(cartItem);
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var cart = db.ShoppingCarts.Find(id);
            db.ShoppingCarts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ChangeItemQuantity(int cartId, int newQuantity)
        {
            var cartItem = db.ShoppingCarts.Find(cartId);
            var book = db.Books.Find(cartItem.BookId);
            var delta = (newQuantity - cartItem.Quantity) * book.Price;
            cartItem.Quantity = newQuantity;
            db.Entry(cartItem).State = EntityState.Modified;
            db.SaveChanges();

            return Json(delta);
        }
    }

}