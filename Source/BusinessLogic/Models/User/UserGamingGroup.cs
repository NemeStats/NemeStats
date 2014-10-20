using System.Linq;

namespace BusinessLogic.Models.User
{
    public class UserGamingGroup
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        public int GamingGroupId { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
