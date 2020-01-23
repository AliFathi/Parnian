using System.Web.Mvc;

namespace Parnian
{
    public static class MvcHelperExtensions
    {
        public static string Thumbnail(this UrlHelper url, string path, int width)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "/App_Files/UI/no-img.jpg";

            return url.Action("Thumbnail", "FileManager", new { area = "Kaveh", path, width });
        }
    }
}
