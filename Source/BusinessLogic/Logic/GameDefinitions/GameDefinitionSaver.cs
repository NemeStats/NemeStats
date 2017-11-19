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
using BusinessLogic.Models;
using BusinessLogic.Models.User;
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

        private BoardGameGeekGameDefinition CreateBoardGameGeekGameDefinition(int? boardGameGeekGameDefinitionId, ApplicationUser currentUser)
        {
            BoardGameGeekGameDefinition newBoardGameGeekGameDefinition = null;
            if (boardGameGeekGameDefinitionId.HasValue)
            {
                newBoardGameGeekGameDefinition = _boardGameGeekGameDefinitionCreator.CreateBoardGameGeekGameDefinition(
                    boardGameGeekGameDefinitionId.Value);
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
