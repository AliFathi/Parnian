using Parnian.Models;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Parnian.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            //new Initializer().MySeed(db);

            return View(db.Categories.Where(i => !i.isHidden && i.isIndexTile).OrderBy(i => i.priority).ToList<Category>());
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View(new Email());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Contact([Bind(Include = "subject,senderName,senderEmail,text")] Email email)
        {
            if (!ModelState.IsValid)
                return Json(new AjaxResponse
                {
                    title = "فرستاده نشد!",
                    text = "آیا همه‌ی ورودی‌های شما درست است؟",
                    type = "error",
                });

            var subject = $"از سایت پرنیان: {email.subject}";
            var body =
                "<div style=\"direction: rtl;\">"
                    + $"نام فرستنده: {email.senderName}<br />"
                    + $"ای‌میلشون: {email.senderEmail}<br />"
                    + email.text
                + "</div>";

            try
            {
                EmailService.Send(subject, body);

                return Json(new AjaxResponse
                {
                    title = "فرستاده شد!",
                    text = "ممنون که با ما تماس گرفتید",
                    type = "success",
                });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse
                {
                    title = "فرستاده نشد!",
                    text = ex.Message,
                    type = "error",
                });
            }
        }

        [HttpGet]
        public ActionResult Gallery(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Category category = db.Categories.Find(id);

            if (category == null) return HttpNotFound();

            CategoryType type = category.type;

            string title = category.title;

            switch (type)
            {
                case CategoryType.video:
                    IQueryable<Video> videos = db.Videos.Where(i => i.categoryId == id).Where(i => !i.isHidden).OrderBy(i => i.priority);
                    ViewBag.Title = title;
                    return View("VideoGallery", videos.ToList());

                case CategoryType.article:
                    IQueryable<Journal> journals = db.Journals.Where(i => i.categoryId == id).Where(i => !i.isHidden).OrderBy(i => i.priority);
                    ViewBag.Title = title;
                    return View("JournalGallery", journals.ToList());

                case CategoryType.graphic:
                    IQueryable<Graphic> graphics = db.Graphics.Where(i => i.categoryId == id).Where(i => !i.isHidden).OrderBy(i => i.priority);
                    ViewBag.Title = title;
                    return View("GraphicGallery", graphics.ToList());

                default:
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Journal(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var journal = db.Journals.Find(id);
            if (journal == null)
                return HttpNotFound();

            return View(journal);
        }

        [HttpGet]
        public void Thumbnail(string path, int width = 51)
        {
            var physialPath = Server.MapPath(path);
            if (System.IO.File.Exists(physialPath))
            {
                var image = new WebImage(physialPath);
                float fH = (width * image.Height) / image.Width;
                int H = (int)Math.Floor(fH);
                image.Resize(width, H).Crop(1, 1).Write();
            }
        }

        [HttpGet]
        public FileResult Download(string url)
        {
            string path = Server.MapPath(url);
            FileInfo file = new FileInfo(path);
            string fileName = file.Name;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
                db.Dispose();

            base.Dispose(disposing);
        }

        #region Helpers

        public class AjaxResponse
        {
            public string title;
            public string text;
            public string type;
        }

        private Page NullPage()
        {
            var page = new Page
            {
                title = "اطلاعات موجود نیست",
                description = "اطلاعات موجود نیست"
            };

            return page;
        }

        private Page NullPage(string title)
        {
            var page = new Page
            {
                title = title,
                description = "اطلاعات موجود نیست"
            };

            return page;
        }

        #endregion
    }
}