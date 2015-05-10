using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionUpdateRequest
    {
        public int GameDefinitionId { get; set; }
        public bool? Active { get; set; }
        public string Name { get; set; }
        public int? BoardGameGeekObjectId { get; set; }
    }
}
