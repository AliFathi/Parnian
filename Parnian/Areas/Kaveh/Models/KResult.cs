namespace Parnian.Areas.Kaveh.Models
{
    public class KResult
    {
        public KResult()
        {
            this.status = KStatus.success;

            this.message = "";

            this.data = null;
        }

        public KStatus status { get; set; }

        public string message { get; set; }

        public dynamic data { get; set; }
    }

    public enum KStatus
    {
        success = 0,
        error,
        warning
    }
}