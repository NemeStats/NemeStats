using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class GamingGroupInvitation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int GamingGroupId { get; set; }
        [StringLength(255)]
        public string InviteeEmail { get; set; }
        public string InvitingUserId { get; set; }
        public DateTime DateSent { get; set; }
        public string RegisteredUserId { get; set; }
        public DateTime? DateRegistered { get; set; }
        [ForeignKey("InvitingUserId")]
        public virtual ApplicationUser InvitingUser { get; set; }
        [ForeignKey("RegisteredUserId")]
        public virtual ApplicationUser RegisteredUser { get; set; }
    }
}
