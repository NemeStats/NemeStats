using System.Collections.Generic;

namespace UI.Models.Players
{
    public class PlayerVersusPlayersViewModel
    {
        public PlayerVersusPlayersViewModel()
        {
            this.OpposingPlayers = new List<OpposingPlayerViewModel>(); 
        }

        public IList<OpposingPlayerViewModel> OpposingPlayers { get; set; }
    }
}