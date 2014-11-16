using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models.User;

namespace BusinessLogic.Models.Players
{
    public class PlayerInvitation
    {
        public int InvitedPlayerId { get; set; }
        public string InvitedPlayerEmail { get; set; }
        public string EmailSubject { get; set; }
        public string CustomEmailMessage { get; set; }
    }
}
