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

using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    public class GamingGroupRetrieverTestBase
    {
        protected GamingGroupRetriever gamingGroupRetriever;
        protected IDataContext dataContextMock;
        protected IPlayerRetriever playerRetrieverMock;
        protected IGameDefinitionRetriever gameDefinitionRetrieverMock;
        protected IPlayedGameRetriever playedGameRetriever;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRetrieverMock = MockRepository.GenerateMock<IPlayerRetriever>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<IGameDefinitionRetriever>();
            playedGameRetriever = MockRepository.GenerateMock<IPlayedGameRetriever>();
            gamingGroupRetriever = new GamingGroupRetriever(
                dataContextMock,
                playerRetrieverMock,
                gameDefinitionRetrieverMock,
                playedGameRetriever);

            currentUser = new ApplicationUser()
            {
                Id = "application user",
                UserName = "user name",
                CurrentGamingGroupId = 1
            };
        }
    }
}
