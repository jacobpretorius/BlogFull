using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Xml.Linq;
using BlogFull.Web.Business.Cache;
using BlogFull.Web.Models.Data;

namespace BlogFull.Web.Business.Helpers
{
    public static class StorageHelper
    {
        private static readonly string _folder = HostingEnvironment.MapPath("~/posts/");

        static StorageHelper()
        {
            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }

            EnsureFolder(DateTime.UtcNow.Year);

			//also ensure the images folder is there
	        var imageFolder = HostingEnvironment.MapPath("~/post-images/");
			if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
        }


        /// <summary>
        /// Ensure that a folder for posts is made
        /// </summary>
        /// <param name="year"></param>
        private static void EnsureFolder(int year)
        {
            if (!Directory.Exists($"{_folder}{year}/"))
            {
                Directory.CreateDirectory($"{_folder}{year}/");
            }
        }

        /// <summary>
		/// Save a new post to file. Replaces existing Posts.
		/// </summary>
		/// <param name="post">The post to save</param>
		/// <returns>The bool result of the save operation</returns>
        public static bool SavePost(Post post)
        {
            //make sure we have it first
            EnsureFolder(post.PostedTime.Year);

            try
	        {
                //clean up the categories
	            var sb = new StringBuilder();
	            if (!string.IsNullOrWhiteSpace(post.Categories))
	            {
	                var skipFirst = false;
	                foreach(var category in post.Categories.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
	                {
	                    sb.Append((skipFirst ? "," : "") + category.Trim());
	                    skipFirst = true;
	                }
                }

				var filePath = Path.Combine(_folder + post.PostedTime.Year + "/", post.Slug + ".webinfo");

				var savePost = new XDocument(
	                new XElement("post",
	                    new XElement("title", post.Title),
	                    new XElement("slug", post.Slug),
	                    new XElement("published_date", post.PostedTime.ToString("yyyy-MM-dd HH:mm:ss")),
	                    new XElement("excerpt", post.MetaDescPost),
	                    new XElement("read_time", post.ReadTime),
	                    new XElement("content", post.BodyHtml),
	                    new XElement("markdown_content", post.BodyMarkdown),
	                    new XElement("dont_index", post.DontIndexPost.ToString()),
	                    new XElement("categories", sb.ToString())
                    ));

                //save to file
                savePost.Save(filePath);

				//add to cache (or update if exists)
	            MemoryCache.AddPost(post);

	            return true;
	        }
	        catch
	        {
		        //dont care really, will go into below
	        }

	        return false;
        }

		/// <summary>
		/// Delete a specific post
		/// </summary>
		/// <param name="year">The year of the post</param>
		/// <param name="slug">The slug of the post</param>
		/// <returns></returns>
        public static bool DeletePost(int year, string slug)
        {
            try
            {
                var file = Path.Combine(_folder + year + "/", slug + ".webinfo");
                if (File.Exists(file))
                {
                    //delete the file
                    File.Delete(file);
                }

                //remove from cache
                MemoryCache.RemovePost(year, slug);

                //all good
                return true;
            }
            catch
            {
                //todo log it maybe
            }

            return false;
        }

        /// <summary>
        /// Get a specific slug Post from disk if available.
        /// </summary>
        /// <param name="year">The year to search in</param>
        /// <param name="slug">The slug to search for</param>
        /// <param name="includeMarkdown">Include the plain markdown in the object</param>
        /// <returns>The Post, or Null if none found.</returns>
        public static Post ReadPost(int year, string slug, bool includeMarkdown = false)
		{
            try
            {
                var file = XElement.Load(Path.Combine(_folder + year + "/", slug + ".webinfo"));
                if (file.HasElements)
                {
                    return new Post
                    {
                        Title = XmlHelper.ReadValue(file, "title"),
                        Slug = XmlHelper.ReadValue(file, "slug").ToLowerInvariant(),
                        PostedTime = DateTime.Parse(XmlHelper.ReadValue(file, "published_date")),
                        MetaDescPost = XmlHelper.ReadValue(file, "excerpt"),
                        ReadTime = XmlHelper.ReadValue(file, "read_time"),
                        BodyHtml = XmlHelper.ReadValue(file, "content"),
                        DontIndexPost = XmlHelper.ReadBool(file, "dont_index"),
                        Categories = XmlHelper.ReadValue(file, "categories"),

                        //now we actually get it
                        BodyMarkdown = includeMarkdown ? XmlHelper.ReadValue(file, "markdown_content") : null
                    };
                }
            }
            catch
            {
                //dont actually care, this happens when a post couldnt be read aka 404
            }

            return null;
        }

        /// <summary>
		/// Reads all posts
		/// </summary>
		/// <returns>An ordered list of all Posts</returns>
        public static List<Post> ReadAllPosts()
        {
            var list = new List<Post>();

            // Can this be done in parallel to speed it up?
            foreach (var file in Directory.EnumerateFiles(_folder, "*.webinfo", SearchOption.AllDirectories))
            {
                var doc = XElement.Load(file);
                var post = new Post
                {
                    Title = XmlHelper.ReadValue(doc, "title"),
                    Slug = XmlHelper.ReadValue(doc, "slug").ToLowerInvariant(),
                    PostedTime = DateTime.Parse(XmlHelper.ReadValue(doc, "published_date")),
                    MetaDescPost = XmlHelper.ReadValue(doc, "excerpt"),
					ReadTime = XmlHelper.ReadValue(doc, "read_time"),
                    BodyHtml = XmlHelper.ReadValue(doc, "content"),
                    DontIndexPost = XmlHelper.ReadBool(doc, "dont_index"),
                    Categories = XmlHelper.ReadValue(doc, "categories"),

                    //we dont care for having this in the cache, as edit post reads from disk regardless
                    BodyMarkdown = null
                };

                list.Add(post);
            }

            if (list.Count > 0)
            {
                list.Sort((p1, p2) => p2.PostedTime.CompareTo(p1.PostedTime));
            }

            return list;
        }

        #region Settings

        /// <summary>
        /// Read the setting file on disk and return the parsed object
        /// </summary>
        /// <returns>The settings object</returns>
        public static Settings ReadSettings()
        {
            try
            {
                var file = XElement.Load(Path.Combine(_folder + "settings.config"));
                if (file.HasElements)
                {
                    return new Settings
                    {
                        BlogName = XmlHelper.ReadValue(file, "blog_name"),
                        TagLine = XmlHelper.ReadValue(file, "tag_line"),
                        DefaultWindowTitle = XmlHelper.ReadValue(file, "window_title"),
                        DefaultMetaDescription = XmlHelper.ReadValue(file, "post_meta"),
                        AuthorName = XmlHelper.ReadValue(file, "author_name")
                    };
                }
            }
            catch
            {
                //something wrong with the settings file, recreate it
                SaveSettings();
            }

            return null;
        }

        /// <summary>
        /// Save a settings object to disk and update the settings cache. An empty object means create default settings file.
        /// </summary>
        /// <param name="settings">The settings object to save</param>
        public static void SaveSettings(Settings settings = null)
        {
            //check if we got the obj
            if (settings == null)
            {
                settings = new Settings
                {
                    BlogName = "BlogFull",
                    TagLine = "Built for speed",
                    DefaultMetaDescription = "Powered by https://github.com/jacobpretorius/BlogFull",
                    DefaultWindowTitle = $"BlogFull | {ConfigurationManager.AppSettings["site-url"]}",
                    AuthorName = "BlogFull"
                };
            }

            //save it
            try
            {
                var filePath = Path.Combine(_folder + "settings.config");

                var saveSettings = new XDocument(
                    new XElement("settings",
                        new XElement("blog_name", settings.BlogName),
                        new XElement("tag_line", settings.TagLine),
                        new XElement("window_title", settings.DefaultWindowTitle),
                        new XElement("post_meta", settings.DefaultMetaDescription),
                        new XElement("author_name", settings.AuthorName)
                    ));

                //save to file
                saveSettings.Save(filePath);

                //add to cache (or update if exists)
                SettingsCache.SetSettings(settings);
            }
            catch
            {
                //todo log it
            }
        }

        #endregion

        #region image

        /// <summary>
        /// Delete an image file from post-images on disk
        /// </summary>
        /// <param name="filename">the full filename to delete</param>
        public static void DeleteImage(string filename)
        {
            try
            {
                var file = Path.Combine(HostingEnvironment.MapPath("~/post-images/") + filename);
                if (File.Exists(file))
                {
                    //delete the file
                    File.Delete(file);
                }
            }
            catch
            {
                //todo log it maybe
            }
        }

        #endregion
    }
}