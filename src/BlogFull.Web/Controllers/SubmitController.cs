using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using BlogFull.Web.Business.Helpers;
using BlogFull.Web.Controllers.Base;
using BlogFull.Web.Models.Data;
using BlogFull.Web.Models.ViewModels;
using CommonMark;

namespace BlogFull.Web.Controllers
{
	public class SubmitController : BaseController
	{
		public ActionResult Index(SubmitPageViewModel currentPage)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

			return View("Submit", currentPage);
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		[Authorize]
		public ActionResult Submit(SubmitPageViewModel currentPage)
		{
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

			if (ModelState.IsValid)
			{
				var now = DateTime.Now;
				var slug = SlugHelper.CreateSlug(now.Year, currentPage.Slug);

				//validate the slug, null means slug exists
				if (string.IsNullOrWhiteSpace(slug))
				{
					ModelState.AddModelError("Slug", "The slug is already in use.");
					currentPage.Slug = string.Empty;
					return View("Submit", currentPage);
				}

				StorageHelper.SavePost(new Post
				{
					BodyHtml = CommonMarkConverter.Convert(currentPage.BodyText),
					BodyMarkdown = currentPage.BodyText,
					DontIndexPost = currentPage.DontIndexNewPost,
					MetaDescPost = currentPage.MetaDescNewPost,
					PostedTime = now,
					Slug = slug,
					Title = currentPage.Title,
					ReadTime = PostHelper.GetReadTime(currentPage.BodyText),
					Categories = currentPage.Categories
				});

				return RedirectToAction("Index", "Post", new { year = now.Year, slug });
			}

			//not valid
			return View("Submit", currentPage);
		}

		[Authorize]
		public ActionResult Edit(EditPostPageViewModel currentPage, int year, string slug)
		{
			//auth only
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

			//redirect on no slug
			if (string.IsNullOrWhiteSpace(slug))
			{
				return RedirectToAction("Index", "Blog");
			}

			//try find the slug directly
			var originalPost = StorageHelper.ReadPost(year, slug, true);
			if (originalPost != null)
			{
				currentPage.Slug = originalPost.Slug;
				currentPage.Title = originalPost.Title;
				currentPage.PostedTime = originalPost.PostedTime;
				currentPage.BodyMarkdown = originalPost.BodyMarkdown;
				currentPage.ReadTime = originalPost.ReadTime;
				currentPage.Categories = originalPost.Categories;
				currentPage.DontIndexPost = originalPost.DontIndexPost;
				currentPage.MetaDescPost = originalPost.MetaDescPost;

				return View("Edit", currentPage);
			}

			return HttpNotFound();
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		[Authorize]
		public ActionResult SubmitEdit(EditPostPageViewModel currentPage)
		{
			//make sure we can do it
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

			if (ModelState.IsValid)
			{
				//get the original from file with markdown for image parsing
				var originalPost = StorageHelper.ReadPost(currentPage.PostedTime.Year, currentPage.Slug, true);

				if (originalPost != null)
				{
                    //first we delete abandoned images
                    PostHelper.ProcessRemovedImages(originalPost.BodyMarkdown, currentPage.BodyMarkdown);

					//NOTICE the html markdown switcharoo
					//update with the fields we allow changing
					originalPost.BodyHtml = CommonMarkConverter.Convert(currentPage.BodyMarkdown);
					originalPost.BodyMarkdown = currentPage.BodyMarkdown;
					originalPost.DontIndexPost = currentPage.DontIndexPost;
					originalPost.MetaDescPost = currentPage.MetaDescPost;
					originalPost.Title = currentPage.Title;
					originalPost.Categories = currentPage.Categories;

                    //PS get the new read time after the new body has been populated
				    originalPost.ReadTime = PostHelper.GetReadTime(ref originalPost);

                    //save the post (will update the cache as well)
                    StorageHelper.SavePost(originalPost);

					return RedirectToAction("Index", "Post", new { year = originalPost.PostedTime.Year, slug = originalPost.Slug });
				}
			}

			//not valid, return
			return View("Edit", currentPage);
		}

		[Authorize]
		public ActionResult DeletePost(int year, string slug)
		{
			//should you though?
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Admin");
			}

            //read the old post first to remove images
		    var originalPost = StorageHelper.ReadPost(year, slug, true);
		    if (originalPost != null)
		    {
		        PostHelper.ProcessRemovedImages(originalPost.BodyMarkdown, "");
            }

            //now delete the post
            if (StorageHelper.DeletePost(year, slug))
			{
				return RedirectToAction("Index", "Blog");
			}

			return HttpNotFound();
		}
		
		[HttpPost]
		public JsonResult Upload()
		{
			if (User.Identity.IsAuthenticated)
			{
				if (Request.Files.Count > 0)
				{
					var file = Request.Files[0];

					if (file != null && file.ContentLength > 0)
					{
						var newFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
						var path = Path.Combine(Server.MapPath("~/post-images/"), newFileName);
						file.SaveAs(path);

						return new JsonResult{ Data = newFileName};
					}
				}
			}

			//something went wrong, session probably died
			//NOTE: We are manually returning 401 instead of using [Authorize] which redirects to the login page
			//		which wont work with an api call.
			Response.StatusCode = (int) HttpStatusCode.Unauthorized;
			HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

			return Json( new { ErrorMessage = "You need to be logged in to upload images." } );;
		}
	}
}