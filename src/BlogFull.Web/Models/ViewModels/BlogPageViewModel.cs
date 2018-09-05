using System.Collections.Generic;
using BlogFull.Web.Models.Data;
using BlogFull.Web.Models.ViewModels.Base;

namespace BlogFull.Web.Models.ViewModels
{
    public class BlogPageViewModel : BasePageViewModel
    {
        public IEnumerable<Post> BlogPosts { get; set; }

        public IEnumerable<string> Categories { get; set; }

        public int Page { get; set; }

        public string ActiveCategory { get; set; }
    }
}