using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace UI.Models.GamingGroup
{
    public class InvitationViewModel
    {
        [DisplayName("Invitee Email")]
        public string InviteeEmail { get; set; }
        [DisplayName("Registered User Name")]
        public string RegisteredUserName { get; set; }
        [DisplayName("Date Registered")]
        public DateTime? DateRegistered { get; set; }
    }
}