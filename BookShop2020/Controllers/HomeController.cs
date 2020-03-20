using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
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

            var books = db.Books.Include(b=> b.Genre);
            if (!(categoryId == null || categoryId ==0))
            {
               books = books.Where(b => b.CategoryId == categoryId); 
            }
            return View(books.ToList());
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
    }
}