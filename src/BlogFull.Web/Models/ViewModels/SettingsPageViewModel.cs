using System.ComponentModel.DataAnnotations;
using BlogFull.Web.Models.ViewModels.Base;

namespace BlogFull.Web.Models.ViewModels
{
    public class SettingsPageViewModel : BasePageViewModel
    {
        [Required(ErrorMessage = "*Required")]
        public string BlogName { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string AuthorName { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string TagLine { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string DefaultWindowTitle { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string DefaultMetaDescription { get; set; }
    }
}