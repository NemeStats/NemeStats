using System.Collections.Generic;
using BusinessLogic.DataAccess;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int? PlayingTime { get; set; }
        public decimal? AverageWeight { get; set; }
        public string Description { get; set; }
    }
}
