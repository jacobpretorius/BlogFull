using System.Linq;
using System.Text;
using System.Web.Mvc;
using BlogFull.Web.Business.Cache;
using BlogFull.Web.Business.Helpers;

namespace BlogFull.Web.Controllers
{
	public class SitemapController : Controller
	{
		/// <summary>
		///     Default root sitemap.
		/// </summary>
		public ActionResult GetSiteMap()
		{
			var baseUrl = "https://" + HttpContext?.Request?.Url?.Host;

			var result = new StringBuilder();
			result.Append("<?xml version='1.0' encoding='UTF-8'?>");
			result.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

			//add homepage and blog page
			result.AppendFormat("<url><loc>{0}/</loc></url>", baseUrl);
			result.AppendFormat("<url><loc>{0}/blog/</loc></url>", baseUrl);

			var posts = StorageHelper.ReadAllPosts();

			//Only add posts that should be indexed
			foreach (var post in posts.Where(w => w.DontIndexPost == false))
			{
				result.AppendFormat("<url><loc>{0}/post/{1}/{2}</loc></url>", baseUrl, post.PostedTime.Year, post.Slug);
			}

			//add category pages
			var categories = MemoryCache.GetAllCategories();
			foreach (var category in categories)
			{
				result.AppendFormat("<url><loc>{0}/blog/category/{1}</loc></url>", baseUrl, category);
			}

			result.AppendFormat("</urlset>");

			return Content(result.ToString(), "text/xml");
		}
	}
}