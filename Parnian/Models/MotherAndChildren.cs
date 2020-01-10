using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Parnian.Models
{
    /* Mothr : Mother of many models! i.e, many of models inherit from Mother.
    ---------------------------------------------------------------------------*/
    public class Mother
    {
        [Key]
        [Display(Name = "شناسه")]
        public int id { get; set; }

        [Display(Name = "اولویت")]
        public int priority { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "باید یک نام وارد کنید")]
        public string title { get; set; }

        [AllowHtml]
        [Display(Name = "شرح")]
        [DataType(DataType.MultilineText)]
        public string description { get; set; }

        [Display(Name = "نام تصویر")]
        public string imageName { get; set; }

        [Display(Name = "ابعاد تصویر")]
        public string imageSize { get; set; }

        [Display(Name = "نام پدیدآورنده")]
        public string creatorName { get; set; }

        [Display(Name = "زمان ایجاد")]
        public string creationTime { get; set; }

        [Display(Name = "نام آخرین ویرایشگر")]
        public string lastEditorName { get; set; }

        [Display(Name = "زمان آخرین ویرایش")]
        public string lastEditTime { get; set; }

        [Display(Name = "پنهان باشد؟")]
        public bool isHidden { get; set; }
    }

    /* Page
    ---------------------------------------------------------------------------*/
    public class Page : Mother { }

    /* Slide
    ---------------------------------------------------------------------------*/
    public class Slide : Mother
    {
        [Display(Name = "آدرس")]
        [DataType(DataType.Url)]
        public string url { get; set; }
    }

    /* Category
    ---------------------------------------------------------------------------*/
    public class Category : Mother
    {
        [Display(Name = "برای کدام نوع؟")]
        public CategoryType type { get; set; }

        [Display(Name = "در نوار بالای صفحه باشد؟")]
        public bool isMenuItem { get; set; }

        [Display(Name = "در صفحه‌ی اول باشد؟")]
        public bool isIndexTile { get; set; }
    }

    public enum CategoryType
    {
        [Display(Name = "ویدئو")]
        video = 1,
        [Display(Name = "نشریه")]
        journal,
        [Display(Name = "مقاله")]
        article,
        [Display(Name = "نگاره")]
        graphic
    }

    /* Video
    ---------------------------------------------------------------------------*/
    public class Video : Mother
    {
        [Required(ErrorMessage = "فایل با فرمت m4v الزامیست")]
        [Display(Name = "فیلم M4V")]
        public string videoName_M4V { get; set; }

        [Display(Name = "فیلم WebM")]
        public string videoName_WebM { get; set; }

        [Display(Name = "فیلم Ogg")]
        public string videoName_Ogg { get; set; }

        [Display(Name = "فیلم MP4")]
        public string videoName_MP4 { get; set; }

        [Display(Name = "دسته")]
        [Required(ErrorMessage = "برای این فیلم یک دسته مشخص کنید")]
        public int categoryId { get; set; }

        public virtual Category category { get; set; }
    }

    /* Journal
    ---------------------------------------------------------------------------*/
    public class Journal : Mother
    {
        [AllowHtml]
        [Display(Name = "چکیده")]
        [DataType(DataType.MultilineText)]
        public string excerpt { get; set; }

        [Display(Name = "دسته")]
        [Required(ErrorMessage = "برای این نوشته یک دسته مشخص کنید")]
        public int categoryId { get; set; }

        public virtual Category category { get; set; }
    }

    /* Graphic
    ---------------------------------------------------------------------------*/
    public class Graphic : Mother
    {
        [Display(Name = "دسته")]
        [Required(ErrorMessage = "برای این نگاره یک دسته مشخص کنید")]
        public int categoryId { get; set; }

        public virtual Category category { get; set; }
    }
}