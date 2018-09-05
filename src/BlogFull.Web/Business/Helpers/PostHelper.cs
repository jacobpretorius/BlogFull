using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BlogFull.Web.Models.Data;

namespace BlogFull.Web.Business.Helpers
{
	public static class PostHelper
	{
		private static readonly int WordsPerMinute = 200;

        /// <summary>
        /// Estimate the time to read a blog post at 200wpm
        /// </summary>
		public static string GetReadTime(ref string post)
		{
			//get the amount of words
			var words = post.Count(Char.IsWhiteSpace);

			//guess that people read 200 words per minute
			var timeToRead = words / WordsPerMinute;

			return timeToRead < 1 ? "1" : timeToRead.ToString();
		}

	    /// <summary>
	    /// Estimate the time to read a blog post at 200wpm
	    /// </summary>
		public static string GetReadTime(ref Post post)
		{
			//get the amount of words
			var words = post.BodyMarkdown.Count(Char.IsWhiteSpace);

			//guess that people read 200 words per minute
			var timeToRead = words / WordsPerMinute;

			return timeToRead < 1 ? "1" : timeToRead.ToString();
		}

	    /// <summary>
	    /// Estimate the time to read a blog post at 200wpm
	    /// </summary>
		public static string GetReadTime(string post)
		{
			//get the amount of words
			var words = post.Count(Char.IsWhiteSpace);

			//guess that people read 200 words per minute
			var timeToRead = words / WordsPerMinute;

			return timeToRead < 1 ? "1" : timeToRead.ToString();
		}

        /// <summary>
        /// Compare two versions of a post looking for image embed markdown, and then delete all images no longer in use.
        /// </summary>
        /// <param name="oldBody">The old post body markdown</param>
        /// <param name="newBody">The new post body markdown</param>
	    public static void ProcessRemovedImages(string oldBody, string newBody)
	    {
            //get all old links
            var allOldLinks = new Regex(@"!\[.*?\]\(.*?\)").Matches(oldBody).Cast<Match>().Select(m => m.Value).ToList();

            //check that there is something to remove
	        if (allOldLinks.Count > 0)
	        {
                //parse all old links to get the guids
	            var oldLinkGuidList = new List<string>();
	            foreach (var oldLink in allOldLinks)
	            {
	                //make sure the link is for this server
	                if (oldLink.Contains("/post-images/"))
	                {
                        //build a list of all old images
	                    oldLinkGuidList.Add(oldLink.Substring(oldLink.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase)));
	                }
	            }

                //get all new links
                var allNewLinks = new Regex(@"!\[.*?\]\(.*?\)").Matches(newBody).Cast<Match>().Select(m => m.Value).ToList();
	            foreach (var newLink in allNewLinks)
	            {
	                //make sure the link is for this server
	                if (newLink.Contains("/post-images/"))
	                {
                        //remove all images still in use from the list
	                    oldLinkGuidList.Remove(newLink.Substring(newLink.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase)));
	                }
	            }

                //once we are done adding and removing to the list, delete all that remain from disk
	            if (oldLinkGuidList.Count > 0)
	            {
	                foreach (var image in oldLinkGuidList)
	                {
                        //delete the image file
	                    StorageHelper.DeleteImage(image.Replace("/", "").Replace(")", ""));
	                }
	            }
            }
        }
	}
}