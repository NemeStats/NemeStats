using System;
using System.Collections.Generic;

namespace UI.Models.Players
{
    public class PlayerVersusPlayersViewModel
    {
        public IEnumerable<OpposingPlayerViewModel> OpposingPlayers { get; set; }
    }
}