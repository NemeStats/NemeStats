using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Points;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameCreator : IPlayedGameCreator
    {
        private readonly IDataContext dataContext;
        private readonly INemeStatsEventTracker playedGameTracker;
        private readonly INemesisRecalculator nemesisRecalculator;

        public PlayedGameCreator(
            IDataContext applicationDataContext, 
            INemeStatsEventTracker playedGameTracker, 
            IPlayerRepository playerRepository,
            INemesisRecalculator nemesisRecalculator)
        {
            this.dataContext = applicationDataContext;
            this.playedGameTracker = playedGameTracker;
            this.nemesisRecalculator = nemesisRecalculator;
        }

        //TODO need to have validation logic here (or on PlayedGame similar to what is on NewlyCompletedGame)
        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser)
        {
            List<PlayerGameResult> playerGameResults = TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(newlyCompletedGame);

            PlayedGame playedGame = TransformNewlyCompletedGameIntoPlayedGame(
                newlyCompletedGame,
                //TODO should throw some kind of exception if GamingGroupId is null
                currentUser.CurrentGamingGroupId.Value,
                playerGameResults);

            dataContext.Save(playedGame, currentUser);

            GameDefinition gameDefinition = dataContext.FindById<GameDefinition>(newlyCompletedGame.GameDefinitionId);
            playedGameTracker.TrackPlayedGame(currentUser, gameDefinition.Name, playedGame.PlayerGameResults.Count);

            foreach(PlayerGameResult result in playerGameResults)
            {
                nemesisRecalculator.RecalculateNemesis(result.PlayerId, currentUser);
            }

            return playedGame;
        }

        //TODO this should be out of the repository
        internal virtual List<PlayerGameResult> TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(NewlyCompletedGame newlyCompletedGame)
        {
            int numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();
            var playerGameResults = newlyCompletedGame.PlayerRanks
                                        .Select(playerRank => new PlayerGameResult()
                                        {
                                            PlayerId = playerRank.PlayerId.Value,
                                            GameRank = playerRank.GameRank.Value,
                                            //TODO maybe too functional in style? Is there a better way?
                                            GordonPoints = GordonPoints.CalculateGordonPoints(numberOfPlayers, playerRank.GameRank.Value)
                                        })
                                        .ToList();
            return playerGameResults;
        }

        //TODO this should be in its own class
        internal virtual PlayedGame TransformNewlyCompletedGameIntoPlayedGame(
            NewlyCompletedGame newlyCompletedGame,
            int gamingGroupId,
            List<PlayerGameResult> playerGameResults)
        {
            int numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();
            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinitionId = newlyCompletedGame.GameDefinitionId.Value,
                NumberOfPlayers = numberOfPlayers,
                PlayerGameResults = playerGameResults,
                DatePlayed = newlyCompletedGame.DatePlayed,
                GamingGroupId = gamingGroupId
            };
            return playedGame;
        }
    }
}
