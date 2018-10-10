using System.Web.Mvc;
using BlogFull.Web.Business.Cache;
using BlogFull.Web.Controllers.Base;
using BlogFull.Web.Models.ViewModels;

namespace BlogFull.Web.Controllers
{
    public class PostController : BaseController
    {
        public ActionResult Index(PostPageViewModel currentPage, int year, string slug = "")
        {
            //we want to allow just /year being hit, and redirected to blog home
            if (string.IsNullOrWhiteSpace(slug))
            {
                return RedirectToAction("Index", "Blog");
            }

            //try get the post from memcache first
            var post = MemoryCache.GetPost(year, slug);
            if (post != null)
            {
                currentPage.Slug = post.Slug;
                currentPage.Title = post.Title;
                currentPage.PostedTime = post.PostedTime;
                currentPage.BodyHtml = post.BodyHtml;
                currentPage.ReadTime = post.ReadTime;
                currentPage.Categories = post.Categories;
                currentPage.AuthorName = SettingsCache.GetAuthor();

                //replace defaults in base page
                currentPage.DontIndexPage = post.DontIndexPost;
                currentPage.WindowTitle = post.Title;
                if (!string.IsNullOrWhiteSpace(post.MetaDescPost))
                {
                    currentPage.MetaDescription = post.MetaDescPost;
                }

                return View("Post", currentPage);
            }

            return RedirectToAction("NotFound", "Error");
        }

        //in a perfect world we wouldn't need this. Unfortunately the way Disqus handles urls sucks
        //so we need to account for it by redirecting "their" format to our format.
        public ActionResult Redirect(PostPageViewModel currentPage, string s, int y)
        {
            return RedirectToAction("Index", new { year = y, slug = s });
        }
    }
}