using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UI.Models.User;

namespace UI.Models.GamingGroup
{
    public class GamingGroupPublicDetailsViewModel
    {
        public int GamingGroupId { get; set; }
        public string Website { get; set; }
        [Display(Name = "Public Description")]
        public string PublicDescription { get; set; }
        [Required]
        [Display(Name = "Gaming Group Name")]
        public string GamingGroupName { get; set; }
        public bool Active { get; set; }
        public List<BasicUserInfoViewModel> OtherUsers { get; set; }
        public bool UserCanDelete { get; set; }
    }
}