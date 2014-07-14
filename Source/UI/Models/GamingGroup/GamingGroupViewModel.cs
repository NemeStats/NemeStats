using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.User;

namespace UI.Models.GamingGroup
{
    public class GamingGroupViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwningUserId { get; set; }
        public string OwningUserName { get; set; }
    }
}