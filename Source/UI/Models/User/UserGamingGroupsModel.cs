using System.Collections.Generic;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;

namespace UI.Models.User
{
    public class UserGamingGroupsModel
    {
        public IList<GamingGroupListItemModel> GamingGroups { get; set; }
        public GamingGroupListItemModel CurrentGamingGroup { get; set; }
        public ApplicationUser CurrentUser { get; set; }
    }
}