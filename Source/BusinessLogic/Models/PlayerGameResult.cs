using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;
using System.Linq;

namespace BusinessLogic.Models
{
    public class PlayerGameResult : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }

        [Index("IX_PlayerId_and_PlayedGameId", 1, IsUnique = true)]
        public int PlayedGameId { get; set; }
        [Index("IX_PlayerId_and_PlayedGameId", 2, IsUnique = true)]
        public int PlayerId { get; set; }
        public int GameRank { get; set; }
        public int GordonPoints { get; set; }

        public virtual PlayedGame PlayedGame { get; set; }
        public virtual Player Player { get; set; }
    }
}
