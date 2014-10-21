using BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models
{
    public class PlayedGame : SecuredEntityWithTechnicalKey<int>
    {
        public override int Id { get; set; }

        public override int GamingGroupId { get; set; }
        public int GameDefinitionId { get; set; }
        public int NumberOfPlayers { get; set; }
        public DateTime DatePlayed { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual GameDefinition GameDefinition { get; set; }
        public virtual IList<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
