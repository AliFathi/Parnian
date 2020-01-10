using System.Collections.Generic;

namespace Parnian.Areas.Kaveh.Models
{
    public class KFileSystemInfo
    {
        public string name { get; set; }
        public string extension { get; set; }
        public string path { get; set; }
        public string creation { get; set; }
        //public string lastAccess { get; set; }
        //public string lastWrite { get; set; }
        public string imageDimensions { get; set; }
        public long length { get; set; }
        public bool isDirectory { get; set; }
        public List<KFileSystemInfo> KFSIList { get; set; }
    }
}