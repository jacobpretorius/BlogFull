using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BlogFull.Web.Models.ViewModels.Base;

namespace BlogFull.Web.Models.ViewModels
{
    public class SubmitPageViewModel : BasePageViewModel
    {
        [Required(ErrorMessage = "*")]
        public string Slug { get; set; }

        [Required(ErrorMessage = "*")]
        public string Title { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "*")]
        public string BodyText { get; set; }

        public string Categories { get; set; }

        //SEO
        public bool DontIndexNewPost { get; set; }

        //also shown on the /blog view as summary
        [AllowHtml]
        [Required(ErrorMessage = "*")]
        public string MetaDescNewPost { get; set; }
    }
}