using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupSaverTests
{
    public class UpdatePublicGamingGroupDetails : GamingGroupSaverTestBase
    {
        private readonly GamingGroup expectedGamingGroup = new GamingGroup();
        private readonly GamingGroup savedGamingGroup = new GamingGroup();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            autoMocker.Get<IDataContext>().Expect(x => x.FindById<GamingGroup>(Arg<int>.Is.Anything))
                .Return(expectedGamingGroup);

            autoMocker.Get<IDataContext>().Expect(x => x.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(savedGamingGroup);
        }

        [Test]
        public void ItUpdatesTheGamingGroup()
        {
            //--Arrange
            var request = new GamingGroupEditRequest
            {
                PublicDescription = "Description",
                Website = "Website",
                GamingGroupName = "some gaming group name",
                GamingGroupId = currentUser.CurrentGamingGroupId
            };

            //--Act
            autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(request, currentUser);

            //--Assert
            autoMocker.Get<IDataContext>().FindById<GamingGroup>(currentUser.CurrentGamingGroupId);
            autoMocker.Get<IDataContext>().AssertWasCalled(x => x.Save(Arg<GamingGroup>.Matches(
                gamingGroup => gamingGroup.Name == request.GamingGroupName
                  && gamingGroup.PublicDescription == request.PublicDescription
                  && gamingGroup.PublicGamingGroupWebsite == request.Website.ToString()),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItReturnsTheGamingGroupThatWasSaved()
        {
            //--Act
            var gamingGroup = autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(new GamingGroupEditRequest(), currentUser);

            //--Assert
            Assert.AreSame(savedGamingGroup, gamingGroup);
        }

        [Test]
        public void ItTracksThatAGamingGroupWasUpdated()
        {
            autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(new GamingGroupEditRequest(), currentUser);

            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackGamingGroupUpdate(currentUser));
        }
    }
}