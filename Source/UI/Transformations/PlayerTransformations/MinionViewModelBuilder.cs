using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public class MinionViewModelBuilder : IMinionViewModelBuilder
    {
        internal const string EXCEPTION_NEMESIS_CANNOT_BE_NULL = "playerWithNemesis.Nemesis cannot be null.";

        public MinionViewModel Build(Player playerWithNemesis)
        {
            if(playerWithNemesis.Nemesis == null)
            {
                throw new ArgumentException(EXCEPTION_NEMESIS_CANNOT_BE_NULL);
            }

            return new MinionViewModel()
            {
                MinionName = playerWithNemesis.Name,
                MinionPlayerId = playerWithNemesis.Id,
                NumberOfGamesWonVersusMinion = playerWithNemesis.Nemesis.NumberOfGamesLost,
                WinPercentageVersusMinion = (int)playerWithNemesis.Nemesis.LossPercentage
            };
        }
    }
}