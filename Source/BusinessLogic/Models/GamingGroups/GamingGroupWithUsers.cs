using System.Collections.Generic;
using BusinessLogic.Models.User;

namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupWithUsers
    {
        public bool Active { get; set; }
        public string GamingGroupName { get; set; }
        public string PublicDescription { get; set; }
        public string PublicGamingGroupWebsite { get; set; }

        public List<BasicUserInfo> OtherUsers { get; set; }
        public int GamingGroupId { get; set; }
        public bool UserCanDelete { get; set; }
    }
}