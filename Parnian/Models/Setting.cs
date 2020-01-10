using System.ComponentModel.DataAnnotations;

namespace Parnian.Models
{
    public class Setting
    {
        [Key]
        [Display(Name = "ویژگی")]
        public string key { get; set; }

        [Display(Name = "ارزش")]
        public string value { get; set; }
    }
}