using MD.PersianDateTime;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Parnian.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Parnian.Controllers
{
    public class SlideController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Slide
        public ActionResult Index(string sortkey = "priority")
        {
            ViewBag.sortkey = new sortkey
            {
                title = "title",
                priority = "priority",
                isHidden = "isHidden",
            };

            var context = db.Slides;

            switch (sortkey)
            {
                case "title":
                    ViewBag.sortkey.title = "titleDesc";
                    return View(context.OrderBy(i => i.title).ToList());

                case "titleDesc":
                    return View(context.OrderByDescending(i => i.title).ToList());

                case "isHidden":
                    ViewBag.sortkey.isHidden = "isHiddenDesc";
                    return View(context.OrderBy(i => i.isHidden).ToList());

                case "isHiddenDesc":
                    return View(context.OrderByDescending(i => i.isHidden).ToList());

                case "priorityDesc":
                    return View(context.OrderByDescending(i => i.priority).ToList());

                default:
                    ViewBag.sortkey.priority = "priorityDesc";
                    return View(context.OrderBy(i => i.priority).ToList());
            }
        }

        public struct sortkey
        {
            public string title;
            public string priority;
            public string isHidden;
        }

        // GET: Slide/Create
        public ActionResult Create()
        {
            ViewBag.isedit = false;
            return View("CreateAndEdit", new Slide());
        }

        // POST: Slide/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,description,imageName,imageSize,isHidden,url")] Slide model)
        {
            if (ModelState.IsValid)
            {
                model.description = WebUtility.HtmlEncode(model.description);
                model.creationTime = PersianDateTime.Now.ToLongDateTimeString();
                model.creatorName = User.Identity.GetUserId<string>();
                model.priority = db.Slides.Count() + 1;
                db.Slides.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.isedit = false;
            return View("CreateAndEdit", model);
        }

        // GET: Slide/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Slide model = db.Slides.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            model.description = WebUtility.HtmlDecode(model.description);

            ApplicationUser editor = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(model.lastEditorName);
            ApplicationUser creator = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(model.creatorName);
            if (editor != null) ViewBag.editorname = editor.UserName;
            else ViewBag.editorname = "کاربر پاک شده";
            if (creator != null) ViewBag.creator = creator.UserName;
            else ViewBag.creator = "کاربر پاک شده";

            ViewBag.isedit = true;
            return View("CreateAndEdit", model);
        }

        // POST: Slide/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,priority,title,description,imageName,imageSize,isHidden,creationTime,creatorName,url")] Slide model)
        {
            if (ModelState.IsValid)
            {
                model.description = WebUtility.HtmlEncode(model.description);
                model.lastEditorName = User.Identity.GetUserId<string>();
                model.lastEditTime = PersianDateTime.Now.ToLongDateTimeString();
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.isedit = true;
            return View("CreateAndEdit", model);
        }

        //Group Actions
        //----------------------------------------------------------------------------------
        //SetPriority
        [HttpPost]
        public string SetPriority(string jsonArray)
        {
            model[] modelArray = new JavaScriptSerializer().Deserialize<model[]>(jsonArray);
            foreach (model model in modelArray)
            {
                Slide item = db.Slides.Find(model.i);
                item.priority = model.p;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            return "تعیین اولویت انجام شد.";
        }

        public class model
        {
            public int i;
            public int p;
        }

        //GroupDelete
        [HttpPost]
        public string GroupDelete(string jsonArray)
        {
            int[] idArray = new JavaScriptSerializer().Deserialize<int[]>(jsonArray);

            foreach (int id in idArray)
            {
                Slide model = db.Slides.Find(id);
                db.Slides.Remove(model);
                db.SaveChanges();
            }
            return "حذف گروهی انجام شد.";
        }

        //GroupDontShow
        [HttpPost]
        public string GroupdoNotShow(string jsonArray)
        {
            int[] idArray = new JavaScriptSerializer().Deserialize<int[]>(jsonArray);

            foreach (int id in idArray)
            {
                Slide model = db.Slides.Find(id);
                model.isHidden = true;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return "پنهان کردن گروهی انجام شد.";
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
