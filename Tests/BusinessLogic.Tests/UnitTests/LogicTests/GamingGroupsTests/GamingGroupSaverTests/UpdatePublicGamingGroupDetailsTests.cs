using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

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
                    GamingGroupId = _expectedGamingGroupId,
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
                GamingGroupId = _expectedGamingGroupId,
                Active = true
            };

            //--Act
            autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(request, currentUser);

            //--Assert
            autoMocker.Get<IDataContext>().FindById<GamingGroup>(currentUser.CurrentGamingGroupId);
            autoMocker.Get<IDataContext>().AssertWasCalled(x => x.Save(Arg<GamingGroup>.Matches(
                gamingGroup => gamingGroup.Name == request.GamingGroupName
                  && gamingGroup.PublicDescription == request.PublicDescription
                  && gamingGroup.PublicGamingGroupWebsite == request.Website.ToString()
                  && gamingGroup.Active == request.Active),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void It_Switches_The_Gaming_Group_Of_Each_User_Who_Had_Their_Current_Gaming_Group_Set_To_This_One()
        {
            //--arrange
            var expectedUser1 = new ApplicationUser
            {
                Id = "user id 1",
                CurrentGamingGroupId = _expectedGamingGroupId
            };
            var expectedUser2 = new ApplicationUser
            {
                Id = "user id 2",
                CurrentGamingGroupId = _expectedGamingGroupId
            };

            var usersWithThisAsCurrentGamingGroup = new List<ApplicationUser>
            {
               expectedUser1,
               expectedUser2,
               //--record that doesn't match the gaming group
               new ApplicationUser
               {
                   CurrentGamingGroupId = -1
               }
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(usersWithThisAsCurrentGamingGroup);

            var request = new GamingGroupEditRequest
            {
                GamingGroupId = _expectedGamingGroupId,
                Active = false
            };

            //--act
            autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(request, currentUser);

            //--assert
            autoMocker.Get<IGamingGroupContextSwitcher>().AssertWasCalled(mock => mock.EnsureContextIsValid(expectedUser1));
            autoMocker.Get<IGamingGroupContextSwitcher>().AssertWasCalled(mock => mock.EnsureContextIsValid(expectedUser2));
        }

        [Test]
        public void ItReturnsTheGamingGroupThatWasSaved()
        {
            //--arrange
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(new List<ApplicationUser>().AsQueryable());

            //--Act
            var gamingGroup = autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(new GamingGroupEditRequest(), currentUser);

            //--Assert
            Assert.AreSame(_savedGamingGroup, gamingGroup);
        }
 
        [Test]
        public void ItTracksThatAGamingGroupWasUpdated()
        {
            //--arrange
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(new List<ApplicationUser>().AsQueryable());

            //--act
            autoMocker.ClassUnderTest.UpdatePublicGamingGroupDetails(new GamingGroupEditRequest(), currentUser);

            //--assert
            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackGamingGroupUpdate(currentUser));
        }
    }
}