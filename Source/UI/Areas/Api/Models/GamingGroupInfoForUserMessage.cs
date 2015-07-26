using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class GamingGroupInfoForUserMessage
    {
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string GamingGroupPublicUrl { get; set; }
        public string GamingGroupPublicDescription { get; set; }
    }
}
