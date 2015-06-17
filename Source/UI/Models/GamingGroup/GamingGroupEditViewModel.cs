using System.ComponentModel.DataAnnotations;

namespace UI.Models.GamingGroup
{
    public class GamingGroupEditViewModel
    {
        public int GamingGroupId { get; set; }
        public string Website { get; set; }
        [Display(Name = "Public Description")]
        public string PublicDescription { get; set; }
    }
}