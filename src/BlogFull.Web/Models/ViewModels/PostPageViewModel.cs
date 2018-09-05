using System;
using System.Web.Mvc;
using BlogFull.Web.Models.ViewModels.Base;

namespace BlogFull.Web.Models.ViewModels
{
    public class PostPageViewModel : BasePageViewModel
    {
        public string Slug { get; set; }

        public string Title { get; set; }

        public DateTime PostedTime { get; set; }

        [AllowHtml]
        public string BodyHtml { get; set; }

        public string ReadTime { get; set; }

        public string Categories { get; set; }

        public string AuthorName { get; set; }
    }
}