#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameCreator : IPlayedGameCreator
    {
        private readonly IDataContext dataContext;
        private readonly INemeStatsEventTracker playedGameTracker;
        private readonly INemesisRecalculator nemesisRecalculator;
        private readonly IChampionRecalculator championRecalculator;
        private readonly ISecuredEntityValidator<Player> securedEntityValidatorForPlayer;
        private readonly ISecuredEntityValidator<GameDefinition> securedEntityValidatorForGameDefinition;
        private readonly IPointsCalculator pointsCalculator;

        public PlayedGameCreator(
            IDataContext applicationDataContext,
            INemeStatsEventTracker playedGameTracker,
            INemesisRecalculator nemesisRecalculator,
            IChampionRecalculator championRecalculator, 
            ISecuredEntityValidator<Player> securedEntityValidatorForPlayer, 
            ISecuredEntityValidator<GameDefinition> securedEntityValidatorForGameDefinition, 
            IPointsCalculator pointsCalculator)
        {
            dataContext = applicationDataContext;
            this.playedGameTracker = playedGameTracker;
            this.nemesisRecalculator = nemesisRecalculator;
            this.championRecalculator = championRecalculator;
            this.securedEntityValidatorForPlayer = securedEntityValidatorForPlayer;
            this.securedEntityValidatorForGameDefinition = securedEntityValidatorForGameDefinition;
            this.pointsCalculator = pointsCalculator;
        }

        //TODO need to have validation logic here (or on PlayedGame similar to what is on NewlyCompletedGame)
        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, TransactionSource transactionSource, ApplicationUser currentUser)
        {
            var gameDefinition = dataContext.FindById<GameDefinition>(newlyCompletedGame.GameDefinitionId);
            securedEntityValidatorForGameDefinition.ValidateAccess(gameDefinition, currentUser, typeof(GameDefinition), newlyCompletedGame.GameDefinitionId);
            BoardGameGeekGameDefinition boardGameGeekGameDefinition = null;
            if (gameDefinition.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekGameDefinition = dataContext.FindById<BoardGameGeekGameDefinition>(gameDefinition.BoardGameGeekGameDefinitionId);
            }

            ValidateAccessToPlayers(newlyCompletedGame, currentUser);

            var playerGameResults = TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(
                newlyCompletedGame,
                boardGameGeekGameDefinition);

            var playedGame = TransformNewlyCompletedGameIntoPlayedGame(
                newlyCompletedGame,
                newlyCompletedGame.GamingGroupId ?? currentUser.CurrentGamingGroupId,
                currentUser.Id,
                playerGameResults);

            dataContext.Save(playedGame, currentUser);

            playedGameTracker.TrackPlayedGame(currentUser, transactionSource);

            foreach (var result in playerGameResults)
            {
                nemesisRecalculator.RecalculateNemesis(result.PlayerId, currentUser);
            }
            championRecalculator.RecalculateChampion(playedGame.GameDefinitionId, currentUser, false);

            return playedGame;
        }

        private void ValidateAccessToPlayers(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser)
        {
            foreach (var playerRank in newlyCompletedGame.PlayerRanks)
            {
                var player = this.dataContext.FindById<Player>(playerRank.PlayerId);
                securedEntityValidatorForPlayer.ValidateAccess(player, currentUser, typeof(Player), player.Id);
            }
        }

        internal virtual List<PlayerGameResult> TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(
            NewlyCompletedGame newlyCompletedGame,
            BoardGameGeekGameDefinition bggGameDefinition)
        {
            var pointsDictionary = pointsCalculator.CalculatePoints(newlyCompletedGame.PlayerRanks, bggGameDefinition);

            var playerGameResults = newlyCompletedGame.PlayerRanks
                                        .Select(playerRank =>
                                        {
                                            var pointsScorecard = pointsDictionary[playerRank.PlayerId];
                                            return new PlayerGameResult
                                            {
                                                PlayerId = playerRank.PlayerId,
                                                GameRank = playerRank.GameRank,
                                                NemeStatsPointsAwarded = pointsScorecard.BasePoints,
                                                GameDurationBonusPoints = pointsScorecard.GameDurationBonusPoints,
                                                GameWeightBonusPoints = pointsScorecard.GameWeightBonusPoints,
                                                PointsScored = playerRank.PointsScored
                                            };
                                        })
                                        .ToList();
            return playerGameResults;
        }

        //TODO this should be in its own class
        internal virtual PlayedGame TransformNewlyCompletedGameIntoPlayedGame(
            NewlyCompletedGame newlyCompletedGame,
            int gamingGroupId,
            string applicationUserId,
            List<PlayerGameResult> playerGameResults)
        {
            var numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();
            var playedGame = new PlayedGame()
            {
                GameDefinitionId = newlyCompletedGame.GameDefinitionId.Value,
                NumberOfPlayers = numberOfPlayers,
                PlayerGameResults = playerGameResults,
                DatePlayed = newlyCompletedGame.DatePlayed,
                GamingGroupId = gamingGroupId,
                Notes = newlyCompletedGame.Notes,
                CreatedByApplicationUserId = applicationUserId
            };
            return playedGame;
        }
    }
}
