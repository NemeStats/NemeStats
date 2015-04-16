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
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
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
        private readonly IChampionRecalculator championRecalculator;
        private readonly ISecuredEntityValidator<Player> securedEntityValidatorForPlayer;
        private readonly ISecuredEntityValidator<GameDefinition> securedEntityValidatorForGameDefinition; 

        public PlayedGameCreator(
            IDataContext applicationDataContext, 
            INemeStatsEventTracker playedGameTracker, 
            INemesisRecalculator nemesisRecalculator,
            IChampionRecalculator championRecalculator, ISecuredEntityValidator<Player> securedEntityValidatorForPlayer, ISecuredEntityValidator<GameDefinition> securedEntityValidatorForGameDefinition)
        {
            this.dataContext = applicationDataContext;
            this.playedGameTracker = playedGameTracker;
            this.nemesisRecalculator = nemesisRecalculator;
            this.championRecalculator = championRecalculator;
            this.securedEntityValidatorForPlayer = securedEntityValidatorForPlayer;
            this.securedEntityValidatorForGameDefinition = securedEntityValidatorForGameDefinition;
        }

        //TODO need to have validation logic here (or on PlayedGame similar to what is on NewlyCompletedGame)
        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, TransactionSource transactionSource, ApplicationUser currentUser)
        {
            GameDefinition gameDefinition = dataContext.FindById<GameDefinition>(newlyCompletedGame.GameDefinitionId);
            securedEntityValidatorForGameDefinition.ValidateAccess(gameDefinition, currentUser, typeof(GameDefinition), newlyCompletedGame.GameDefinitionId);

            this.ValidateAccessToPlayers(newlyCompletedGame, currentUser);

            List<PlayerGameResult> playerGameResults = TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(newlyCompletedGame);

            PlayedGame playedGame = TransformNewlyCompletedGameIntoPlayedGame(
                newlyCompletedGame,
                //TODO should throw some kind of exception if GamingGroupId is null
                currentUser.CurrentGamingGroupId.Value,
                currentUser.Id,
                playerGameResults);

            dataContext.Save(playedGame, currentUser);

            playedGameTracker.TrackPlayedGame(currentUser, transactionSource);

            foreach(PlayerGameResult result in playerGameResults)
            {
                nemesisRecalculator.RecalculateNemesis(result.PlayerId, currentUser);
            }
            championRecalculator.RecalculateChampion(playedGame.GameDefinitionId, currentUser);

            return playedGame;
        }

        private void ValidateAccessToPlayers(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser)
        {
            foreach (var playerRank in newlyCompletedGame.PlayerRanks)
            {
                Player player = this.dataContext.FindById<Player>(playerRank.PlayerId);
                this.securedEntityValidatorForPlayer.ValidateAccess(player, currentUser, typeof(Player), player.Id);
            }
        }

        internal virtual List<PlayerGameResult> TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(NewlyCompletedGame newlyCompletedGame)
        {
            var pointsDictionary = PointsCalculator.CalculatePoints(newlyCompletedGame.PlayerRanks);

            var playerGameResults = newlyCompletedGame.PlayerRanks
                                        .Select(playerRank => new PlayerGameResult()
                                        {
                                            PlayerId = playerRank.PlayerId,
                                            GameRank = playerRank.GameRank,
                                            GordonPoints = pointsDictionary[playerRank.PlayerId],
                                            PointScored = playerRank.PointsScored
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
            int numberOfPlayers = newlyCompletedGame.PlayerRanks.Count();
            PlayedGame playedGame = new PlayedGame()
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
