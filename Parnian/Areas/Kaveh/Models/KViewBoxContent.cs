using System.Collections.Generic;

namespace Parnian.Areas.Kaveh.Models
{
    public class KViewBoxContent
    {
        public string parentName { get; set; }

        public string parentPath { get; set; }

        public List<KViewBoxItem> list { get; set; }
    }

    public class KViewBoxItem
    {
        public string name { get; set; }

        public string extension { get; set; }

        public string path { get; set; }

        public string creation { get; set; }

        public string imageDimensions { get; set; }

        public long length { get; set; }

        public bool isDirectory { get; set; }
    }
}