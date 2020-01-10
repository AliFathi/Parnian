using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Parnian.Models
{
    public class HomeViewModel
    {
        public List<Category> categories { get; set; }
        public List<Slide> slides { get; set; }
    }

    public class ContactViewModel
    {
        public Page page { get; set; }
        public Email email { get; set; }
    }

    public class Email
    {
        [Display(Name = "موضوع (الزامی)")]
        [Required(ErrorMessage = "لطفا موضوع پیام را بنویسید")]
        public string subject { get; set; }

        [Display(Name = "نام (الزامی)")]
        [Required(ErrorMessage = "لطفا نام خود را وارد کنید")]
        public string senderName { get; set; }

        [Display(Name = "رایانامه (الزامی)")]
        [Required(ErrorMessage = "لطفا رایانامه‌ی خود را وارد کنید")]
        [DataType(DataType.EmailAddress, ErrorMessage = "این رایانامه درست نیست")]
        public string senderEmail { get; set; }

        [AllowHtml]
        [Display(Name = "پیام شما")]
        [Required(ErrorMessage = "لطفا پیام خود را بنویسید")]
        [DataType(DataType.MultilineText)]
        public string text { get; set; }
    }
}