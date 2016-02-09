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
    }
}
