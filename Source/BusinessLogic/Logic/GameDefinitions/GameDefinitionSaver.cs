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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionSaver : IGameDefinitionSaver
    {
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NAME_CANNOT_BE_NULL_OR_WHITESPACE 
            = "gameDefinition.Name cannot be null or whitespace.";

        private readonly IDataContext dataContext;
        private readonly INemeStatsEventTracker eventTracker;

        public GameDefinitionSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
        }

        public virtual GameDefinition Save(GameDefinition gameDefinition, ApplicationUser currentUser)
        {
            ValidateGameDefinitionIsNotNull(gameDefinition);
            ValidateGameDefinitionNameIsNotNullOrWhitespace(gameDefinition.Name);
            bool isNewGameDefinition = !gameDefinition.AlreadyInDatabase();
            if (isNewGameDefinition)
            {
                new Task(() => eventTracker.TrackGameDefinitionCreation(currentUser, gameDefinition.Name)).Start();
            }
            GameDefinition newGameDefinition = dataContext.Save<GameDefinition>(gameDefinition, currentUser);

            return newGameDefinition;
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

            if (gameDefinitionUpdateRequest.BoardGameGeekObjectId.HasValue)
            {
                gameDefinition.BoardGameGeekObjectId = gameDefinitionUpdateRequest.BoardGameGeekObjectId;
            }

            this.Save(gameDefinition, applicationUser);
        }
    }
}
