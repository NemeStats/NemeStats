using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.UserRetrieverTests
{
    [TestFixture]
    public class RetrieveUserInformationTests
    {
        private RhinoAutoMocker<UserRetriever> autoMocker;
        private ApplicationUser expectedApplicationUser;
        
        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<UserRetriever>();
        }

        [Test]
        public void It_Throws_An_UnauthorizedEntityAccessException_If_The_User_Requesting_The_Information_Is_Not_The_User_Being_Requested()
        {
            expectedApplicationUser = new ApplicationUser
            {
                Id = "some application user id",
            };
            const string REQUESTED_USER_ID = "some OTHER user id";
            var expectedException = new UnauthorizedEntityAccessException(expectedApplicationUser.Id, typeof(ApplicationUser), REQUESTED_USER_ID);
            
            var actualException = Assert.Throws<UnauthorizedEntityAccessException>(
                () => autoMocker.ClassUnderTest.RetrieveUserInformation(REQUESTED_USER_ID, expectedApplicationUser));

            Assert.That(actualException.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public void It_Sets_The_Basic_User_Information_For_The_Requested_User()
        {
            expectedApplicationUser = new ApplicationUser
            {
                Id = "some application user id",
                UserName = "some user name",
                Email = "some email address",
                UserGamingGroups = new List<UserGamingGroup>
                {
                    new UserGamingGroup
                    {
                        GamingGroup = new GamingGroup()
                    }
                },
                Players = new List<Player>(),
                BoardGameGeekUser = new BoardGameGeekUserDefinition
                {
                    Name = "bgg name",
                    Id = 1
                }
            };
            var userQueryable = new List<ApplicationUser>
            {
                expectedApplicationUser,
                new ApplicationUser()
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(userQueryable);

            var actualResult = autoMocker.ClassUnderTest.RetrieveUserInformation(expectedApplicationUser.Id, expectedApplicationUser);

            Assert.That(actualResult.UserId, Is.EqualTo(expectedApplicationUser.Id));
            Assert.That(actualResult.UserName, Is.EqualTo(expectedApplicationUser.UserName));
            Assert.That(actualResult.Email, Is.EqualTo(expectedApplicationUser.Email));
            Assert.That(actualResult.BoardGameGeekUser.Name, Is.EqualTo(expectedApplicationUser.BoardGameGeekUser.Name));
        }

        [Test]
        public void It_Sets_The_Gaming_Group_Information()
        {
            const int EXPECTED_GAMING_GROUP_ID_1 = 1;
            const string EXPECTED_GAMING_GROUP_NAME_1 = "gaming group name 1";
            const string EXPECTED_URL = "gaming group name 1";
            const string EXPECTED_DESCRIPTION = "gaming group name 1";

            expectedApplicationUser = new ApplicationUser
            {
                Id = "some application user id",
                UserGamingGroups = new List<UserGamingGroup>
                {
                    new UserGamingGroup
                    {
                        GamingGroup = new GamingGroup
                        {
                            Id = EXPECTED_GAMING_GROUP_ID_1,
                            Name = EXPECTED_GAMING_GROUP_NAME_1,
                            PublicGamingGroupWebsite = EXPECTED_URL,
                            PublicDescription = EXPECTED_DESCRIPTION
                        }
                    },
                    new UserGamingGroup
                    {
                        GamingGroup = new GamingGroup()
                    }
                },
                Players = new List<Player>(),
                BoardGameGeekUser = null
            };
            var userQueryable = new List<ApplicationUser>
            {
                expectedApplicationUser
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(userQueryable);

            var actualResult = autoMocker.ClassUnderTest.RetrieveUserInformation(expectedApplicationUser.Id, expectedApplicationUser);

            Assert.That(actualResult.GamingGroups.Count, Is.EqualTo(2));
            var actualGamingGroupInfo = actualResult.GamingGroups[0];
            Assert.That(actualGamingGroupInfo.GamingGroupId, Is.EqualTo(EXPECTED_GAMING_GROUP_ID_1));
            Assert.That(actualGamingGroupInfo.GamingGroupName, Is.EqualTo(EXPECTED_GAMING_GROUP_NAME_1));
            Assert.That(actualGamingGroupInfo.GamingGroupPublicUrl, Is.EqualTo(EXPECTED_URL));
            Assert.That(actualGamingGroupInfo.GamingGroupPublicDescription, Is.EqualTo(EXPECTED_DESCRIPTION));
            Assert.That(actualResult.UserId, Is.EqualTo(expectedApplicationUser.Id));
        }

        [Test]
        public void It_Sets_The_Player_Information()
        {
            const int EXPECTED_PLAYER_ID_1 = 1;
            const string EXPECTED_PLAYER_NAME_1 = "player name 1";
            const int EXPECTED_GAMING_GROUP_ID_1 = 2;

            expectedApplicationUser = new ApplicationUser
            {
                Id = "some application user id",
                UserGamingGroups = new List<UserGamingGroup>
                {
                    new UserGamingGroup
                    {
                        GamingGroup = new GamingGroup()
                    }
                },
                Players = new List<Player>
                {
                    new Player
                    {
                        Id = EXPECTED_PLAYER_ID_1,
                        Name = EXPECTED_PLAYER_NAME_1,
                        GamingGroupId = EXPECTED_GAMING_GROUP_ID_1 
                    },
                    new Player()
                },
                BoardGameGeekUser = new BoardGameGeekUserDefinition
                {
                    Name = EXPECTED_PLAYER_NAME_1,
                    Id = EXPECTED_PLAYER_ID_1
                }
            };
            var userQueryable = new List<ApplicationUser>
            {
                expectedApplicationUser
            }.AsQueryable();
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(userQueryable);

            var actualResult = autoMocker.ClassUnderTest.RetrieveUserInformation(expectedApplicationUser.Id, expectedApplicationUser);

            Assert.That(actualResult.Players.Count, Is.EqualTo(2));
            var actualPlayer = actualResult.Players[0];
            Assert.That(actualPlayer.PlayerId, Is.EqualTo(EXPECTED_PLAYER_ID_1));
            Assert.That(actualPlayer.PlayerName, Is.EqualTo(EXPECTED_PLAYER_NAME_1));
            Assert.That(actualPlayer.GamingGroupId, Is.EqualTo(EXPECTED_GAMING_GROUP_ID_1));

        }
    }
}
