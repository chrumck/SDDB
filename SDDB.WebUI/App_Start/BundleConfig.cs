using System.Web;
using System.Web.Optimization;

namespace SDDB.WebUI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js", "~/Scripts/respond.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css", "~/Content/SDDB/bootstrap-custom.css"));

            //DataTables
            bundles.Add(new ScriptBundle("~/bundles/DataTables").Include("~/Scripts/DataTables/jquery.dataTables.js", "~/Scripts/DataTables/dataTables.bootstrap.js"));
            bundles.Add(new StyleBundle("~/bundles/DataTables-CSS").Include("~/Content/DataTables/css/jquery.dataTables.css", "~/Content/DataTables/css/dataTables.bootstrap.css"));

            //jqueryUI
            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include("~/Scripts/jquery-ui-{version}.js"));
            bundles.Add(new StyleBundle("~/bundles/jquery-ui-CSS").Include("~/Content/themes/base/all.css", "~/Content/SDDB/jqueryui-custom.css"));

            //bootstrap toggle
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-toggle").Include("~/Scripts/BootstrapToggle/bootstrap-toggle.js"));
            bundles.Add(new StyleBundle("~/bundles/bootstrap-toggle-CSS").Include("~/Content/BootstrapToggle/bootstrap-toggle.css"));

            //magicsuggest
            bundles.Add(new ScriptBundle("~/bundles/magicsuggest").Include("~/Scripts/MagicSuggest/magicsuggest.js"));
            bundles.Add(new StyleBundle("~/bundles/magicsuggest-css").Include("~/Content/MagicSuggest/magicsuggest.css"));

            //moment.js
            bundles.Add(new ScriptBundle("~/bundles/moment").Include("~/Scripts/moment.js"));

            //bootstrap-datetimepicker
            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datetimepicker").Include("~/Scripts/bootstrap-datetimepicker.js"));
            bundles.Add(new StyleBundle("~/bundles/bootstrap-datetimepicker-CSS").Include("~/Content/bootstrap-datetimepicker.css"));
        }
    }
}