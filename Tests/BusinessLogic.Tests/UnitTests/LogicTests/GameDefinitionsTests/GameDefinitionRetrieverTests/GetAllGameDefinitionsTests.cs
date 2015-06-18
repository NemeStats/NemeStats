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
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetAllGameDefinitionsTests : GameDefinitionRetrieverTestBase
    {
        [Test]
        public void ItOnlyReturnsActiveGameDefinitions()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>())
                .Repeat.Once()
                .Return(gameDefinitionQueryable);


            IList<GameDefinitionSummary> gameDefinitions = autoMocker.ClassUnderTest.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value);

            Assert.True(gameDefinitions.All(gameDefinition => gameDefinition.Active));
        }

        [Test]
        public void ItOnlyReturnsGameDefinitionsForTheCurrentUsersGamingGroup()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>())
                .Repeat.Once()
                .Return(gameDefinitionQueryable);

            IList<GameDefinitionSummary> gameDefinitions = autoMocker.ClassUnderTest.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value);

            Assert.True(gameDefinitions.All(gameDefinition => gameDefinition.GamingGroupId == currentUser.CurrentGamingGroupId));
        }
    }
}
