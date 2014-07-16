using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [DisplayName("Owning User Name")]
        public string OwningUserName { get; set; }
        [DataType(DataType.EmailAddress)]
        [DisplayName("Invitee Email")]
        public string InviteeEmail { get; set; }
        public IList<InvitationViewModel> Invitations { get; set; }
    }
}