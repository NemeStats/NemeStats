using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.User;

namespace BusinessLogic.Models
{
    public class PlayedGame : SecuredEntityWithTechnicalKey<int>
    {
        public PlayedGame()
        {
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }

        public override int GamingGroupId { get; set; }
        public int GameDefinitionId { get; set; }
        public int NumberOfPlayers { get; set; }
        public DateTime DatePlayed { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedByApplicationUserId { get; set; }
        public string Notes { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual GameDefinition GameDefinition { get; set; }
        [ForeignKey("CreatedByApplicationUserId")]
        public virtual ApplicationUser CreatedByApplicationUser { get; set; }
        public virtual IList<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
