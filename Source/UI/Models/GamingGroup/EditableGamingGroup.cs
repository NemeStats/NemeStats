using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.GamingGroup
{
    public class EditableGamingGroup
    {
        public string GamingGroupName { get; set; }
        public string GamingGroupPublicDescription { get; set; }
        public Uri PublicGamingGroupWebsite { get; set; }
    }
}