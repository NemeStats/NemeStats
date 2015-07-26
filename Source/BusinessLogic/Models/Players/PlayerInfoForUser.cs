using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.Players
{
    public class PlayerInfoForUser
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GamingGroupId { get; set; }
    }
}
