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

using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class GetQueryableIntegrationTests : IntegrationTestBase
    {
        //TODO why does the top test fail and the bottom test passes? Looks like the clever predicate addition in ApplicationDataContext
        // isn't working :(
        /*
        [Test]
        public void ItAutomaticallyFiltersDownToTheCurrentUsersGamingGroupIfTheEntityIsSecured()
        {
            using(ApplicationDataContext dataContext = new ApplicationDataContext())
            {
                List<GameDefinition> gameDefinitions = dataContext
                    .GetQueryable<GameDefinition>(testUserWithOtherGamingGroup)
                    .ToList();

                Assert.True(gameDefinitions.All(gameDefinition => gameDefinition.GamingGroupId 
                    == testUserWithOtherGamingGroup.CurrentGamingGroupId));
            }
        }

        [Test]
        public void TEMP_testToShowThatTheFilterWorksIfThereIsNoCasting()
        {
            using (NemeStatsDbContext dataContext = new NemeStatsDbContext())
            {
                List<GameDefinition> gameDefinitions = dataContext.Set<GameDefinition>()
                    .Where(x => x.GamingGroupId == testUserWithOtherGamingGroup.CurrentGamingGroupId.Value)
                    .ToList();

                Assert.True(gameDefinitions.All(gameDefinition => gameDefinition.GamingGroupId
                    == testUserWithOtherGamingGroup.CurrentGamingGroupId));
            }
        }
         * */
    }
}
