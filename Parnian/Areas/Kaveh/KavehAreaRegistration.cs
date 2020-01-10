using System.Web.Mvc;

namespace Parnian.Areas.Kaveh
{
    public class KavehAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Kaveh";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Kaveh_default",
                "Kaveh/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}