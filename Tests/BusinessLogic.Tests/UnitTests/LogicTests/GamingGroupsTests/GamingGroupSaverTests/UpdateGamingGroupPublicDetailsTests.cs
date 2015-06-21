using System;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupSaverTests
{
    public class UpdateGamingGroupPublicDetailsTests : GamingGroupSaverTestBase
    {
        private readonly GamingGroup expectedGamingGroup = new GamingGroup();
        private readonly GamingGroup savedGamingGroup = new GamingGroup();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            dataContextMock.Expect(x => x.FindById<GamingGroup>(currentUser.CurrentGamingGroupId.Value))
                .Return(expectedGamingGroup);

            dataContextMock.Expect(x => x.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
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
                GamingGroupId = currentUser.CurrentGamingGroupId.Value
            };

            //--Act
            gamingGroupSaver.UpdatePublicGamingGroupDetails(request, currentUser);

            //--Assert
            dataContextMock.AssertWasCalled(x => x.Save(Arg<GamingGroup>.Matches(
                gamingGroup => gamingGroup.Name == request.GamingGroupName
                  && gamingGroup.PublicDescription == request.PublicDescription
                  && gamingGroup.PublicGamingGroupWebsite == request.Website.ToString()),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItReturnsTheGamingGroupThatWasSaved()
        {
            //--Act
            var gamingGroup = gamingGroupSaver.UpdateGamingGroupName(gamingGroupName, currentUser);

            //--Assert
            Assert.AreSame(savedGamingGroup, gamingGroup);
        }
    }
}