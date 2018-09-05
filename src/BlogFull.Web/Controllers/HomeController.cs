using System.Web.Mvc;

namespace BlogFull.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Home");
        }
    }
}