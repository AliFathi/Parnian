using System.Collections.Generic;

namespace Parnian.Areas.Kaveh.Models
{
    public class KNavPaneItem
    {
        public string name { get; set; }

        public string path { get; set; }

        public List<KNavPaneItem> list { get; set; }
    }
}