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
    public class GraphicController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Graphic
        public ActionResult Index(string sortkey = "priority")
        {
            ViewBag.sortkey = new sortkey
            {
                title = "title",
                priority = "priority",
                isHidden = "isHidden",
                category = "category",
            };

            var context = db.Graphics;

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

                case "category":
                    ViewBag.sortkey.category = "categoryDesc";
                    return View(context.OrderBy(i => i.categoryId).ToList());

                case "categoryDesc":
                    return View(context.OrderByDescending(i => i.categoryId).ToList());

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
            public string category;
        }

        // GET: Graphic/Create
        public ActionResult Create()
        {
            ViewBag.isedit = false;
            ViewBag.categoryId = new SelectList(db.Categories.Where(c => c.type == CategoryType.graphic).ToList(), "id", "title", null);
            return View("CreateAndEdit", new Graphic());
        }

        // POST: Graphic/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,description,imageName,imageSize,isHidden,categoryId")] Graphic model)
        {
            if (ModelState.IsValid)
            {
                model.description = WebUtility.HtmlEncode(model.description);
                model.creationTime = PersianDateTime.Now.ToLongDateTimeString();
                model.creatorName = User.Identity.GetUserId<string>();
                model.priority = db.Graphics.Count() + 1;
                db.Graphics.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.isedit = false;
            ViewBag.categoryId = new SelectList(db.Categories.Where(c => c.type == CategoryType.graphic).ToList(), "id", "title", null);
            return View("CreateAndEdit", model);
        }

        // GET: Graphic/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Graphic model = db.Graphics.Find(id);
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
            ViewBag.categoryId = new SelectList(db.Categories.Where(c => c.type == CategoryType.graphic).ToList(), "id", "title", model.categoryId);
            return View("CreateAndEdit", model);
        }

        // POST: Graphic/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,priority,title,description,imageName,imageSize,isHidden,categoryId,creationTime,creatorName")] Graphic model)
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
            ViewBag.categoryId = new SelectList(db.Categories.Where(c => c.type == CategoryType.graphic).ToList(), "id", "title", model.categoryId);
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
                Graphic item = db.Graphics.Find(model.i);
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
                Graphic model = db.Graphics.Find(id);
                db.Graphics.Remove(model);
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
                Graphic model = db.Graphics.Find(id);
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
