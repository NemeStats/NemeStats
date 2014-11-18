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
        [Display(Name = "Email Subject")]
        public string EmailSubject { get; set; }
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Display(Name = "Email Body")]
        public string EmailBody { get; set; }
        public string PlayerName { get; set; }
    }
}
