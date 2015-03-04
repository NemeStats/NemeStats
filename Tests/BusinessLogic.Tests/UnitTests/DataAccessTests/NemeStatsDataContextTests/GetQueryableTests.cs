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
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class GetQueryableTests : NemeStatsDataContextTestBase
    {
        [Test]
        [Ignore("Having trouble testing this. Going to try an integration test instead.")]
        public void ItAutomaticallyFiltersOnGamingGroupIfTheEntityIsSecured()
        {
            DbSet<GameDefinition> dbSetMock = MockRepository.GenerateMock<DbSet<GameDefinition>>();
            //IQueryable<GameDefinition> gameDefinitionQueryable;
            nemeStatsDbContext.Expect(mock => mock.Set<GameDefinition>())
                .Repeat.Once()
                .Return(dbSetMock);

            //IQueryable<GameDefinition> gameDefinitionQueryable = dataContext.GetQueryable<GameDefinition>(currentUser);

            //TODO any way to check the predicate? or do I really just need an integration test here?
        }
    }
}
