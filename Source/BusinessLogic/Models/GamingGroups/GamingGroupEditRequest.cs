using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupEditRequest
    {
        public int GamingGroupId { get; set; }
        [Required]
        public string GamingGroupName { get; set; }
        [Url]
        public string Website { get; set; }
        public string PublicDescription { get; set; }
        public bool Active { get; set; }
    }
}