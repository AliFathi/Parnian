using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parnian.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "نام کاربری را وارد کنید")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required(ErrorMessage = "رمز را وارد کنید")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز")]
        public string Password { get; set; }

        [Display(Name = "مرا به یاد بسپار")]
        public bool RememberMe { get; set; }
    }

    public class ReCaptchaResponse
    {
        public bool success { get; set; }
        public string error { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "رایانامه را وارد کنید")]
        [EmailAddress(ErrorMessage = "این رایانامه درست نیست")]
        [Display(Name = "رایانامه")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "نام کاربری را وارد کنید")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required(ErrorMessage = "رمز را وارد کنید")]
        [StringLength(100, ErrorMessage = "رمز باید حداقل ۶ حرفی باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تایید رمز")]
        [Compare("Password", ErrorMessage = "رمز تایید نشد")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    //public class ExternalLoginConfirmationViewModel
    //{
    //    [Required]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}

    //public class ExternalLoginListViewModel
    //{
    //    public string ReturnUrl { get; set; }
    //}

    //public class SendCodeViewModel
    //{
    //    public string SelectedProvider { get; set; }
    //    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    //    public string ReturnUrl { get; set; }
    //    public bool RememberMe { get; set; }
    //}

    //public class VerifyCodeViewModel
    //{
    //    [Required]
    //    public string Provider { get; set; }

    //    [Required]
    //    [Display(Name = "Code")]
    //    public string Code { get; set; }
    //    public string ReturnUrl { get; set; }

    //    [Display(Name = "Remember this browser?")]
    //    public bool RememberBrowser { get; set; }

    //    public bool RememberMe { get; set; }
    //}

    //public class ForgotViewModel
    //{
    //    [Required]
    //    [Display(Name = "رایانامه")]
    //    public string Email { get; set; }
    //}

    //public class RegisterViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Password")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm password")]
    //    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }
    //}
}
