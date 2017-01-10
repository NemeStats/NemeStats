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

using System;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using Shouldly;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture]
    public class EntityFrameworkGeneralIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void TheAddOrInsertExtensionMethodSetsTheIdOnNewEntities()
        {
            var dataContext = GetInstance<IDataContext>();
            GamingGroup gamingGroup = new GamingGroup()
            {
                Name = "new gaming group without an ID yet",
                OwningUserId = testUserWithDefaultGamingGroup.Id
            };

            dataContext.Save(gamingGroup, testUserWithDefaultGamingGroup);
            dataContext.CommitAllChanges();

            Cleanup(dataContext, gamingGroup, testUserWithDefaultGamingGroup);

            Assert.AreNotEqual(default(int), gamingGroup.Id);
        }

        [Test]
        public void TestIncludeMethod()
        {
            var dataContext = GetInstance<IDataContext>();
            List<Player> players = dataContext.GetQueryable<Player>()
                                        .Where(player => player.Active && player.GamingGroupId == 1)
                                        .Include(player => player.Nemesis)
                                        .Include(player => player.Nemesis.NemesisPlayer)

                                        .OrderBy(player => player.Name)
                                        .ToList();

            List<Player> playersWithNemesisid = players.Where(player => player.NemesisId != null).ToList();

            Assert.Greater(playersWithNemesisid.Count, 0);
        }

        private static void Cleanup(
            IDataContext dataContext, 
            GamingGroup gamingGroup, 
            ApplicationUser currentUser)
        {
            GamingGroup gamingGroupToDelete = dataContext
                .GetQueryable<GamingGroup>().FirstOrDefault(game => game.Name == gamingGroup.Name);
            if (gamingGroupToDelete != null)
            {
                dataContext.Delete(gamingGroupToDelete, currentUser);
                dataContext.CommitAllChanges();
            }
        }
    }
}
