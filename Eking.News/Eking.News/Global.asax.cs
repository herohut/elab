using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Eking.News
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Mine
            var txt = ConfigurationManager.AppSettings["HomeView_ItemPerGroup"];
            WebConfig.HomeViewItemPerGroup = string.IsNullOrEmpty(txt) ? WebConfig.HomeViewItemPerGroup : int.Parse(txt);
            txt = ConfigurationManager.AppSettings["GroupView_ItemPerPage"];
            WebConfig.GroupViewItemPerPage = string.IsNullOrEmpty(txt) ? WebConfig.GroupViewItemPerPage : int.Parse(txt);
        }
    }

    public class WebConfig
    {
        public static int HomeViewItemPerGroup = 10;
        public static int GroupViewItemPerPage = 20;
    }
}