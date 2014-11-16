using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BusinessLogic.Models
{
    public class GamingGroupInvitation : SecuredEntityWithTechnicalKey<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id { get; set; }
        public override int GamingGroupId { get; set; }
        [StringLength(255)]
        public string InviteeEmail { get; set; }
        public string InvitingUserId { get; set; }
        public DateTime DateSent { get; set; }
        public string RegisteredUserId { get; set; }
        public DateTime? DateRegistered { get; set; }
        public int? PlayerId { get; set; }
        [ForeignKey("InvitingUserId")]
        public virtual ApplicationUser InvitingUser { get; set; }
        [ForeignKey("RegisteredUserId")]
        public virtual ApplicationUser RegisteredUser { get; set; }
        [ForeignKey("PlayerId")]
        public virtual Player RegisteredPlayer { get; set; }
    }
}
