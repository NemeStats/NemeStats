using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.GameDefinitionModels
{
    public class CreateGameDefinitionViewModel
    {
        public CreateGameDefinitionViewModel()
        {
            Active = true;
        }

        public bool Active { get; internal set; }
        public int? BoardGameGeekGameDefinitionId { get; internal set; }
        public string Description { get; internal set; }
        public string Name { get; internal set; }
    }
}