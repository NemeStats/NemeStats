using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.Players;

namespace UI.Transformations.Player
{
    public interface IPlayerDetailsViewModelBuilder
    {
        PlayerDetailsViewModel Build(PlayerDetails playerDetails, ApplicationUser currentUser = null);
    }
}
