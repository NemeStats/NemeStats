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
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Events;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Security;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameSaver : BusinessLogicEventSender, IPlayedGameSaver
    {
        private readonly IDataContext _dataContext;
        private readonly ILinkedPlayedGameValidator _linkedPlayedGameValidator;
        private readonly INemeStatsEventTracker _playedGameTracker;
        private readonly INemesisRecalculator _nemesisRecalculator;
        private readonly IChampionRecalculator _championRecalculator;
        private readonly ISecuredEntityValidator<Player> _securedEntityValidatorForPlayer;
        private readonly ISecuredEntityValidator<GameDefinition> _securedEntityValidatorForGameDefinition;
        private readonly IPointsCalculator _pointsCalculator;
        private readonly IApplicationLinker _applicationLinker;

        public PlayedGameSaver(
            IDataContext applicationDataContext,
            INemeStatsEventTracker playedGameTracker,
            INemesisRecalculator nemesisRecalculator,
            IChampionRecalculator championRecalculator,
            ISecuredEntityValidator<Player> securedEntityValidatorForPlayer,
            ISecuredEntityValidator<GameDefinition> securedEntityValidatorForGameDefinition,
            IPointsCalculator pointsCalculator,
            IBusinessLogicEventBus eventBus, 
            ILinkedPlayedGameValidator linkedPlayedGameValidator, IApplicationLinker applicationLinker) : base(eventBus)
        {
            _dataContext = applicationDataContext;
            _playedGameTracker = playedGameTracker;
            _nemesisRecalculator = nemesisRecalculator;
            _championRecalculator = championRecalculator;
            _securedEntityValidatorForPlayer = securedEntityValidatorForPlayer;
            _securedEntityValidatorForGameDefinition = securedEntityValidatorForGameDefinition;
            _pointsCalculator = pointsCalculator;
            _linkedPlayedGameValidator = linkedPlayedGameValidator;
            _applicationLinker = applicationLinker;
        }

        //TODO need to have validation logic here (or on PlayedGame similar to what is on NewlyCompletedGame)
        public PlayedGame CreatePlayedGame(NewlyCompletedGame newlyCompletedGame, TransactionSource transactionSource, ApplicationUser currentUser)
        {
            var gameDefinition = ValidateAccessToGameDefinition(newlyCompletedGame.GameDefinitionId, currentUser);

            _linkedPlayedGameValidator.Validate(newlyCompletedGame);

            int gamingGroupId = newlyCompletedGame.GamingGroupId ?? currentUser.CurrentGamingGroupId;
            BoardGameGeekGameDefinition boardGameGeekGameDefinition = null;
            if (gameDefinition.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekGameDefinition = _dataContext.FindById<BoardGameGeekGameDefinition>(gameDefinition.BoardGameGeekGameDefinitionId);
            }

            ValidateAccessToPlayers(newlyCompletedGame.PlayerRanks, gamingGroupId, currentUser);

            var playerGameResults = TransformNewlyCompletedGamePlayerRanksToPlayerGameResults(
                newlyCompletedGame,
                boardGameGeekGameDefinition);

            var playedGame = TransformNewlyCompletedGameIntoPlayedGame(
                newlyCompletedGame,
                gamingGroupId,
                currentUser.Id,
                playerGameResults);

            playedGame = _dataContext.Save(playedGame, currentUser);

            _applicationLinker.LinkApplication(playedGame.Id, ApplicationLinker.APPLICATION_NAME_NEMESTATS, playedGame.Id.ToString());
            foreach (var applicationLinkage in newlyCompletedGame.ApplicationLinkages)
            {
                _applicationLinker.LinkApplication(playedGame.Id, applicationLinkage.ApplicationName, applicationLinkage.EntityId);
            }

            _playedGameTracker.TrackPlayedGame(currentUser, transactionSource);

            foreach (var result in playerGameResults)
            {
                _nemesisRecalculator.RecalculateNemesis(result.PlayerId, currentUser);
            }
            _championRecalculator.RecalculateChampion(playedGame.GameDefinitionId, currentUser, false);

            SendEvents(new IBusinessLogicEvent[] { new PlayedGameCreatedEvent() { TriggerEntityId = playedGame.Id } });

            return playedGame;
        }

        internal virtual GameDefinition ValidateAccessToGameDefinition(int gameDefinitionId, ApplicationUser currentUser)
        {
            var gameDefinition = _dataContext.FindById<GameDefinition>(gameDefinitionId);
            _securedEntityValidatorForGameDefinition.ValidateAccess(gameDefinition, currentUser, gameDefinitionId);
            return gameDefinition;
        }

        private void ValidateAccessToPlayers(IEnumerable<PlayerRank> playerRanks, int gamingGroupId, ApplicationUser currentUser)
        {
            foreach (var playerRank in playerRanks)
            {
                var player = _dataContext.FindById<Player>(playerRank.PlayerId);
                if (player.GamingGroupId != gamingGroupId)
                {
                    throw new PlayerNotInGamingGroupException(player.Id, gamingGroupId);
                }
                _securedEntityValidatorForPlayer.ValidateAccess(player, currentUser, player.Id);
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

            var winnerType = WinnerTypes.PlayerWin;

            if (newlyCompletedGame.WinnerType.HasValue)
            {
                winnerType = newlyCompletedGame.WinnerType.Value;
            }
            else
            {
                if (playerGameResults.All(x => x.GameRank == 1))
                {
                    winnerType = WinnerTypes.TeamWin;
                }
                else if (playerGameResults.All(x => x.GameRank > 1))
                {
                    winnerType = WinnerTypes.TeamLoss;
                }
            }

            var numberOfPlayers = newlyCompletedGame.PlayerRanks.Count;
            var playedGame = new PlayedGame
            {
                GameDefinitionId = newlyCompletedGame.GameDefinitionId,
                NumberOfPlayers = numberOfPlayers,
                WinnerType = winnerType,
                PlayerGameResults = playerGameResults,
                DatePlayed = newlyCompletedGame.DatePlayed,
                GamingGroupId = gamingGroupId,
                Notes = newlyCompletedGame.Notes,
                CreatedByApplicationUserId = applicationUserId
            };
            return playedGame;
        }

        public PlayedGame UpdatePlayedGame(UpdatedGame updatedGame, TransactionSource transactionSource, ApplicationUser currentUser)
        {
            var playedGameWithStuff = _dataContext.GetQueryable<PlayedGame>()
                .Where(x => x.Id == updatedGame.PlayedGameId)
                .Include(x => x.ApplicationLinkages)
                .Include(x => x.GameDefinition)
                .Include(x => x.PlayerGameResults)
                .FirstOrDefault();

            if (playedGameWithStuff == null)
            {
                throw new EntityDoesNotExistException(typeof(PlayedGame), updatedGame.PlayedGameId);
            }

            ValidateAccessToGameDefinition(updatedGame.GameDefinitionId, currentUser);

            return null;
        }
    }
}
