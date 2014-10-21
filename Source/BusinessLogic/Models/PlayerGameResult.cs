using BusinessLogic.DataAccess;
using System.Linq;

namespace BusinessLogic.Models
{
    public class PlayerGameResult : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }

        public int PlayedGameId { get; set; }
        public int PlayerId { get; set; }
        public int GameRank { get; set; }
        public int GordonPoints { get; set; }

        public virtual PlayedGame PlayedGame { get; set; }
        public virtual Player Player { get; set; }
    }
}
