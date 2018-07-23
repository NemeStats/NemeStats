using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetRegisteredUserEmailAddressesTests
    {
        private RhinoAutoMocker<PlayerRetriever> _autoMocker;
        private int _playerIdInFirstGamingGroupWithRegisteredUser = 1;
        private int _playerIdInSecondGamingGroupWithRegisteredUser = 2;
        private int _playerIdInSecondGamingGroupWithOutRegisteredUser = 3;
        private int _playerIdNotInGamingGroup = 4;
        private int _currentUserGamingGroupId1 = 50;
        private int _currentUserGamingGroupId2 = 51;
        private string _expectedPlayer1Email = "player1email@email.com";
        private string _expectedPlayer2Email = "player2email@email.com";
        private List<int> _allPotentialPlayerIds;
        private ApplicationUser _currentUser;

        [SetUp]
        public void LocalSetUp()
        {
            _autoMocker = new RhinoAutoMocker<PlayerRetriever>();

            _currentUser = new ApplicationUser
            {
                Id = "some user id",
            };

            var currentUserGamingGroup1 = new UserGamingGroup
            {
                GamingGroupId = _currentUserGamingGroupId1,
                ApplicationUserId = _currentUser.Id
            };
            var currentUserGamingGroup2 = new UserGamingGroup
            {
                GamingGroupId = _currentUserGamingGroupId2,
                ApplicationUserId = _currentUser.Id
            };

            var validPlayer1 = new Player
            {
                Id = _playerIdInFirstGamingGroupWithRegisteredUser,
                GamingGroupId = _currentUserGamingGroupId1,
                ApplicationUserId = "some non-null value",
                User = new ApplicationUser
                {
                    Email = _expectedPlayer1Email
                }
            };

            var validPlayer2 = new Player
            {
                Id = _playerIdInSecondGamingGroupWithRegisteredUser,
                GamingGroupId = _currentUserGamingGroupId2,
                ApplicationUserId = "some non-null value",
                User = new ApplicationUser
                {
                    Email = _expectedPlayer2Email
                }
            };

            var playerInGamingGroupButWithoutARegisteredUser = new Player
            {
                Id = _playerIdInSecondGamingGroupWithOutRegisteredUser,
                GamingGroupId = _currentUserGamingGroupId2
            };

            var playerNotInGamingGroup = new Player
            {
                Id = _playerIdNotInGamingGroup,
                GamingGroupId = -1,
                User = new ApplicationUser
                {
                    Email = "someemailthatwontgetreturned@email.com"
                }
            };

            var expectedUserGamingGroups = new List<UserGamingGroup>
            {
                currentUserGamingGroup1,
                currentUserGamingGroup2
            }.AsQueryable();

            _autoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<UserGamingGroup>())
                .Return(expectedUserGamingGroups);

            var expectedPlayers = new List<Player>
            {
                validPlayer1,
                validPlayer2,
                playerInGamingGroupButWithoutARegisteredUser,
                playerNotInGamingGroup
            }.AsQueryable();

            _autoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Player>())
                .Return(expectedPlayers);

            _allPotentialPlayerIds = expectedPlayers.Select(x => x.Id).ToList();
        }

        [Test]
        public void It_Returns_An_Empty_Dictionary_When_There_Is_No_Current_User()
        {
            //--arrange

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetRegisteredUserEmailAddresses(_allPotentialPlayerIds, null);

            //--assert
            actualResult.ShouldNotBeNull();
            actualResult.Count.ShouldBe(0);
        }

        [Test]
        public void It_Doesnt_Return_Results_For_Players_That_Arent_In_One_Of_The_Current_Users_Gaming_Groups()
        {
            //--arrange
            var userWithNoGamingGroups = new ApplicationUser();

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetRegisteredUserEmailAddresses(_allPotentialPlayerIds, userWithNoGamingGroups);

            //--assert
            actualResult.ShouldNotBeNull();
            actualResult.Count.ShouldBe(0);
        }

        [Test]
        public void It_Only_Returns_Records_For_Players_Who_Are_In_The_Same_Gaming_Group_As_The_Current_User()
        {
            //--arrange

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetRegisteredUserEmailAddresses(_allPotentialPlayerIds, _currentUser);

            //--assert
            actualResult.ShouldNotBeNull();
            actualResult.Count.ShouldBe(2);

            actualResult.ShouldContainKey(_playerIdInFirstGamingGroupWithRegisteredUser);
            actualResult[_playerIdInFirstGamingGroupWithRegisteredUser].ShouldBe(_expectedPlayer1Email);

            actualResult.ShouldContainKey(_playerIdInSecondGamingGroupWithRegisteredUser);
            actualResult[_playerIdInSecondGamingGroupWithRegisteredUser].ShouldBe(_expectedPlayer2Email);
        }

        [Test]
        public void It_Returns_An_Empty_Dictionary_If_No_Players_Are_In_The_Current_Users_Gaming_Groups()
        {
            //--arrange
            var userWithNoGamingGroups = new ApplicationUser
            {
                Id = "some user with no gaming groups"
            };

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetRegisteredUserEmailAddresses(_allPotentialPlayerIds, userWithNoGamingGroups);

            //--assert
            actualResult.ShouldNotBeNull();
            actualResult.Count.ShouldBe(0);
        }
    }
}
