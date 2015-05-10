using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class UpdateGameDefinitionMessage
    {
        public string GameDefinitionName { get; set; }
        public bool? Active { get; set; }
        public int? BoardGameGeekObjectId { get; set; }
    }
}
