using System.Web.Mvc;
using BlogFull.Web.Business.Cache;
using BlogFull.Web.Controllers.Base;
using BlogFull.Web.Models.ViewModels;

namespace BlogFull.Web.Controllers
{
    public class BlogController : BaseController
    {
        public ActionResult Index(BlogPageViewModel currentPage, int pager = 0)
        {
            currentPage.BlogPosts = MemoryCache.GetTenPosts(pager);
            currentPage.Categories = MemoryCache.GetAllCategories();
            currentPage.Page = pager;

            return View("Blog", currentPage);
        }

        public ActionResult Category(BlogPageViewModel currentPage, string category, int pager = 0)
        {
            currentPage.BlogPosts = MemoryCache.GetTenPostsFromCategory(category, pager);
            currentPage.Categories = MemoryCache.GetAllCategories();
            currentPage.Page = pager;
            currentPage.ActiveCategory = category;

            return View("Blog", currentPage);
        }
    }
}