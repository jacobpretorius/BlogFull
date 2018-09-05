using System;
using System.Web.Mvc;

namespace BlogFull.Web.Models.Data
{
    public class Post
    {
        public bool DontIndexPost { get; set; }

        public string MetaDescPost { get; set; }

        public string Slug { get; set; }

        public string Title { get; set; }

        public DateTime PostedTime { get; set; }

        [AllowHtml]
        public string BodyHtml { get; set; }

		[AllowHtml]
        public string BodyMarkdown { get; set; }

		public string ReadTime { get; set; }

		public string Categories { get; set; }
    }
}