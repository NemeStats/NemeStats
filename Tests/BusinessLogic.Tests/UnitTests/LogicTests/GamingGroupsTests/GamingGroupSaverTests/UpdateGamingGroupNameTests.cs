using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupSaverTests
{
    [TestFixture]
    public class UpdateGamingGroupNameTests : GamingGroupSaverTestBase
    {
        private readonly GamingGroup expectedGamingGroup = new GamingGroup();
        private readonly GamingGroup savedGamingGroup = new GamingGroup();

        [Test]
        public override void SetUp()
        {
            base.SetUp();

            dataContextMock.Expect(mock => mock.FindById<GamingGroup>(currentUser.CurrentGamingGroupId.Value))
                .Return(expectedGamingGroup);
            dataContextMock.Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(savedGamingGroup);
        }

        [Test]
        public void ItSetsTheGamingGroupName()
        {
            gamingGroupSaver.UpdateGamingGroupName(gamingGroupName, currentUser);

            dataContextMock.AssertWasCalled(dataContext => dataContext.Save(
                Arg<GamingGroup>.Matches(gamingGroup => gamingGroup.Name == gamingGroupName),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItReturnsTheGamingGroupThatWasSaved()
        {
            GamingGroup gamingGroup = gamingGroupSaver.UpdateGamingGroupName(gamingGroupName, currentUser);

            Assert.AreSame(savedGamingGroup, gamingGroup);
        }
    }
}
