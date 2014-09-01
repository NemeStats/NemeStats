using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupQuickStart
    {
        public string GamingGroupName { get; set; }
        public List<string> NewPlayerNames { get; set; }
        public List<string> NewGameDefinitionNames { get; set; }
    }
}
