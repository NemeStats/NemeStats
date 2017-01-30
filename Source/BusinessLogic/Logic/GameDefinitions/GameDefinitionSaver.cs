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
using BusinessLogic.Models.Games;
using BusinessLogic.Logic.BoardGameGeek;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionSaver : IGameDefinitionSaver
    {
        internal const string ExceptionMessageGameDefinitionNameCannotBeNullOrWhitespace 
            = "gameDefinition.Name cannot be null or whitespace.";

        private readonly IDataContext _dataContext;
        private readonly INemeStatsEventTracker _eventTracker;
        private readonly IBoardGameGeekGameDefinitionCreator _boardGameGeekGameDefinitionCreator;

        public GameDefinitionSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker, IBoardGameGeekGameDefinitionCreator boardGameGeekGameDefinitionAttacher)
        {
            _dataContext = dataContext;
            _eventTracker = eventTracker;
            _boardGameGeekGameDefinitionCreator = boardGameGeekGameDefinitionAttacher;
        }

        public GameDefinition CreateGameDefinition(CreateGameDefinitionRequest createGameDefinitionRequest, ApplicationUser currentUser)
        {
            ValidateNotNull(createGameDefinitionRequest);

            ValidateGameDefinitionNameIsNotNullOrWhitespace(createGameDefinitionRequest.Name);

            int gamingGroupId = createGameDefinitionRequest.GamingGroupId ?? currentUser.CurrentGamingGroupId;

            var boardGameGeekGameDefinition = CreateBoardGameGeekGameDefinition(
                createGameDefinitionRequest.BoardGameGeekGameDefinitionId, 
                currentUser);
            
            var existingGameDefinition = _dataContext.GetQueryable<GameDefinition>()
                .FirstOrDefault(game => game.GamingGroupId == gamingGroupId
                        && game.Name == createGameDefinitionRequest.Name);

            if (existingGameDefinition == null)
            {
                var newGameDefinition = new GameDefinition
                {
                    Name = createGameDefinitionRequest.Name,
                    BoardGameGeekGameDefinitionId = boardGameGeekGameDefinition?.Id,
                    Description = createGameDefinitionRequest.Description,
                    GamingGroupId = gamingGroupId
                };

                new Task(() => _eventTracker.TrackGameDefinitionCreation(currentUser, createGameDefinitionRequest.Name)).Start();

                return _dataContext.Save(newGameDefinition, currentUser);
            }

            ValidateNotADuplicateGameDefinition(existingGameDefinition);

            existingGameDefinition.Active = true;
            existingGameDefinition.BoardGameGeekGameDefinitionId = boardGameGeekGameDefinition?.Id;
            if (!string.IsNullOrWhiteSpace(createGameDefinitionRequest.Description))
            {
                existingGameDefinition.Description = createGameDefinitionRequest.Description;
            }
            return _dataContext.Save(existingGameDefinition, currentUser);
        }


        private static void ValidateNotNull(CreateGameDefinitionRequest createGameDefinitionRequest)
        {
            if (createGameDefinitionRequest == null)
            {
                throw new ArgumentNullException(nameof(createGameDefinitionRequest));
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
                string message = $"An active Game Definition with name '{existingGameDefinition.Name}' already exists in this Gaming Group.";
                throw new DuplicateKeyException(message);
            }
        }

        private BoardGameGeekGameDefinition CreateBoardGameGeekGameDefinition(int? boardGameGeekGameDefinitionId, ApplicationUser currentUser)
        {
            BoardGameGeekGameDefinition newBoardGameGeekGameDefinition = null;
            if (boardGameGeekGameDefinitionId.HasValue)
            {
                newBoardGameGeekGameDefinition = _boardGameGeekGameDefinitionCreator.CreateBoardGameGeekGameDefinition(
                    boardGameGeekGameDefinitionId.Value, currentUser);
            }

            return newBoardGameGeekGameDefinition;
        }

        public virtual void UpdateGameDefinition(GameDefinitionUpdateRequest gameDefinitionUpdateRequest, ApplicationUser currentUser)
        {
            var gameDefinition = _dataContext.FindById<GameDefinition>(gameDefinitionUpdateRequest.GameDefinitionId);

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

            AttachToBoardGameGeekGameDefinition(gameDefinitionUpdateRequest.BoardGameGeekGameDefinitionId, currentUser, gameDefinition);

            _dataContext.Save(gameDefinition, currentUser);
        }

        private void AttachToBoardGameGeekGameDefinition(int? boardGameGeekGameDefinitionId, ApplicationUser currentUser, GameDefinition gameDefinition)
        {
            if (!boardGameGeekGameDefinitionId.HasValue)
            {
                return;
            }

            var newlyCreatedBoardGameGeekGameDefinition = CreateBoardGameGeekGameDefinition(
                boardGameGeekGameDefinitionId.Value,
                currentUser);
            if (newlyCreatedBoardGameGeekGameDefinition != null)
            {
                gameDefinition.BoardGameGeekGameDefinitionId = newlyCreatedBoardGameGeekGameDefinition.Id;
            }
        }
    }
}
