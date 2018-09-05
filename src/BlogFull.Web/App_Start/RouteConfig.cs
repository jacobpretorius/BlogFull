using System.Web.Mvc;
using System.Web.Routing;

namespace BlogFull.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            routes.MapRoute(
                "Post",
                "post/{year}/{slug}",
                new {controller = "Post", action = "Index", slug = UrlParameter.Optional }
            );

            //handle redirect
            routes.MapRoute(
                "Redirect",
                "post-redirect/",
                new { controller = "Post", action = "Redirect"}
            );

            routes.MapRoute(
                "Category",
                "blog/category/{category}/{pager}",
                new { controller = "Blog", action = "Category", pager = UrlParameter.Optional }
            );

            routes.MapRoute(
                "BlogPaging",
                "blog/{pager}",
                new { controller = "Blog", action = "Index" }
            );

			routes.MapRoute(
	            "Sitemap",
	            "sitemap.xml",
	            new { controller = "Sitemap", action = "GetSiteMap" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new {controller = "Home", action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}