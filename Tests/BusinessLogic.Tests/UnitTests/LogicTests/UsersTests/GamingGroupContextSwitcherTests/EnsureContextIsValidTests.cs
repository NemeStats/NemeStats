using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.GamingGroupContextSwitcherTests
{
    [TestFixture]
    public class EnsureContextIsValidTests
    {
        private RhinoAutoMocker<GamingGroupContextSwitcher> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<GamingGroupContextSwitcher>();
        }

        [Test]
        public void It_Does_Nothing_If_The_Users_Current_Gaming_Group_Is_Already_Active()
        {
            //--arrange
            var applicationUser = new ApplicationUser
            {
                Id = "some user id",
                CurrentGamingGroupId = 1
            };
            var userGamingGroups = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    ApplicationUserId = applicationUser.Id,
                    GamingGroupId = applicationUser.CurrentGamingGroupId.Value,
                    GamingGroup = new GamingGroup
                    {
                        Active = true
                    }
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>()).Return(userGamingGroups);

            //--act
            _autoMocker.ClassUnderTest.EnsureContextIsValid(applicationUser);

            //--assert
            _autoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.AdminSave(Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Sets_The_Users_CurrentGamingGroupId_To_Null_If_There_Are_No_More_Active_Gaming_Groups_For_That_User()
        {
            //--arrange
            var applicationUser = new ApplicationUser
            {
                Id = "some user id",
                CurrentGamingGroupId = 1
            };
            var userGamingGroups = new List<UserGamingGroup>
            {
                //--record where gaming group is not active
                new UserGamingGroup
                {
                    ApplicationUserId = applicationUser.Id,
                    GamingGroupId = applicationUser.CurrentGamingGroupId.Value,
                    GamingGroup = new GamingGroup
                    {
                        Active = false
                    }
                },
                new UserGamingGroup
                {
                    //--record for a different user
                    ApplicationUserId = "some other application user id",
                    GamingGroupId = applicationUser.CurrentGamingGroupId.Value,
                    GamingGroup = new GamingGroup
                    {
                        Active = true
                    }
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>()).Return(userGamingGroups);

            //--act
            _autoMocker.ClassUnderTest.EnsureContextIsValid(applicationUser);

            //--assert
            applicationUser.CurrentGamingGroupId.ShouldBeNull();
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.AdminSave(Arg<ApplicationUser>
                .Matches(user => user.Id == applicationUser.Id
                    && user.CurrentGamingGroupId == null)));
        }

        [Test]
        public void It_Sets_The_Users_CurrentGamingGroupId_To_The_Oldest_Active_Gaming_Group_For_That_User_If_Their_Current_Gaming_Group_Is_No_Longer_Active()
        {
            //--arrange
            int expectedGamingGroupId = 1;
            var applicationUser = new ApplicationUser
            {
                Id = "some user id",
                CurrentGamingGroupId = -200
            };

            var userGamingGroups = new List<UserGamingGroup>
            {
                new UserGamingGroup
                {
                    ApplicationUserId = applicationUser.Id,
                    GamingGroupId = -1,
                    GamingGroup = new GamingGroup
                    {
                        Active = true,
                        DateCreated = DateTime.UtcNow
                    }
                },
                new UserGamingGroup
                {
                    ApplicationUserId = applicationUser.Id,
                    GamingGroupId = expectedGamingGroupId,
                    GamingGroup = new GamingGroup
                    {
                        Active = true,
                        DateCreated = DateTime.UtcNow.AddYears(-1)
                    }
                },
                new UserGamingGroup
                {
                    ApplicationUserId = "some invalid user id",
                    GamingGroupId = -2,
                    GamingGroup = new GamingGroup
                    {
                        Active = true,
                        DateCreated = DateTime.UtcNow.AddYears(-10)
                    }
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<UserGamingGroup>()).Return(userGamingGroups);

            //--act
            _autoMocker.ClassUnderTest.EnsureContextIsValid(applicationUser);

            //--assert
            applicationUser.CurrentGamingGroupId.ShouldBe(expectedGamingGroupId);
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.AdminSave(Arg<ApplicationUser>
                .Matches(user => user.Id == applicationUser.Id
                    && user.CurrentGamingGroupId == expectedGamingGroupId)));
        }

    }
}
