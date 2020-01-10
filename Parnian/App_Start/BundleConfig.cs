using System.Web.Optimization;

namespace Parnian
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Scripts
            //-------------------------------------------------------------------------
            // jquery and bootstrap
            bundles.Add(new ScriptBundle("~/bundles/jquery-bootstrap").Include(
                        "~/Scripts/jquery-2.1.4.min.js",
                        "~/Scripts/bootstrap.min.js"));

            // jquery validation
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"));

            // site-layout
            bundles.Add(new ScriptBundle("~/bundles/site-layout").Include(
                        "~/plugins/lightslider/js/lightslider.min.js",
                        "~/Scripts/site.js"));

            // jquery ui
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery.ui.core.js",
                        "~/Scripts/jquery.ui.widget.js",
                        "~/Scripts/jquery.ui.mouse.js",
                        "~/Scripts/jquery.ui.sortable.js"));

            bundles.Add(new ScriptBundle("~/bundles/admindex").Include(
                        "~/Scripts/bootbox.min.js",
                        "~/Scripts/admindex.js"));

            // Styles
            //-------------------------------------------------------------------------

            bundles.Add(new StyleBundle("~/Content/site").Include(
                        "~/Content/bootstrap-my-rtl.min.css",
                        "~/Content/site.css",
                        "~/plugins/lightslider/css/lightslider.min.css"));

            bundles.Add(new StyleBundle("~/Content/adminCss").Include(
                        "~/Content/bootstrap-my-rtl.min.css",
                        "~/Content/admin.css"));

            // photoswipe
            //-------------------------------------------------------------------------
            bundles.Add(new ScriptBundle("~/bundles/photoswipe").Include(
                        "~/plugins/photoswipe/photoswipe.min.js",
                        "~/plugins/photoswipe/photoswipe-ui-default.min.js"));

            bundles.Add(new StyleBundle("~/Content/photoswipe").Include(
                        "~/plugins/photoswipe/photoswipe.css",
                        "~/plugins/photoswipe/default-skin.css"));
        }
    }
}
