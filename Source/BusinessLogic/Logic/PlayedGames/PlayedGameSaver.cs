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

using System;
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
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameSaver : IPlayedGameSaver
    {
        private readonly IDataContext _dataContext;
        private readonly ISecuredEntityValidator _securedEntityValidator;
        private readonly IPointsCalculator _pointsCalculator;
        private readonly IApplicationLinker _applicationLinker;
        private readonly IBusinessLogicEventSender _businessLogicEventSender;
        private readonly IWinnerTypeCalculator _winnerTypeCalculator;

        public PlayedGameSaver(
            IDataContext applicationDataContext,
            ISecuredEntityValidator securedEntityValidator,
            IPointsCalculator pointsCalculator,
            IApplicationLinker applicationLinker, 
            IBusinessLogicEventSender businessLogicEventSender, IWinnerTypeCalculator winnerTypeCalculator)
        {
            _dataContext = applicationDataContext;
            _securedEntityValidator = securedEntityValidator;
            _pointsCalculator = pointsCalculator;
            _applicationLinker = applicationLinker;
            _businessLogicEventSender = businessLogicEventSender;
            _winnerTypeCalculator = winnerTypeCalculator;
        }

        public virtual void ValidateAccessToPlayers(IEnumerable<PlayerRank> playerRanks, int gamingGroupId, ApplicationUser currentUser, IDataContext dataContext)
        {
            foreach (var playerRank in playerRanks)
            {
                var player = dataContext.FindById<Player>(playerRank.PlayerId);
                if (player.GamingGroupId != gamingGroupId)
                {
                    throw new PlayerNotInGamingGroupException(player.Id, gamingGroupId);
                }
                _securedEntityValidator.ValidateAccess(player, currentUser);
            }
        }

        public virtual List<PlayerGameResult> MakePlayerGameResults(
           SaveableGameBase savedGame,
           int? boardGameGeekGameDefinitionId, IDataContext dataContext)
        {
            BoardGameGeekGameDefinition boardGameGeekGameDefinition = null;
            if (boardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekGameDefinition = dataContext.FindById<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId.Value);
            }

            var pointsDictionary = _pointsCalculator.CalculatePoints(savedGame.PlayerRanks, boardGameGeekGameDefinition);

            var playerGameResults = savedGame.PlayerRanks
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

        //TODO this should be in its own class or just in AutoMapperConfiguration
        public virtual PlayedGame TransformNewlyCompletedGameIntoPlayedGame(
            SaveableGameBase savedGame,
            int gamingGroupId,
            string applicationUserId,
            List<PlayerGameResult> playerGameResults)
        {
            var gameRanks = playerGameResults.Select(x => x.GameRank).ToList();
            var winnerType = _winnerTypeCalculator.CalculateWinnerType(gameRanks);

            var numberOfPlayers = savedGame.PlayerRanks.Count;
            var playedGame = new PlayedGame
            {
                GameDefinitionId = savedGame.GameDefinitionId,
                NumberOfPlayers = numberOfPlayers,
                WinnerType = winnerType,
                PlayerGameResults = playerGameResults,
                DatePlayed = savedGame.DatePlayed,
                GamingGroupId = gamingGroupId,
                Notes = savedGame.Notes,
                CreatedByApplicationUserId = applicationUserId
            };
            return playedGame;
        }

        public virtual void CreateApplicationLinkages(IList<ApplicationLinkage> applicationLinkages, int playedGameId, IDataContext dataContext)
        {
            _applicationLinker.LinkApplication(playedGameId, ApplicationLinker.APPLICATION_NAME_NEMESTATS, playedGameId.ToString(), dataContext);
            foreach (var applicationLinkage in applicationLinkages)
            {
                _applicationLinker.LinkApplication(
                    playedGameId, 
                    applicationLinkage.ApplicationName,
                    applicationLinkage.EntityId,
                    dataContext);
            }
        }

        public PlayedGame UpdatePlayedGame(UpdatedGame updatedGame, TransactionSource transactionSource, ApplicationUser currentUser)
        {
            if (updatedGame.GamingGroupId.HasValue)
            {
                _securedEntityValidator.RetrieveAndValidateAccess<GamingGroup>(updatedGame.GamingGroupId.Value, currentUser);
            }
            _securedEntityValidator.RetrieveAndValidateAccess<PlayedGame>(updatedGame.PlayedGameId, currentUser);
            _securedEntityValidator.RetrieveAndValidateAccess<GameDefinition>(updatedGame.GameDefinitionId, currentUser);

            var playedGameWithStuff = _dataContext.GetQueryable<PlayedGame>()
                .Where(x => x.Id == updatedGame.PlayedGameId)
                .Include(x => x.ApplicationLinkages)
                .Include(x => x.GameDefinition)
                .Include(x => x.PlayerGameResults)
                .FirstOrDefault();

            if (playedGameWithStuff == null)
            {
                throw new EntityDoesNotExistException<PlayedGame>(updatedGame.PlayedGameId);
            }

            var gamingGroupId = updatedGame.GamingGroupId ?? playedGameWithStuff.GamingGroupId;

            ValidateAccessToPlayers(updatedGame.PlayerRanks, gamingGroupId, currentUser, _dataContext);
            
            var playerGameResults = MakePlayerGameResults(updatedGame, playedGameWithStuff.GameDefinition?.BoardGameGeekGameDefinitionId, _dataContext);
            var updatedPlayedGame = TransformNewlyCompletedGameIntoPlayedGame(updatedGame, gamingGroupId, currentUser.Id, playerGameResults);

            updatedPlayedGame.Id = updatedGame.PlayedGameId;
            updatedPlayedGame.DateUpdated = DateTime.UtcNow;

            CleanupPlayerResultsAndApplicationLinkages(currentUser, playedGameWithStuff);

            var returnPlayedGame = _dataContext.Save(updatedPlayedGame, currentUser);

            //--Entity Framework doesn't appear to save new child entities when updating the parent entity so we have to save them separately
            playerGameResults.ForEach(x =>
            {
                x.PlayedGameId = returnPlayedGame.Id;
                _dataContext.Save(x, currentUser);
            });

            CreateApplicationLinkages(updatedGame.ApplicationLinkages, updatedGame.PlayedGameId, _dataContext);

            var playerIds = playerGameResults.Select(x => x.PlayerId).ToList();
            _businessLogicEventSender.SendEvent(new PlayedGameCreatedEvent(
                updatedGame.PlayedGameId,
                updatedGame.GameDefinitionId, 
                playerIds, 
                transactionSource,
                currentUser));

            return returnPlayedGame;
        }

        private void CleanupPlayerResultsAndApplicationLinkages(ApplicationUser currentUser, PlayedGame playedGameWithStuff)
        {
            foreach (var playerGameResult in playedGameWithStuff.PlayerGameResults.ToList())
            {
                _dataContext.DeleteById<PlayerGameResult>(playerGameResult.Id, currentUser);
            }

            foreach (var applicationLinkage in playedGameWithStuff.ApplicationLinkages.ToList())
            {
                _dataContext.DeleteById<PlayedGameApplicationLinkage>(applicationLinkage.Id, currentUser);
            }
        }
    }
}
