using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models.User
{
    public class UserGamingGroup : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }

        [Index("IX_USERID_AND_GAMING_GROUPID", 1, IsUnique = true)]
        public string ApplicationUserId { get; set; }
        [Index("IX_USERID_AND_GAMING_GROUPID", 2, IsUnique = true)]
        public int GamingGroupId { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
