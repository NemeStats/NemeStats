using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class UpdatePlayerMessage
    {
        public string PlayerName { get; set; }
        public bool? Active { get; set; }
    }
}
    