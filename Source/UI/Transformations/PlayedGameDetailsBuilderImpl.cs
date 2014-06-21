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
        internal const string EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL = "PlayedGame.GameDefintion cannot be null.";
        internal const string EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL = "PlayedGame.PlayerGameResults cannot be null.";

        private PlayerGameResultDetailsBuilder playerResultBuilder;

        public PlayedGameDetailsBuilderImpl(PlayerGameResultDetailsBuilder playerGameResultBuilder)
        {
            playerResultBuilder = playerGameResultBuilder;
        }

        public PlayedGameDetails Build(PlayedGame playedGame)
        {
            if(playedGame == null)
            {
                throw new ArgumentNullException("playedGame");
            }

            if(playedGame.GameDefinition == null)
            {
                throw new ArgumentException(EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL);
            }

            if (playedGame.PlayerGameResults == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL);
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