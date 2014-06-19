using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Transformations
{
    public class PlayedGameDetailsBuilderImpl : PlayedGameDetailsBuilder
    {
        public PlayedGameDetails Build(BusinessLogic.Models.PlayedGame playedGame)
        {
            PlayedGameDetails summary = new PlayedGameDetails();
            summary.GameDefinitionId = playedGame.GameDefinitionId;

            return summary;
        }
    }
}