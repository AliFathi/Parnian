using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static System.Configuration.ConfigurationManager;

namespace Parnian
{
    public static class EmailService
    {
        private readonly static string _FROM = AppSettings["mail:from"]; // alifathi1388@gmail.com
        private readonly static string _HOST = AppSettings["mail:host"]; // smtp.gmail.com
        private readonly static string _PORT = AppSettings["mail:port"];
        private readonly static string _USERNAME = AppSettings["mail:username"]; // alifathi1388@gmail.com
        private readonly static string _PASSWORD = AppSettings["mail:password"]; // gmail password

        public static void Send(string subject, string body)
        {
            var message = CreateMessage(to: _FROM, subject, body);

            using (var client = CreateClient())
                client.Send(message);
        }

        public static async Task SendAsync(string subject, string body)
        {
            var message = CreateMessage(to: _FROM, subject, body);

            using (var client = CreateClient())
                await client.SendMailAsync(message);
        }

        public static void Send(string to, string subject, string body)
        {
            var message = CreateMessage(to, subject, body);

            using (var client = CreateClient())
                client.Send(message);
        }

        public static async Task SendAsync(string to, string subject, string body)
        {
            var message = CreateMessage(to, subject, body);

            using (var client = CreateClient())
                await client.SendMailAsync(message);
        }

        #region Helpers

        private static SmtpClient CreateClient()
        {
            return new SmtpClient(_HOST, int.Parse(_PORT))
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(_USERNAME, _PASSWORD)
            };
        }

        private static MailMessage CreateMessage(string to, string subject, string body)
        {
            return new MailMessage(_FROM, to)
            {
                Subject = subject,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
        }

        #endregion
    }
}
