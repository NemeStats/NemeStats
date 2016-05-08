﻿#region LICENSE
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
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Events;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;

namespace BusinessLogic.Logic.PlayedGames
{

    public class PlayedGameCreator : BusinessLogicEventSender, IPlayedGameCreator
    {
        private readonly IDataContext _dataContext;
        private readonly INemeStatsEventTracker _playedGameTracker;
        private readonly INemesisRecalculator _nemesisRecalculator;
        private readonly IChampionRecalculator _championRecalculator;
        private readonly ISecuredEntityValidator<Player> _securedEntityValidatorForPlayer;
        private readonly ISecuredEntityValidator<GameDefinition> _securedEntityValidatorForGameDefinition;
        private readonly IPointsCalculator _pointsCalculator;

        public PlayedGameCreator(
            IDataContext applicationDataContext,
            INemeStatsEventTracker playedGameTracker,
            INemesisRecalculator nemesisRecalculator,
            IChampionRecalculator championRecalculator,
            ISecuredEntityValidator<Player> securedEntityValidatorForPlayer,
            ISecuredEntityValidator<GameDefinition> securedEntityValidatorForGameDefinition,
            IPointsCalculator pointsCalculator, 
            IBusinessLogicEventBus eventBus) : base(eventBus)
        {
            _dataContext = applicationDataContext;
            this._playedGameTracker = playedGameTracker;
            this._nemesisRecalculator = nemesisRecalculator;
            this._championRecalculator = championRecalculator;
            this._securedEntityValidatorForPlayer = securedEntityValidatorForPlayer;
            this._securedEntityValidatorForGameDefinition = securedEntityValidatorForGameDefinition;
            this._pointsCalculator = pointsCalculator;
        }

        //TODO need to have validation logic here (or on PlayedGame similar to what is on NewlyCompletedGame)
        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, TransactionSource transactionSource, ApplicationUser currentUser)
        {

            var gameDefinition = _dataContext.FindById<GameDefinition>(newlyCompletedGame.GameDefinitionId);
            _securedEntityValidatorForGameDefinition.ValidateAccess(gameDefinition, currentUser, typeof(GameDefinition), newlyCompletedGame.GameDefinitionId);
            BoardGameGeekGameDefinition boardGameGeekGameDefinition = null;
            if (gameDefinition.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekGameDefinition = _dataContext.FindById<BoardGameGeekGameDefinition>(gameDefinition.BoardGameGeekGameDefinitionId);
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

            _dataContext.Save(playedGame, currentUser);

            _playedGameTracker.TrackPlayedGame(currentUser, transactionSource);

            foreach (var result in playerGameResults)
            {
                _nemesisRecalculator.RecalculateNemesis(result.PlayerId, currentUser);
            }
            _championRecalculator.RecalculateChampion(playedGame.GameDefinitionId, currentUser, false);

            this.SendEvents(new IBusinessLogicEvent[] { new PlayedGameCreatedEvent() { TriggerEntityId = playedGame.Id } });

            return playedGame;
        }

        private void ValidateAccessToPlayers(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser)
        {
            foreach (var playerRank in newlyCompletedGame.PlayerRanks)
            {
                var player = this._dataContext.FindById<Player>(playerRank.PlayerId);
                _securedEntityValidatorForPlayer.ValidateAccess(player, currentUser, typeof(Player), player.Id);
            }
        }

        internal virtual List<PlayerGameResult> TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(
            NewlyCompletedGame newlyCompletedGame,
            BoardGameGeekGameDefinition bggGameDefinition)
        {
            var pointsDictionary = _pointsCalculator.CalculatePoints(newlyCompletedGame.PlayerRanks, bggGameDefinition);

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
