using System.Collections.Generic;
using BusinessLogic.DataAccess;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Models
{
    public class BoardGameGeekGameDefinition : EntityWithTechnicalKey<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public virtual IList<GameDefinition> GameDefinitions { get; set; }
        public int? MaxPlayers { get; set; }
        public int? MinPlayers { get; set; }
        public int? MaxPlayTime { get; set; }
        public int? MinPlayTime { get; set; }
        public decimal? AverageWeight { get; set; }
        public string Description { get; set; }
        public int? YearPublished { get; set; }
        public bool IsExpansion { get; set; }
        public int? Rank { get; set; }
        public virtual IList<BoardGameGeekGameToCategory> Categories { get; set; }
        [NotMapped]
        public int? AveragePlayTime {
            get
            {
                if (!MaxPlayTime.HasValue)
                {
                    return MinPlayTime;
                }
                if (MinPlayTime.HasValue)
                {
                    return (MaxPlayTime.Value + MinPlayTime.Value) / 2;
                }
                return MaxPlayTime;
            }
        }
    }
}
