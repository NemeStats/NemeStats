using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace UI.Models.Players
{
    public class PlayerInvitationViewModel
    {
        public int PlayerId { get; set; }
        public string EmailSubject { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        public string EmailBody { get; set; }
        public string PlayerName { get; set; }
    }
}
