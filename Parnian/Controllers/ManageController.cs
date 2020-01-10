using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Parnian.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Parnian.Controllers
{
    public class ManageController : Controller
    {
        // fields

        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // properties

        private ApplicationSignInManager SignInManager => HttpContext.GetOwinContext().Get<ApplicationSignInManager>();

        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Settings(ManageMessageId? message)
        {
            ViewBag.Subtitle =
                message == ManageMessageId.ChangePasswordSuccess ? "رمز حساب کاربری شما عوض شد"
                : message == ManageMessageId.ChangeUsernameAndEmailSuccess ? "اطلاعات حساب کاربری شما عوض شد"
                : message == ManageMessageId.Error ? "خطایی رخ داده"
                : "";
            //: message == ManageMessageId.ChangeEmailSuccess ? "رایانامه‌ی شما عوض شد"
            //: message == ManageMessageId.ChangeUsernameSuccess ? "نام کاربری شما عوض شد"
            //: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
            //: message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
            //: message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
            //: message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."

            List<Setting> settings = db.Settings.ToList<Setting>();

            ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());

            UserViewModel UVM = new UserViewModel();

            if (user != null)
            {
                UVM.Username = user.UserName;
                UVM.Email = user.Email;
                UVM.Roles = user.RolesList();
            }

            SettingsViewModel SVM = new SettingsViewModel
            {
                Settings = settings,
                User = UVM,
            };

            return View(SVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser([Bind(Prefix = "User", Include = "UserName,Email")] UserViewModel model)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = UserManager.FindById(userId);
            if (user != null)
            {
                user.UserName = model.Username;
                user.Email = model.Email;
                IdentityResult updateResult = UserManager.Update(user);
                if (updateResult.Succeeded)
                {
                    SignInManager.SignIn(user, isPersistent: false, rememberBrowser: true);
                    return RedirectToAction("Settings", new { message = ManageMessageId.ChangeUsernameAndEmailSuccess });
                }
            }
            return RedirectToAction("Settings", new { message = ManageMessageId.Error });
        }

        [HttpGet]
        public ActionResult Statistics()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FileManager()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityResult result = UserManager.ChangePassword(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (user != null)
                {
                    SignInManager.SignIn(user, isPersistent: false, rememberBrowser: true);
                }
                return RedirectToAction("Settings", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Users()
        {
            return View(db.Users.ToList<ApplicationUser>());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult AddUser(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser newUser = new ApplicationUser { UserName = model.Username, Email = model.Email, EmailConfirmed = true };
                IdentityResult createResult = UserManager.Create(newUser, model.Password);
                if (createResult.Succeeded)
                {
                    IdentityResult addToRoleResult = UserManager.AddToRole(newUser.Id, model.Role.ToString());
                    if (addToRoleResult.Succeeded)
                    {
                        return RedirectToAction("Users");
                    }
                    AddErrors(addToRoleResult);
                    return View(model);
                }
                AddErrors(createResult);
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public string DeleteUsers(string jsonArray)
        {
            string[] idArray = new JavaScriptSerializer().Deserialize<string[]>(jsonArray);
            string result = "حذف گروهی انجام شد.";

            foreach (string id in idArray)
            {
                ApplicationUser user = UserManager.FindById(id);
                IdentityResult deleteResult = UserManager.Delete(user);
                if (!deleteResult.Succeeded)
                {
                    foreach (string err in deleteResult.Errors)
                    {
                        result += "<br />" + err;
                    }
                }
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UserManager != null)
                    UserManager.Dispose();

                if (SignInManager != null)
                    SignInManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            ChangeUsernameAndEmailSuccess,
            ChangePasswordSuccess,
            Error
        }

        #endregion Helpers
    }
}