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

using System.Data.Entity.Infrastructure;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using BoardGameGeekApiClient.Interfaces;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionSaver : IGameDefinitionSaver
    {
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NAME_CANNOT_BE_NULL_OR_WHITESPACE 
            = "gameDefinition.Name cannot be null or whitespace.";

        private readonly IDataContext dataContext;
        private readonly INemeStatsEventTracker eventTracker;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;

        public GameDefinitionSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker, IBoardGameGeekApiClient boardGameGeekApiClient)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
            _boardGameGeekApiClient = boardGameGeekApiClient;
        }

        public virtual GameDefinition Save(GameDefinition gameDefinition, ApplicationUser currentUser)
        {
            ValidateGameDefinitionIsNotNull(gameDefinition);
            ValidateGameDefinitionNameIsNotNullOrWhitespace(gameDefinition.Name);
            bool gameDefinitionAlreadyExists = gameDefinition.AlreadyInDatabase();
            if (gameDefinitionAlreadyExists)
            {
                var existingGameDefinition = dataContext.FindById<GameDefinition>(gameDefinition.Id);
                if(existingGameDefinition != null && existingGameDefinition.BoardGameGeekGameDefinitionId != gameDefinition.BoardGameGeekGameDefinitionId)
                {
                    SetBoardGameGeekThumbnail(gameDefinition);
                }
                return this.dataContext.Save(gameDefinition, currentUser);
            }
            var definition = gameDefinition;

            this.SetBoardGameGeekThumbnail(gameDefinition);

            new Task(() => this.eventTracker.TrackGameDefinitionCreation(currentUser, definition.Name)).Start();

            gameDefinition = this.HandleExistingGameDefinitionWithThisName(gameDefinition, currentUser.CurrentGamingGroupId.Value);

            return dataContext.Save<GameDefinition>(gameDefinition, currentUser);
        }

        private static void ValidateGameDefinitionIsNotNull(GameDefinition gameDefinition)
        {
            if (gameDefinition == null)
            {
                throw new ArgumentNullException("gameDefinition");
            }
        }

        private static void ValidateGameDefinitionNameIsNotNullOrWhitespace(string gameDefinitionName)
        {
            if (string.IsNullOrWhiteSpace(gameDefinitionName))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAME_DEFINITION_NAME_CANNOT_BE_NULL_OR_WHITESPACE);
            }
        }

        private void SetBoardGameGeekThumbnail(GameDefinition gameDefinition)
        {
            if (gameDefinition.BoardGameGeekGameDefinitionId.HasValue)
            {
                var gameDetails = this._boardGameGeekApiClient.GetGameDetails(gameDefinition.BoardGameGeekGameDefinitionId.Value);
                if (gameDetails != null)
                {
                    gameDefinition.ThumbnailImageUrl = gameDetails.Thumbnail;
                }
            }
        }

        private GameDefinition HandleExistingGameDefinitionWithThisName(GameDefinition gameDefinition, int currentUsersGamingGroupId)
        {
            var existingGameDefinition = this.dataContext.GetQueryable<GameDefinition>()
                .FirstOrDefault(x => x.Name == gameDefinition.Name && x.GamingGroupId == currentUsersGamingGroupId);
            if (existingGameDefinition == null)
            {
                return gameDefinition;
            }

            if (existingGameDefinition.Active)
            {
                throw new DuplicateKeyException(string.Format("An active Game Definition with name '{0}' already exists in this Gaming Group.", gameDefinition.Name));
            }

            if (!string.IsNullOrWhiteSpace(gameDefinition.Description))
            {
                existingGameDefinition.Description = gameDefinition.Description;
            }

            existingGameDefinition.Active = true;

            return existingGameDefinition;
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

            if (gameDefinitionUpdateRequest.BoardGameGeekGameDefinitionId.HasValue)
            {
                gameDefinition.BoardGameGeekGameDefinitionId = gameDefinitionUpdateRequest.BoardGameGeekGameDefinitionId;
            }

            this.Save(gameDefinition, applicationUser);
        }
    }
}
