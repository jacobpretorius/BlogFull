using System.Linq;
using System.Web.Mvc;
using BlogFull.Web.Business.Cache;
using BlogFull.Web.Models.ViewModels.Base;

namespace BlogFull.Web.Controllers.Base
{
    public class BaseController : Controller
    {
        public BasePageViewModel Model { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            Model = (BasePageViewModel) context.ActionParameters?.FirstOrDefault(a => a.Value is BasePageViewModel).Value;

            //only continue if we have a model
            if (Model != null)
            {
                //load settings from cache
                var settings = SettingsCache.GetSettings();

                //set defaults
                Model.BlogName = settings.BlogName;
                Model.TagLine = settings.TagLine;
                Model.WindowTitle = settings.DefaultWindowTitle;
                Model.MetaDescription = settings.DefaultMetaDescription;
            }
        }
    }
}