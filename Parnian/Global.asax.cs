using Parnian.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parnian
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            //
            //Database.SetInitializer(new Initializer());
        }

        public static List<Setting> GetSettings()
        {
            using (var db = new ApplicationDbContext())
                return db.Settings.AsNoTracking().ToList();
        }

        public static string[] GetSettings(string[] keys)
        {
            var settings = new List<Setting>();

            using (var db = new ApplicationDbContext())
                settings = db.Settings.Where(s => keys.Contains(s.key)).AsNoTracking().ToList();

            var result = new List<string>();
            foreach (string key in keys)
            {
                var setting = settings.FirstOrDefault(s => s.key == key);
                result.Add(setting == null ? $"ویژگی {key} تعریف نشده!" : setting.value);
            }

            return result.ToArray();
        }

        public static List<Category> GetMenuItems()
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Categories.Where(i => !i.isHidden && i.isMenuItem)
                    .OrderBy(i => i.priority)
                    .AsNoTracking()
                    .ToList();
            }
        }

        public static List<Slide> GetSlides()
        {
            using (var db = new ApplicationDbContext())
                return db.Slides.Where(i => !i.isHidden).OrderBy(i => i.priority).AsNoTracking().ToList();
        }
    }
}
