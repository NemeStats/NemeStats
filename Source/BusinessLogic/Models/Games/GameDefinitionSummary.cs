using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models.Games
{
    [NotMapped]
    public class GameDefinitionSummary : GameDefinition
    {
        public int TotalNumberOfGamesPlayed { get; set; }
    }
}
