using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.Players
{
    public class PlayerEditViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public bool Active { get; set; }
        public int GamingGroupId { get; set; }
    }
}