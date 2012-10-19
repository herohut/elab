using System.Web;
using System.Web.Optimization;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using dotless.Core;

namespace Eking.News
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui*", "~/Scripts/jquery.ui*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            // MyJs
            bundles.Add(new Bundle("~/MyJs").Include(
                "~/Scripts/jquery.nailthumb.1.1.js",
                "~/Scripts/jquery.mousewheel.js",
                "~/Scripts/jquery.jscrollpane.js",
                "~/Scripts/jquery.ba-bbq.js",
                "~/Scripts/jquery.ba-resize.js"
                ));


            // MyCss
            bundles.Add(new Bundle("~/Content/MyCss").Include(
                "~/Content/jquery.nailthumb.1.1.css",
                "~/Content/jquery.jscrollpane1.css"
                ));


            var jsTransformer = new JsTransformer();
            var cssTransformer = new CssTransformer();
            var nullOrderer = new NullOrderer();
            var cssMinify = new CssMinify();
            var jsMinify = new JsMinify();

            // MyCoffee
            var bundle = new Bundle("~/MyCoffee").Include(
                "~/Scripts/EntryRenderer.coffee",
                "~/Scripts/HomePresenter.coffee",
                "~/Scripts/Application.coffee"
                );
            bundle.Transforms.Add(jsTransformer);
            bundle.Transforms.Add(jsMinify);
            bundle.Orderer = nullOrderer;
            bundles.Add(bundle);

            // MySass
            bundle = new Bundle("~/Content/MySass").Include(
                "~/Content/enews.sass"
                );
            bundle.Transforms.Add(cssTransformer);
            bundle.Transforms.Add(cssMinify);
            bundle.Orderer = nullOrderer;
            bundles.Add(bundle);
        }
    }
}