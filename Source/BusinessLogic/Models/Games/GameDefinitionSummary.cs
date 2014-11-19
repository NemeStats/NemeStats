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
        public string GamingGroupName { get; set; }
        internal GameDefinition GameDefinition { get; set; }
        public Uri BoardGameGeekUri { get; set; }
    }
}
