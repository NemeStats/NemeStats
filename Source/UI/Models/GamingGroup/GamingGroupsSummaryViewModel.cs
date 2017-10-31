using System.Collections.Generic;

namespace UI.Models.GamingGroup
{
    public class GamingGroupsSummaryViewModel
    {
        public List<GamingGroupSummaryViewModel> GamingGroups { get; set; }
        public bool ShowForEdit { get; set; } = false;
    }
}