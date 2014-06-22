using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.PlayedGame;

namespace UI.Transformations.Player
{
    public interface IndividualPlayerGameSummaryViewModelBuilder
    {
        IndividualPlayerGameSummaryViewModel Build(PlayerGameResult playerGameResult);
    }
}