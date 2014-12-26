using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models
{
    public class GameDefinition : SecuredEntityWithTechnicalKey<int>
    {
        public GameDefinition()
        {
            Active = true;
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }

        public override int GamingGroupId { get; set; }

        public int? BoardGameGeekObjectId { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual IList<PlayedGame> PlayedGames { get; set; }
        public virtual GamingGroup GamingGroup { get; set; }

        public int? ChampionId { get; set; }
        public int? PreviousChampionId { get; set; }

        public virtual Champion Champion { get; set; }
        public virtual Champion PreviousChampion { get; set; }
    }
}
