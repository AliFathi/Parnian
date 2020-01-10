using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parnian.Models
{
    public class SettingsViewModel
    {
        public IEnumerable<Setting> Settings { get; set; }

        public UserViewModel User { get; set; }
    }
    
    public class UserViewModel
    {
        [Required(ErrorMessage = "نام کاربری را وارد کنبد")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required(ErrorMessage = "رایانامه‌ی را وارد کنبد")]
        [Display(Name = "رایانامه‌")]
        [EmailAddress(ErrorMessage = "این رایانامه درست نیست")]
        public string Email { get; set; }

        [Display(Name = "سطح دسترسی‌")]
        public List<String> Roles { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "رمز کنونی را وارد کنبد")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز کنونی")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "رمز جدید را وارد کنبد")]
        [StringLength(100, ErrorMessage = "رمز حداقل ۶ حرفیست", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز جدید")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تایید رمز جدید")]
        [Compare("NewPassword", ErrorMessage = "رمز جدید تایید نشد")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeUsernameViewModel
    {
        [Required(ErrorMessage = "نام کاربری کنونی را وارد کنبد")]
        [Display(Name = "نام کاربری کنونی")]
        public string OldUsername { get; set; }

        [Required(ErrorMessage = "نام کاربری جدید را وارد کنبد")]
        [Display(Name = "نام کاربری جدید")]
        public string NewUsername { get; set; }

        [Display(Name = "تایید نام کاربری جدید")]
        [Compare("NewUsername", ErrorMessage = "نام کاربری جدید تایید نشد")]
        public string ConfirmUsername { get; set; }
    }

    public class ChangeEmailViewModel
    {
        [Required(ErrorMessage = "رایانامه‌ی کنونی را وارد کنبد")]
        [Display(Name = "رایانامه‌ی کنونی")]
        [EmailAddress(ErrorMessage = "این رایانامه درست نیست")]
        public string OldEmail { get; set; }

        [Required(ErrorMessage = "رایانامه‌ی جدید را وارد کنبد")]
        [Display(Name = "رایانامه‌ی جدید")]
        [EmailAddress(ErrorMessage = "این رایانامه درست نیست")]
        public string NewEmail { get; set; }

        [Display(Name = "تایید رایانامه‌ی جدید")]
        [EmailAddress(ErrorMessage = "این رایانامه درست نیست")]
        [Compare("NewEmail", ErrorMessage = "رایانامه‌ی جدید تایید نشد")]
        public string ConfirmEmail { get; set; }
    }

    public class AddUserViewModel
    {
        [Required(ErrorMessage = "نام کاربری‌ را وارد کنید")]
        [Display(Name = "نام کاربری‌")]
        public string Username { get; set; }

        [Required(ErrorMessage = "رمز‌ را وارد کنبد")]
        [StringLength(100, ErrorMessage = "رمز حداقل ۶ حرفیست", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تایید رمز")]
        [Compare("Password", ErrorMessage = "رمز تایید نشد")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "رایانامه را وارد کنبد")]
        [Display(Name = "رایانامه‌")]
        [EmailAddress(ErrorMessage = "این رایانامه درست نیست")]
        public string Email { get; set; }

        [Required(ErrorMessage = "سطح دسترسی را تعیین کنید")]
        [Display(Name = "سطح دسترسی")]
        public Roles Role { get; set; }
    }

    //public class IndexViewModel
    //{
    //    public bool HasPassword { get; set; }
    //    public IList<UserLoginInfo> Logins { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public bool TwoFactor { get; set; }
    //    public bool BrowserRemembered { get; set; }
    //}

    //public class ManageLoginsViewModel
    //{
    //    public IList<UserLoginInfo> CurrentLogins { get; set; }
    //    public IList<AuthenticationDescription> OtherLogins { get; set; }
    //}

    //public class FactorViewModel
    //{
    //    public string Purpose { get; set; }
    //}

    //public class SetPasswordViewModel
    //{
    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "New password")]
    //    public string NewPassword { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm new password")]
    //    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }
    //}

    //public class UsersInRole
    //{
    //    public string RoleName { get; set; }
    //    public List<ApplicationUser> Users { get; set; }
    //}

    //public class AddPhoneNumberViewModel
    //{
    //    [Required]
    //    [Phone]
    //    [Display(Name = "Phone Number")]
    //    public string Number { get; set; }
    //}

    //public class VerifyPhoneNumberViewModel
    //{
    //    [Required]
    //    [Display(Name = "Code")]
    //    public string Code { get; set; }

    //    [Required]
    //    [Phone]
    //    [Display(Name = "Phone Number")]
    //    public string PhoneNumber { get; set; }
    //}

    //public class ConfigureTwoFactorViewModel
    //{
    //    public string SelectedProvider { get; set; }
    //    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    //}
}