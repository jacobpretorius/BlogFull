using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BlogFull.Web.Business.Helpers;
using BlogFull.Web.Controllers.Base;
using BlogFull.Web.Models.Data;
using BlogFull.Web.Models.ViewModels;

namespace BlogFull.Web.Controllers
{
	public class AdminController : BaseController
	{
		public ActionResult Index(LoginPageViewModel currentPage)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			return View("Login", currentPage);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult SubmitLogin(LoginPageViewModel currentPage)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			if (ModelState.IsValid)
			{
				if (!string.IsNullOrWhiteSpace(currentPage.Username) && !string.IsNullOrWhiteSpace(currentPage.Password))
				{
					//prevent brute force first
					if (ReCaptchaHelper.Validate(Request.Form["g-Recaptcha-Response"]))
					{
						//yes this is depreciated, but there are no decent alternatives for this simple use-case.
						//please make one.
						if (FormsAuthentication.Authenticate(currentPage.Username, currentPage.Password))
						{
							FormsAuthentication.RedirectFromLoginPage(currentPage.Username, currentPage.RememberMe);
						}
					}
					else
					{
						ModelState.AddModelError("Login", "Beep Boop - Robot test failed.");
					}
				}
			}

			//forget the entered password on retry
			currentPage.Password = string.Empty;

			//model not valid
			return View("Login", currentPage);
		}

		public ActionResult Logout()
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			FormsAuthentication.SignOut();

			return RedirectToAction("Index");
		}

		[Authorize]
		public ActionResult Settings(SettingsPageViewModel currentPage)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			var settings = StorageHelper.ReadSettings();

			currentPage.BlogName = settings.BlogName;
			currentPage.AuthorName = settings.AuthorName;
			currentPage.TagLine = settings.TagLine;
			currentPage.DefaultWindowTitle = settings.DefaultWindowTitle;
			currentPage.DefaultMetaDescription = settings.DefaultMetaDescription;

			return View("Settings", currentPage);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult SubmitSettings(SettingsPageViewModel currentPage)
		{
			Response.Cache.SetCacheability(HttpCacheability.NoCache);

			StorageHelper.SaveSettings(new Settings
			{
				BlogName = currentPage.BlogName,
                AuthorName = currentPage.AuthorName,
                TagLine = currentPage.TagLine,
				DefaultWindowTitle = currentPage.DefaultWindowTitle,
				DefaultMetaDescription = currentPage.DefaultMetaDescription
			});

			return RedirectToAction("Settings");
		}
	}
}