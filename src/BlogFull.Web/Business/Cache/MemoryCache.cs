using System;
using System.Collections.Generic;
using System.Linq;
using BlogFull.Web.Business.Helpers;
using BlogFull.Web.Models.Data;

namespace BlogFull.Web.Business.Cache
{
    public static class MemoryCache
    {
        private static List<Post> _memoryCache = new List<Post>();

        private static List<string> _categoryCache = new List<string>();

        static MemoryCache()
        {
            //fill the cache with all posts, ordered by storage helper
            _memoryCache = StorageHelper.ReadAllPosts();

            //and fill the categoryCache from that
            foreach (var post in _memoryCache)
            {
                AddCategory(post.Categories);
            }
        }

        /// <summary>
        /// Get a Post by slug from memory and then from disk.
        /// </summary>
        /// <param name="year">the year to search</param>
        /// <param name="slug">The slug to search for</param>
        /// <returns>Post if found in memory or disk, else returns null</returns>
        public static Post GetPost(int year, string slug)
        {
            if (_memoryCache?.Count(w => w?.Slug == slug && w?.PostedTime.Year == year) > 0)
            {
                //cache hit
                return _memoryCache?.FirstOrDefault(w => w.Slug == slug && w.PostedTime.Year == year);
            }

            //cache miss
            var missed = StorageHelper.ReadPost(year, slug);
            if (missed != null)
            {
                _memoryCache?.Add(missed);
                return missed;
            }

            return null;
        }

		/// <summary>
		/// Get the 10 newest posts
		/// </summary>
		/// <param name="pager">Skip in increments of 10</param>
		/// <returns>10 Posts in reverse order</returns>
        public static IEnumerable<Post> GetTenPosts(int pager = 0)
        {
            return _memoryCache?.Skip(pager * 10)?.Take(10);
        }

        /// <summary>
        /// Get the 10 newest posts in a specific category
        /// </summary>
        /// <param name="category">The category to search</param>
        /// <param name="pager">Skip in increments of 10</param>
        /// <returns></returns>
        public static IEnumerable<Post> GetTenPostsFromCategory(string category, int pager = 0)
        {
            var result = new List<Post>();
            foreach (var post in _memoryCache)
            {
                //check for partial matches
                if (post?.Categories != null && post.Categories.Contains(category))
                {
                    //possible lead, check for full matches
                    var thisPostCat = post.Categories.Split(new []{ ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (thisPostCat.Contains(category))
                    {
                        //gotcha
                        result.Add(post);

                        //break out if we have enough
                        if (result.Count >= (10 * (pager + 1)))
                        {
                            result.Sort((p1, p2) => p2.PostedTime.CompareTo(p1.PostedTime));

                            return result?.Skip(pager * 10)?.Take(10);
                        }
                    }
                }
            }

            //if we end up here there arent the full amount of results (so the break if never happened)
            result.Sort((p1, p2) => p2.PostedTime.CompareTo(p1.PostedTime));
            return result?.Skip(pager * 10)?.Take(10);
        }

        /// <summary>
        /// Get a list of all categories we know of
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAllCategories()
        {
            return _categoryCache;
        }

        /// <summary>
        /// Add or Updates the memory Cache with a post
        /// </summary>
        /// <param name="post">The post to add</param>
        public static void AddPost(Post post)
        {
            //check if we have it in the cache
            if (_memoryCache.Count(w => w.Slug == post.Slug && w.PostedTime.Year == post.PostedTime.Year) > 0)
            {
                //we do have it, remove old one first
                _memoryCache.RemoveAll(s => s.Slug == post.Slug && s.PostedTime.Year == post.PostedTime.Year);
            }

            //now add it
            _memoryCache.Add(post);

            //and sort for good measure
            _memoryCache.Sort((p1, p2) => p2.PostedTime.CompareTo(p1.PostedTime));

            //update the category cache
            AddCategory(post.Categories);
        }

        /// <summary>
        /// remove a post from the Cache
        /// </summary>
        /// <param name="year">The year the post was made</param>
        /// <param name="slug">The slug of the post</param>
        public static void RemovePost(int year, string slug)
        {
            //check if we have it in the cache
            if (_memoryCache.Count(w => w.Slug == slug && w.PostedTime.Year == year) > 0)
            {
                //we do have it, remove
                _memoryCache.RemoveAll(s => s.Slug == slug && s.PostedTime.Year == year);
            }
        }

        /// <summary>
        /// Add a category to the cache
        /// </summary>
        /// <param name="categories">A Comma separated list of categories</param>
        private static void AddCategory(string categories)
        {
            foreach (var category in categories.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                //check if we have it in the cache
                if (_categoryCache.Count(w => w == category) == 0)
                {
                    //we do have it, add
                    _categoryCache.Add(category);
                }
            }
        }
    }
}