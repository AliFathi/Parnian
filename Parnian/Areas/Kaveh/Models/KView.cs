namespace Parnian.Areas.Kaveh.Models
{
    public class KView
    {
        public KView(bool showStatus = true)
        {
            if (showStatus)
            {
                this.hasToolBar = true;
                this.hasAddressBar = true;
                this.hasNavPane = true;
                this.hasViewBox = true;
                this.hasStatusBar = true;
                this.hasChooseBar = true;
                this.hasUploadForm = true;
            }
            else
            {
                this.hasToolBar = false;
                this.hasAddressBar = false;
                this.hasNavPane = false;
                this.hasViewBox = true;
                this.hasStatusBar = false;
                this.hasChooseBar = false;
                this.hasUploadForm = false;
            }
        }

        public bool hasToolBar { get; set; }
        public bool hasAddressBar { get; set; }
        public bool hasNavPane { get; set; }
        public bool hasViewBox { get; set; }
        public bool hasStatusBar { get; set; }
        public bool hasChooseBar { get; set; }
        public bool hasUploadForm { get; set; }
    }
}