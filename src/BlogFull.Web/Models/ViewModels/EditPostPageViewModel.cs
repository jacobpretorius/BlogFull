using System;
using System.Web.Mvc;
using BlogFull.Web.Models.Data;
using BlogFull.Web.Models.ViewModels.Base;

namespace BlogFull.Web.Models.ViewModels
{
    public class EditPostPageViewModel : BasePageViewModel
    {
        public string Slug { get; set; }

        public string Title { get; set; }

        public DateTime PostedTime { get; set; }

        [AllowHtml]
        public string BodyMarkdown { get; set; }

        public string ReadTime { get; set; }

        public string Categories { get; set; }

        public bool DontIndexPost { get; set; }

        public string MetaDescPost { get; set; }
    }
}