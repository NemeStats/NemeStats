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
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.Models.Games;
using BusinessLogic.Logic.BoardGameGeek;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionSaver : IGameDefinitionSaver
    {
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NAME_CANNOT_BE_NULL_OR_WHITESPACE 
            = "gameDefinition.Name cannot be null or whitespace.";

        private readonly IDataContext dataContext;
        private readonly INemeStatsEventTracker eventTracker;
        private readonly IBoardGameGeekApiClient boardGameGeekApiClient;
        private readonly IBoardGameGeekGameDefinitionCreator boardGameGeekGameDefinitionAttacher;

        public GameDefinitionSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker, IBoardGameGeekApiClient boardGameGeekApiClient, IBoardGameGeekGameDefinitionCreator boardGameGeekGameDefinitionAttacher)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
            this.boardGameGeekApiClient = boardGameGeekApiClient;
            this.boardGameGeekGameDefinitionAttacher = boardGameGeekGameDefinitionAttacher;
        }

        public GameDefinition CreateGameDefinition(CreateGameDefinitionRequest createGameDefinitionRequest, ApplicationUser currentUser)
        {
            ValidateNotNull(createGameDefinitionRequest);

            ValidateGameDefinitionNameIsNotNullOrWhitespace(createGameDefinitionRequest.Name);

            int? boardGameGeekGameDefinitionId = CreateBoardGameGeekGameDefinition(createGameDefinitionRequest, currentUser);

            var existingGameDefinition = dataContext.GetQueryable<GameDefinition>()
                .FirstOrDefault(game => game.GamingGroupId == currentUser.CurrentGamingGroupId.Value
                        && game.Name == createGameDefinitionRequest.Name);

            if (existingGameDefinition == null)
            {
                var newGameDefinition = new GameDefinition
                {
                    Name = createGameDefinitionRequest.Name,
                    BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionId,
                    Description = createGameDefinitionRequest.Description,
                    GamingGroupId = currentUser.CurrentGamingGroupId.Value
                };

                new Task(() => this.eventTracker.TrackGameDefinitionCreation(currentUser, createGameDefinitionRequest.Name)).Start();

                return dataContext.Save(newGameDefinition, currentUser);
            }

            ValidateNotADuplicateGameDefinition(existingGameDefinition);

            existingGameDefinition.Active = true;
            existingGameDefinition.BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionId;
            if (!string.IsNullOrWhiteSpace(createGameDefinitionRequest.Description))
            {
                existingGameDefinition.Description = createGameDefinitionRequest.Description;
            }
            return dataContext.Save(existingGameDefinition, currentUser);
        }


        private static void ValidateNotNull(CreateGameDefinitionRequest createGameDefinitionRequest)
        {
            if (createGameDefinitionRequest == null)
            {
                throw new ArgumentNullException("createGameDefinitionRequest");
            }
        }

        private static void ValidateGameDefinitionNameIsNotNullOrWhitespace(string gameDefinitionName)
        {
            if (string.IsNullOrWhiteSpace(gameDefinitionName))
            {
                throw new ArgumentException("createGameDefinitionRequest.Name cannot be null or whitespace.");
            }
        }

        private static void ValidateNotADuplicateGameDefinition(GameDefinition existingGameDefinition)
        {
            if (existingGameDefinition.Active)
            {
                string message = string.Format("An active Game Definition with name '{0}' already exists in this Gaming Group.", existingGameDefinition.Name);
                throw new DuplicateKeyException(message);
            }
        }

        private int? CreateBoardGameGeekGameDefinition(CreateGameDefinitionRequest createGameDefinitionRequest, ApplicationUser currentUser)
        {
            int? boardGameGeekGameDefinitionId = null;
            if (createGameDefinitionRequest.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekGameDefinitionId = boardGameGeekGameDefinitionAttacher.CreateBoardGameGeekGameDefinition(
                    createGameDefinitionRequest.BoardGameGeekGameDefinitionId.Value, currentUser);
            }

            return boardGameGeekGameDefinitionId;
        }

        public virtual void UpdateGameDefinition(GameDefinitionUpdateRequest gameDefinitionUpdateRequest, ApplicationUser applicationUser)
        {
            var gameDefinition = dataContext.FindById<GameDefinition>(gameDefinitionUpdateRequest.GameDefinitionId);

            if (gameDefinitionUpdateRequest.Active.HasValue)
            {
                gameDefinition.Active = gameDefinitionUpdateRequest.Active.Value;
            }

            if (!string.IsNullOrWhiteSpace(gameDefinitionUpdateRequest.Name))
            {
                gameDefinition.Name = gameDefinitionUpdateRequest.Name;
            }

            if (!string.IsNullOrWhiteSpace(gameDefinitionUpdateRequest.Description))
            {
                gameDefinition.Description = gameDefinitionUpdateRequest.Description;
            }

            if (gameDefinitionUpdateRequest.BoardGameGeekGameDefinitionId.HasValue)
            {
                gameDefinition.BoardGameGeekGameDefinitionId = gameDefinitionUpdateRequest.BoardGameGeekGameDefinitionId;
            }

            dataContext.Save(gameDefinition, applicationUser);
        }
    }
}
