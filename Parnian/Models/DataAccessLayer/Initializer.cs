using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using MD.PersianDateTime;
using static System.Configuration.ConfigurationManager;

namespace Parnian.Models
{
    //public class Initializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    public class Initializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        public void MySeed(ApplicationDbContext context)
        {
            Seed(context);
        }

        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);

            //Add Role
            //-------------------------------------------------------------------------
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                RoleManager.Create(new IdentityRole(Enum.GetName(typeof(Roles), role)));
            }

            context.SaveChanges();

            //Add User
            //-------------------------------------------------------------------------
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var adminUser = new ApplicationUser
            {
                UserName = AppSettings["admin:username"],
                Email = AppSettings["admin:email"],
                EmailConfirmed = true,
                PhoneNumber = AppSettings["admin:phone"],
                PhoneNumberConfirmed = true,
            };

            userManager.Create(adminUser, AppSettings["admin:password"]);
            userManager.AddToRole(adminUser.Id, Roles.Admin.ToString());
            userManager.AddToRole(adminUser.Id, Roles.AppUser.ToString());

            context.SaveChanges();

            //Add Setting
            //-------------------------------------------------------------------------
            List<Setting> list = new List<Setting> {
                new Setting { key = "Name", value = "پرنیان" },
                new Setting { key = "Slogan", value = "مؤسسه‌ی فرهنگی هنری پرنیان اندیشه هنر" },
                new Setting { key = "Description", value = "مؤسسه‌ی فرهنگی هنری پرنیان اندیشه هنر" },
                new Setting { key = "Keywords", value = "تبلیغ، مستند، نماهنگ" },
                new Setting { key = "Logo", value = "logo.png" },
                new Setting { key = "Background", value = "bg.jpg" }
            };

            list.ForEach(s => context.Settings.Add(s));

            context.Categories.Add(new Category
            {
                title = "گروه ویدئو",
                type = CategoryType.video,
                isMenuItem = true,
                creatorName = adminUser.Id,
                creationTime = PersianDateTime.Now.ToLongDateTimeString(),
                priority = 1,
            });

            context.SaveChanges();

            context.Slides.Add(new Slide
            {
                title = "اسلاید اول",
                creatorName = adminUser.Id,
                creationTime = PersianDateTime.Now.ToLongDateTimeString(),
                priority = 1,
            });

            context.SaveChanges();
        }
    }
}