using System.Web;
using System.Web.Optimization;

namespace Khareedo
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                     "~/Scripts/bootstrap.js",
                     "~/Scripts/respond.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/font-awesome.css",
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/jquery.smartmenus.bootstrap.css",
                      "~/Content/css/jquery.simpleLens.css",
                      "~/Content/css/slick.css",
                      "~/Content/css/nouislider.css",
                      "~/Content/css/theme-color/green-theme.css",
                      "~/Content/css/sequence-theme.modern-slide-in.css",
                      "~/Content/css/style.css"));
             bundles.Add(new ScriptBundle("~/Content/js").Include(
                    "~/Content/js/jquery.unobtrusive-ajax.min.js",
                    "~/Content/js/bootstrap.js",                    
                    "~/Content/js/jquery.smartmenus.js",
                    "~/Content/js/jquery.smartmenus.bootstrap.js",
                    "~/Content/js/sequence.js",
                    "~/Content/js/sequence-theme.modern-slide-in.js",
                    "~/Content/js/jquery.simpleGallery.js",
                    "~/Content/js/jquery.simpleLens.js",
                    "~/Content/js/slick.js",
                    "~/Content/js/nouislider.js",
                    "~/Content/js/custom.js"
                   ));
        }
    }
}
