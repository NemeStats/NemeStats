using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupSaverTests
{
    public class UpdatePublicGamingGroupDetailsTests : GamingGroupSaverTestBase
    {
        private readonly GamingGroup _expectedGamingGroup = new GamingGroup();
        private readonly GamingGroup _savedGamingGroup = new GamingGroup();
        private readonly int _expectedGamingGroupId = 1;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            autoMocker.Get<IDataContext>().Expect(x => x.FindById<GamingGroup>(Arg<int>.Is.Anything))
                .Return(_expectedGamingGroup);

            autoMocker.Get<IDataContext>().Expect(x => x.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_savedGamingGroup);
        }

        private void SetupUserGamingGroups()
        {
            var userGamingGroups = new List<UserGamingGroup>
            {
                //--non-matching user id
                new UserGamingGroup
                {
                    ApplicationUserId = "some other user id",
                    GamingGroup = new GamingGroup
                    {
                        Active = true
                    }
                },
                //--active is false
                new UserGamingGroup
                {
                    ApplicationUserId = currentUser.Id,
                    GamingGroup = new GamingGroup
                    {
                        Active = false
                    }
                },
                //--same gaming group the user is already on
                new UserGamingGroup
                {
                    ApplicationUserId = currentUser.Id,
                    GamingGroupId = currentUser.CurrentGamingGroupId,
                    GamingGroup = new GamingGroup
                    {
                        Active = true
                    }
                },
                new UserGamingGroup
                {
                    ApplicationUserId = currentUser.Id,
                    GamingGroupId = _expectedGamingGroupId,
                    GamingGroup = new GamingGroup
                    {
                        Active = true
                    }
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>()).Return(userGamingGroups);
        }

        [Test]
        public void It_Updates_The_Gaming_Group()
        {
            //--Arrange
            SetupUserGamingGroups();

            var request = new GamingGroupEditRequest
            {
                PublicDescription = "Description",
                Website = "Website",
                GamingGroupName = "some gaming group name",
                GamingGroupId = currentUser.CurrentGamingGroupId,
                Active = false
            };

            //--Act
            autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(request, currentUser);

            //--Assert
            autoMocker.Get<IDataContext>().FindById<GamingGroup>(currentUser.CurrentGamingGroupId);
            autoMocker.Get<IDataContext>().AssertWasCalled(x => x.Save(Arg<GamingGroup>.Matches(
                gamingGroup => gamingGroup.Name == request.GamingGroupName
                  && gamingGroup.PublicDescription == request.PublicDescription
                  && gamingGroup.PublicGamingGroupWebsite == request.Website.ToString()
                  && gamingGroup.Active == false),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void It_Throws_A_Last_Valid_Gaming_Group_Exception_If_The_User_Tries_To_Deactivate_Their_Only_Remaining_Gaming_Group()
        {
            //--arrange
            SetupSingleUserGamingGroup(active: false);

            var request = new GamingGroupEditRequest
            {
                GamingGroupId = currentUser.CurrentGamingGroupId,
                Active = false
            };
            var expectedException = new LastValidGamingGroupException(currentUser.CurrentGamingGroupId);

            //--act
            var exception = Assert.Throws<LastValidGamingGroupException>(() => autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(request, currentUser));

            //--assert
            exception.Message.ShouldBe(expectedException.Message);
        }

        private void SetupSingleUserGamingGroup(bool active = true)
        {
            var userGamingGroups = new List<UserGamingGroup>
            {
                //--non-matching user id
                new UserGamingGroup
                {
                    ApplicationUserId = currentUser.Id,
                    GamingGroupId = currentUser.CurrentGamingGroupId,
                    GamingGroup = new GamingGroup
                    {
                        Active = active
                    }
                }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>()).Return(userGamingGroups);
        }

        [Test]
        public void It_Changes_The_Users_Current_Gaming_Group_If_They_Deactivated_Their_Current_Gaming_Group_And_There_Is_Another_Active()
        {
            //--arrange
            SetupUserGamingGroups();
            var request = new GamingGroupEditRequest
            {
                GamingGroupId = currentUser.CurrentGamingGroupId,
                Active = false
            };

            //--act
            autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(request, currentUser);

            //--assert
            autoMocker.Get<IGamingGroupContextSwitcher>().AssertWasCalled(mock => mock.SwitchGamingGroupContext(_expectedGamingGroupId, currentUser));
        }

//WHAT ABOUT CASE WHEN DEACTIVATING THE ONLY GAMING GROUP FOR A DIFFERENT USER?

        [Test]
        public void ItReturnsTheGamingGroupThatWasSaved()
        {
            //--arrange
            SetupSingleUserGamingGroup();

            //--Act
            var gamingGroup = autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(new GamingGroupEditRequest(), currentUser);

            //--Assert
            Assert.AreSame(_savedGamingGroup, gamingGroup);
        }
 
        [Test]
        public void ItTracksThatAGamingGroupWasUpdated()
        {
            //--arrange
            SetupSingleUserGamingGroup();

            //--act
            autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(new GamingGroupEditRequest(), currentUser);

            //--assert
            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackGamingGroupUpdate(currentUser));
        }
    }
}