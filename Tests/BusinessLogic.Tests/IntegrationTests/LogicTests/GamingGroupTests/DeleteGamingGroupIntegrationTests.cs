using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.GamingGroupTests
{
    [TestFixture]
    [Category("Integration")]
    [Ignore("Run manually to see if it can wipe out an entire gaming group successfully.")]
    public class DeleteGamingGroupIntegrationTests : IntegrationTestIoCBase
    {
        [Test]
        public void It_Works()
        {
            //--arrange
            var gamingGroupIdToDelete = 14648;
            var dataContext = GetInstance<IDataContext>();
            var gamingGroupDeleter = GetInstance<DeleteGamingGroupComponent>();
            var applicationUserWithAccessToGamingGroup = dataContext.GetQueryable<ApplicationUser>()
                .FirstOrDefault(x => x.UserGamingGroups.Any(y => y.GamingGroupId == gamingGroupIdToDelete));
            applicationUserWithAccessToGamingGroup.ShouldNotBeNull("You picked a gaming group that either doesn't exist or doesn't have any users with access to it.");

            //--act
            gamingGroupDeleter.Execute(gamingGroupIdToDelete, applicationUserWithAccessToGamingGroup);

            //--assert
            var gamingGroupThatShouldntExist = dataContext.GetQueryable<GamingGroup>()
                .FirstOrDefault(x => x.Id == gamingGroupIdToDelete);
            gamingGroupThatShouldntExist.ShouldBeNull();
        }
    }
}
