using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.Players
{
    public class GameDefinitionTotals
    {
        public GameDefinitionTotals()
        {
            SummariesOfGameDefinitionTotals = new List<GameDefinitionTotal>();
        }
        public IList<GameDefinitionTotal> SummariesOfGameDefinitionTotals { get; set; }
    }
}
