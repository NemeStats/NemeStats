using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models.Games
{
    public class BoardGameGeekGameMechanic : EntityWithTechnicalKey<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        public int BoardGameGeekGameMechanicId { get; set; }

        public string MechanicName { get; set; }

        public virtual IList<BoardGameGeekGameDefinition> Games { get; set; }
    }
}
