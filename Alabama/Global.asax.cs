using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Globalization;
using System.Threading;

namespace DKLCT
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        //    routes.MapRoute(
        //    "Site_Language", // Route name
        //    "{site_language}/{controller}/{action}/{id}", // URL with parameters
        //    new { controller = "Home", action = "Index", site_language = Constant.DefaultSiteLanguage, id = UrlParameter.Optional }, // Parameter defaults
        //    new { site_language = AllowCulture() }
        //);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Report", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_End()
        {

        }



        //private static string AllowCulture()
        //{
        //    if (Common.LanguageTable == null)
        //    {
        //        Common.LanguageTable = new AlabamaEntities().mLanguage.ToList();
        //    }

        //    string s = "(admin)|";
        //    for (int i = 0; i < Common.LanguageTable.Count; i++)
        //    {
        //        s += string.Format("({0})|({1})", Common.LanguageTable[i].Code, "admin-" + Common.LanguageTable[i].Code);
        //        if (i < Common.LanguageTable.Count - 1) s += '|';
        //    }
        //    return s;
        //}
    }
}