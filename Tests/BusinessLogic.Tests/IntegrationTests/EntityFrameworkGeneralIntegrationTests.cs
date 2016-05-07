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
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture]
    public class EntityFrameworkGeneralIntegrationTests : IntegrationTestBase
    {
        [Test, Ignore("just learning Entity Framework")]
        public void TheAddOrInsertExtensionMethodSetsTheIdOnNewEntities()
        {
            using(NemeStatsDataContext dataContext = new NemeStatsDataContext(
                new NemeStatsDbContext(), 
                new SecuredEntityValidatorFactory()))
            {
                GamingGroup gamingGroup = new GamingGroup()
                {
                    Name = "new gaming group without an ID yet",
                    OwningUserId = testUserWithDefaultGamingGroup.Id
                };

                dataContext.Save(gamingGroup, testUserWithDefaultGamingGroup);
                dataContext.CommitAllChanges();

                int actualId = gamingGroup.Id;
                Cleanup(dataContext, gamingGroup, testUserWithDefaultGamingGroup);

                Assert.AreNotEqual(default(int), gamingGroup.Id);
            }
        }

        [Test, Ignore("playing around")]
        public void TestIncludeMethod()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext(
                            new NemeStatsDbContext(),
                            new SecuredEntityValidatorFactory(),null))
            {
                List<Player> players = dataContext.GetQueryable<Player>()
                                        .Where(player => player.Active && player.GamingGroupId == 1)
                                        .Include(player => player.Nemesis)
                                        .Include(player => player.Nemesis.NemesisPlayer)

                                        .OrderBy(player => player.Name)
                                        .ToList();

                List<Player> playersWithNemesisid = players.Where(player => player.NemesisId != null).ToList();

                Assert.Greater(playersWithNemesisid.Count, 0);
            }
        }

        private static void Cleanup(
            NemeStatsDataContext dbContext, 
            GamingGroup gamingGroup, 
            ApplicationUser currentUser)
        {
            GamingGroup gamingGroupToDelete = dbContext.GetQueryable<GamingGroup>()
                .Where(game => game.Name == gamingGroup.Name).FirstOrDefault();
            if (gamingGroupToDelete != null)
            {
                dbContext.Delete(gamingGroupToDelete, currentUser);
                dbContext.CommitAllChanges();
            }
        }
    }
}
