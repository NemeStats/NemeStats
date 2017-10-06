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

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.UserRetrieverTests
{
    [TestFixture]
    public class RetrieveUserInformationTests
    {
        private RhinoAutoMocker<UserRetriever> _autoMocker;
        
        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UserRetriever>();
        }

        [Test]
        public void It_Sets_The_Basic_User_Information_For_The_Requested_User()
        {
            var expectedApplicationUser = new ApplicationUser
            {
                Id = "some application user id",
                UserName = "some user name",
                Email = "some email address",
                UserGamingGroups = new List<UserGamingGroup>
                {
                    new UserGamingGroup
                    {
                        GamingGroup = new GamingGroup
                        {
                            PlayedGames = new List<PlayedGame>(),
                            Players = new List<Player>()
                        }
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
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(userQueryable);

            var actualResult = _autoMocker.ClassUnderTest.RetrieveUserInformation(expectedApplicationUser);

            actualResult.UserId.ShouldBe(expectedApplicationUser.Id);
            actualResult.UserName.ShouldBe(expectedApplicationUser.UserName);
            actualResult.Email.ShouldBe(expectedApplicationUser.Email);
            actualResult.BoardGameGeekUser.Name.ShouldBe(expectedApplicationUser.BoardGameGeekUser.Name);
        }

        [Test]
        public void It_Sets_The_Gaming_Group_Information()
        {
            const int EXPECTED_GAMING_GROUP_ID_1 = 1;
            const string EXPECTED_GAMING_GROUP_NAME_1 = "gaming group name 1";
            const string EXPECTED_URL = "gaming group name 1";
            const string EXPECTED_DESCRIPTION = "gaming group name 1";

            var expectedChampion = new Player();
            var expectedGamingGroup1 = new GamingGroup
            {
                Id = EXPECTED_GAMING_GROUP_ID_1,
                Name = EXPECTED_GAMING_GROUP_NAME_1,
                PublicGamingGroupWebsite = EXPECTED_URL,
                PublicDescription = EXPECTED_DESCRIPTION,
                Active = !default(bool),
                PlayedGames = new List<PlayedGame>
                {
                    new PlayedGame()
                },
                Players = new List<Player>
                {
                    new Player(),
                    new Player()
                },
                GamingGroupChampion = expectedChampion
            };
            var expectedApplicationUser = new ApplicationUser
            {
                Id = "some application user id",
                UserGamingGroups = new List<UserGamingGroup>
                {
                    new UserGamingGroup
                    {
                        GamingGroup = expectedGamingGroup1
                    },
                    new UserGamingGroup
                    {
                        GamingGroup = new GamingGroup
                        {
                            PlayedGames = new List<PlayedGame>(),
                            Players = new List<Player>()
                        }
                    }
                },
                Players = new List<Player>(),
                BoardGameGeekUser = null
            };
            var userQueryable = new List<ApplicationUser>
            {
                expectedApplicationUser
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(userQueryable);

            var actualResult = _autoMocker.ClassUnderTest.RetrieveUserInformation(expectedApplicationUser);

            actualResult.GamingGroups.Count.ShouldBe(2);
            var actualGamingGroupInfo = actualResult.GamingGroups[0];
            actualGamingGroupInfo.GamingGroupId.ShouldBe(EXPECTED_GAMING_GROUP_ID_1);
            actualGamingGroupInfo.GamingGroupName.ShouldBe(EXPECTED_GAMING_GROUP_NAME_1);
            actualGamingGroupInfo.GamingGroupPublicUrl.ShouldBe(EXPECTED_URL);
            actualGamingGroupInfo.GamingGroupPublicDescription.ShouldBe(EXPECTED_DESCRIPTION);
            actualGamingGroupInfo.Active.ShouldBe(expectedGamingGroup1.Active);
            actualGamingGroupInfo.NumberOfGamesPlayed.ShouldBe(expectedGamingGroup1.PlayedGames.Count);
            actualGamingGroupInfo.NumberOfPlayers.ShouldBe(expectedGamingGroup1.Players.Count);
            actualGamingGroupInfo.GamingGroupChampion.ShouldBeSameAs(expectedChampion);
        }

        [Test]
        public void It_Sets_The_Player_Information()
        {
            const int EXPECTED_PLAYER_ID_1 = 1;
            const string EXPECTED_PLAYER_NAME_1 = "player name 1";
            const int EXPECTED_GAMING_GROUP_ID_1 = 2;

            var expectedApplicationUser = new ApplicationUser
            {
                Id = "some application user id",
                UserGamingGroups = new List<UserGamingGroup>
                {
                    new UserGamingGroup
                    {
                        GamingGroup = new GamingGroup
                        {
                            PlayedGames = new List<PlayedGame>(),
                            Players = new List<Player>()
                        }
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
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<ApplicationUser>()).Return(userQueryable);

            var actualResult = _autoMocker.ClassUnderTest.RetrieveUserInformation(expectedApplicationUser);

            actualResult.Players.Count.ShouldBe(2);
            var actualPlayer = actualResult.Players[0];
            actualPlayer.PlayerId.ShouldBe(EXPECTED_PLAYER_ID_1);
            actualPlayer.PlayerName.ShouldBe(EXPECTED_PLAYER_NAME_1);
            actualPlayer.GamingGroupId.ShouldBe(EXPECTED_GAMING_GROUP_ID_1);
        }
    }
}
