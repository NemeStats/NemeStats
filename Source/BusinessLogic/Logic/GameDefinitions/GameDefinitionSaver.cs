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
        private readonly IBoardGameGeekGameDefinitionAttacher boardGameGeekGameDefinitionAttacher;

        public GameDefinitionSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker, IBoardGameGeekApiClient boardGameGeekApiClient, IBoardGameGeekGameDefinitionAttacher boardGameGeekGameDefinitionAttacher)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
            this.boardGameGeekApiClient = boardGameGeekApiClient;
            this.boardGameGeekGameDefinitionAttacher = boardGameGeekGameDefinitionAttacher;
        }

        public GameDefinition CreateGameDefinition(CreateGameDefinitionRequest createGameDefinitionRequest, ApplicationUser applicationUser)
        {
            if(createGameDefinitionRequest == null)
            {
                throw new ArgumentNullException("createGameDefinitionRequest");
            }

            if (string.IsNullOrWhiteSpace(createGameDefinitionRequest.Name))
            {
                throw new ArgumentException("createGameDefinitionRequest.Name cannot be null or whitespace.");
            }

            int? boardGameGeekGameDefinitionId = null;
            if (createGameDefinitionRequest.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekGameDefinitionId = boardGameGeekGameDefinitionAttacher.CreateBoardGameGeekGameDefinition(createGameDefinitionRequest.BoardGameGeekGameDefinitionId);
            }

            var existingGameDefinition = dataContext.GetQueryable<GameDefinition>()
                .FirstOrDefault(game => game.GamingGroupId == applicationUser.CurrentGamingGroupId.Value
                        && game.Name == createGameDefinitionRequest.Name);

            if(existingGameDefinition == null)
            {
                var newGameDefinition = new GameDefinition
                {
                    Name = createGameDefinitionRequest.Name,
                    BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionId,
                    Description = createGameDefinitionRequest.Description,
                    GamingGroupId = applicationUser.CurrentGamingGroupId.Value
                };

                new Task(() => this.eventTracker.TrackGameDefinitionCreation(applicationUser, createGameDefinitionRequest.Name)).Start();

                return dataContext.Save(newGameDefinition, applicationUser);
            }

            if (existingGameDefinition.Active)
            {
                string message = string.Format("An active Game Definition with name '{0}' already exists in this Gaming Group.", existingGameDefinition.Name);
                throw new DuplicateKeyException(message);
            }

            existingGameDefinition.Active = true;
            existingGameDefinition.BoardGameGeekGameDefinitionId = boardGameGeekGameDefinitionId;
            if(!string.IsNullOrWhiteSpace(createGameDefinitionRequest.Description))
            {
                existingGameDefinition.Description = createGameDefinitionRequest.Description;
            }
            return dataContext.Save(existingGameDefinition, applicationUser);
        }

        //public virtual GameDefinitionDisplayInfo CreateGameDefinition(GameDefinition gameDefinition, ApplicationUser currentUser)
        //{
        //    ValidateGameDefinitionIsNotNull(gameDefinition);
        //    ValidateGameDefinitionNameIsNotNullOrWhitespace(gameDefinition.Name);
        //    bool gameDefinitionAlreadyExists = gameDefinition.AlreadyInDatabase();
        //    if (gameDefinitionAlreadyExists)
        //    {
        //        var existingGameDefinition = dataContext.FindById<GameDefinition>(gameDefinition.Id);
        //        if(existingGameDefinition != null && existingGameDefinition.BoardGameGeekGameDefinitionId != gameDefinition.BoardGameGeekGameDefinitionId)
        //        {
        //            SetBoardGameGeekThumbnail(gameDefinition);
        //        }
        //        var savedGameDefinition = this.dataContext.Save(gameDefinition, currentUser);

        //        return new GameDefinitionDisplayInfo();
        //    }
        //    var definition = gameDefinition;

        //    this.SetBoardGameGeekThumbnail(gameDefinition);

        //    new Task(() => this.eventTracker.TrackGameDefinitionCreation(currentUser, definition.Name)).Start();

        //    gameDefinition = this.HandleExistingGameDefinitionWithThisName(gameDefinition, currentUser.CurrentGamingGroupId.Value);

        //    var savedGameDefinition2 = dataContext.Save(gameDefinition, currentUser);

        //    return new GameDefinitionDisplayInfo();

        //}

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
                var gameDetails = this.boardGameGeekApiClient.GetGameDetails(gameDefinition.BoardGameGeekGameDefinitionId.Value);
                if (gameDetails != null)
                {
                    //TODO big refactor
                    //gameDefinition.ThumbnailImageUrl = gameDetails.Thumbnail;
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
