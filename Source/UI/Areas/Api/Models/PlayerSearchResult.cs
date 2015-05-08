using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class PlayerSearchResult
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public bool Active { get; set; }
        public int? CurrentNemesisPlayerId { get; set; }
    }
}
