using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookShop2020.Models;

namespace BookShop2020.Controllers
{
    [Authorize(Roles = "manager")]
    public class BookController : Controller
    {
        private BookContext db = new BookContext();

        private const int Height = 200, Width = 150;

        private const int MinBookQuantity = 5;
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
        public ActionResult Create([Bind(Include = "Id,Name,Author,Price,Year,CategoryId,Number")] Book book, HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                if(CheckByGraphicsFormat(fileName))
                {
                    SaveImage(upload, fileName);
                    book.ImageUrl = fileName;
                }
            }
            if (!ModelState.IsValid) return View(book);
            db.Books.Add(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void SaveImage(HttpPostedFileBase upload, string fileName)
        {
            var image = new Bitmap(upload.InputStream);
            var smallImg = ResizeImage(image, Width, Height);
            smallImg.Save(Server.MapPath("~/Images/" + fileName));
        }

        private bool CheckByGraphicsFormat(string fileName)
        {
            var ext = fileName.Substring(fileName.Length - 3);
            return string.Compare(ext, "png", StringComparison.Ordinal) == 0 ||
                   string.Compare(ext, "jpg", StringComparison.Ordinal) == 0;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
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
        public ActionResult Edit([Bind(Include = "Id,Name,Author,Price,Year,CategoryId,Number")] Book book, HttpPostedFileBase upload)
        {
            if (upload != null)
            {
                string fileName = System.IO.Path.GetFileName(upload.FileName);
                if (CheckByGraphicsFormat(fileName))
                {
                    SaveImage(upload, fileName);
                    book.ImageUrl = fileName;
                }
            }
            if (!ModelState.IsValid) return View(book);
            db.Entry(book).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
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
        public ActionResult GetSmallNumberBooks()
        {
            var books = db.Books.Include(b => b.Genre).Where(b => b.Number <= MinBookQuantity).ToList(); 
            return View(books);
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