using System.Web.Mvc;
using System.Data.Entity;
using BookShop2020.Models;

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
    }
}