using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class PlayedGameApplicationLinkage : EntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }

        [Index("IX_PLAYEDGAMEID_APPLICATIONID_ENTITYID", 1, IsUnique = true)]
        [Required]
        public int PlayedGameId { get; set; }

        [Index("IX_PLAYEDGAMEID_APPLICATIONID_ENTITYID", 2, IsUnique = true)]
        [Required]
        public string ApplicationName { get; set; }

        [Index("IX_PLAYEDGAMEID_APPLICATIONID_ENTITYID", 3, IsUnique = true)]
        [Required]
        public string EntityId { get; set; }

        public virtual PlayedGame PlayedGame { get; set; }
    }
}
