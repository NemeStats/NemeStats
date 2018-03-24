using System.Linq;

namespace UI.Areas.Api.Models
{
    public class GamingGroupInfoForUserMessage
    {
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string GamingGroupPublicUrl { get; set; }
        public string GamingGroupPublicDescription { get; set; }
        public bool Active { get; set; }
    }
}
