using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UI.Models.GameDefinitionModels
{ 
    public class GameDefinitionEditViewModel
    {
        public int GameDefinitionId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
    }
}