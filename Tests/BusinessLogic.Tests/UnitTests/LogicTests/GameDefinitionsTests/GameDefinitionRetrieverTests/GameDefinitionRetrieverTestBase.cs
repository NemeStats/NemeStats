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
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    public class GameDefinitionRetrieverTestBase
    {
        protected RhinoAutoMocker<GameDefinitionRetriever> autoMocker; 
        protected ApplicationUser currentUser;
        protected IQueryable<GameDefinition> gameDefinitionQueryable;
        protected int gamingGroupId = 123;
        protected const int CHAMPION_ID = 1;
        protected const int PREVIOUS_CHAMPION_ID = 2;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<GameDefinitionRetriever>();
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = gamingGroupId
            };
            var gameDefinitions = new List<GameDefinition>()
            {
                new GameDefinition() { Id = 1, Active = true, GamingGroupId = gamingGroupId, PlayedGames = new List<PlayedGame>()},  
                new GameDefinition() { Id = 2, Active = false, GamingGroupId = gamingGroupId, PlayedGames = new List<PlayedGame>() },
                new GameDefinition() 
                { 
                    Id = 3, 
                    Active = true, 
                    GamingGroupId = -1, 
                    PlayedGames = new List<PlayedGame>(), 
                    Champion = new Champion
                    {
                        Player = new Player
                        {
                            Id = CHAMPION_ID
                        }
                    },
                    PreviousChampion = new Champion()
                    {
                        Player = new Player
                        {
                            Id = PREVIOUS_CHAMPION_ID
                        }
                    }
                }
            };
            gameDefinitionQueryable = gameDefinitions.AsQueryable();

            var players = new List<Player>
            {
                new Player
                {
                    Id = CHAMPION_ID
                },
                new Player
                {
                    Id = PREVIOUS_CHAMPION_ID
                }
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(players.AsQueryable());
        }
    }
}
