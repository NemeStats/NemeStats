using System.ComponentModel.DataAnnotations;

namespace UI.Models.GamingGroup
{
    public class GamingGroupPublicDetailsViewModel
    {
        public int GamingGroupId { get; set; }
        public string Website { get; set; }
        [Display(Name = "Public Description")]
        public string PublicDescription { get; set; }
        [Display(Name = "Gaming Group Name")]
        public string GamingGroupName { get; set; }
        public bool Active { get; set; }
    }
}