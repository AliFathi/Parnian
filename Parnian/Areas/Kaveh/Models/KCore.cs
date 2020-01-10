using System.Collections.Generic;
using System.Web.Hosting;

namespace Parnian.Areas.Kaveh.Models
{
    public static class KCore
    {
        public static List<string> ImageExtensions = new List<string> { ".jpeg", ".jpg", ".png", ".ico", ".gif" };

        public static List<string> ReservedNames = new List<string> {
            ".", "CON", "PRN", "AUX", "CLOCK$", "NUL",
            "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        public static List<string> TextExtensions = new List<string> {
            "srt", "html", "htm", "csv", "conf","config", "txt","xml","asp","rdf","tsv","text","lrc",
            "log","cmd","dat","gnu","gpl","html5","htmls","inc","js","cs","map","md","me",
            "readme","reg","sbv","skv"
        };

        public static bool IsValidFileName(string name)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                if (name.Contains(c.ToString()))
                    return false;
            }

            var upperName = name.ToUpper();
            foreach (string r in ReservedNames)
            {
                if (upperName.StartsWith(r))
                    return false;
            }

            return true;
        }

        public static string PhysicalPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }

        public static string VirtualPath(string physicalPath)
        {
            return physicalPath.Replace(HostingEnvironment.MapPath("~/"), "/").Replace(@"\", "/");
        }
    }
}