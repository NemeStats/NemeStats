using BusinessLogic.Models;
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
        PlayerGameResultDetailsBuilder playerResultBuilder;

        public PlayedGameDetailsBuilderImpl(PlayerGameResultDetailsBuilder playerGameResultBuilder)
        {
            playerResultBuilder = playerGameResultBuilder;
        }

        public PlayedGameDetails Build(PlayedGame playedGame)
        {
            //TODO add validation logic to transformations??
            if(playedGame.GameDefinition == null)
            {
                throw new ArgumentNullException("PlayedGame.GameDefinition");
            }

            if (playedGame.PlayerGameResults == null)
            {
                throw new ArgumentNullException("PlayedGame.PlayerGameResults");
            }
            
            PlayedGameDetails summary = new PlayedGameDetails();
            summary.GameDefinitionName = playedGame.GameDefinition.Name;
            summary.GameDefinitionId = playedGame.GameDefinitionId;
            summary.PlayedGameId = playedGame.Id;
            summary.DatePlayed = playedGame.DatePlayed;
            summary.PlayerResults = new List<PlayerGameResultDetails>();
            
            foreach(PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
            {
                summary.PlayerResults.Add(playerResultBuilder.Build(playerGameResult));
            }

            return summary;
        }
    }
}