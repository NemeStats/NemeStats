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
    public class GetPlayersForEditingPlayedGameTests : PlayerRetrieverTestBase
    {
        private int _playedGameId = 100;
        private int _gamingGroupId = 20;
        private PlayerGameResult _firstExpectedPlayerGameResult;
        private PlayerGameResult _secondExpectedPlayerGameResult;
        private Player _firstExpectedPlayer;
        private Player _secondExpectedPlayer;

        [SetUp]
        public void SetUp()
        {
            //--reset the base one because it has crappy expectations
            autoMocker = new RhinoAutoMocker<PlayerRetriever>();

            var playedGame = new PlayedGame
            {
                GamingGroupId = _gamingGroupId,
            };

            _firstExpectedPlayerGameResult = new PlayerGameResult
            {
                PlayedGameId = _playedGameId,
                PlayerId = 1,
                Player = new Player
                {
                    Name = "aaa - player 1 name"
                },
                GameRank = 2,
                PlayedGame = playedGame
            };

            _secondExpectedPlayerGameResult = new PlayerGameResult
            {
                PlayedGameId = _playedGameId,
                PlayerId = 2,
                Player = new Player
                {
                    Name = "bbb - player 2 name",
                    //--inactive players are allowed to get pulled
                    Active = false
                },
                GameRank = 1,
                PlayedGame = playedGame
            };

            var playerGameResults = new List<PlayerGameResult>
            {
                _secondExpectedPlayerGameResult,
                _firstExpectedPlayerGameResult,
                //--result that will be excluded due to playedGameId
                new PlayerGameResult
                {
                    PlayedGameId = -1,
                }
            }; 

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerGameResult>())
                .Return(playerGameResults.AsQueryable());

            _firstExpectedPlayer = new Player
            {
                Id = 10,
                Name = "aaa - player 1",
                GamingGroupId = _gamingGroupId,
                Active = true
            };

            _secondExpectedPlayer = new Player
            {
                Id = 20,
                Name = "bbb - player 2",
                GamingGroupId = _gamingGroupId,
                Active = true
            };

            var players = new List<Player>
            {
                _secondExpectedPlayer,
                _firstExpectedPlayer,
                //--exclude because of bad gaming group id
                new Player
                {
                    GamingGroupId = -1
                },
                //--exclude because of inactive
                new Player
                {
                    GamingGroupId = _gamingGroupId,
                    Active = false
                },
                //--exclude because player was already in  "recent players"
                new Player
                {
                    GamingGroupId = _gamingGroupId,
                    Id = _firstExpectedPlayerGameResult.PlayerId
                },
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(players.AsQueryable());
        }

        [Test]
        public void It_Returns_The_Players_In_The_Game_As_The_Recent_Players_Ordered_By_Player_Name()
        {
            //--arrange
            var currentUser = new ApplicationUser();

            //--act
            var result = autoMocker.ClassUnderTest.GetPlayersForEditingPlayedGame(_playedGameId, currentUser);

            //--assert
            result.RecentPlayers.Count.ShouldBe(2);

            var firstResult = result.RecentPlayers[0];
            firstResult.GamingGroupId.ShouldBe(_gamingGroupId);
            firstResult.PlayerId.ShouldBe(_firstExpectedPlayerGameResult.PlayerId);
            firstResult.PlayerName.ShouldBe(_firstExpectedPlayerGameResult.Player.Name);

            var secondResult = result.RecentPlayers[1];
            secondResult.GamingGroupId.ShouldBe(_gamingGroupId);
            secondResult.PlayerId.ShouldBe(_secondExpectedPlayerGameResult.PlayerId);
            secondResult.PlayerName.ShouldBe(_secondExpectedPlayerGameResult.Player.Name);
        }

        [Test]
        public void It_Returns_The_Other_Players_As_The_Players_Who_Were_Not_In_The_Game_Ordered_By_Player_Name()
        {
            //--arrange
            var currentUser = new ApplicationUser();

            //--act
            var result = autoMocker.ClassUnderTest.GetPlayersForEditingPlayedGame(_playedGameId, currentUser);

            //--assert
            result.OtherPlayers.Count.ShouldBe(2);

            var firstResult = result.OtherPlayers[0];
            firstResult.GamingGroupId.ShouldBe(_gamingGroupId);
            firstResult.PlayerId.ShouldBe(_firstExpectedPlayer.Id);
            firstResult.PlayerName.ShouldBe(_firstExpectedPlayer.Name);

            var secondResult = result.OtherPlayers[1];
            secondResult.GamingGroupId.ShouldBe(_gamingGroupId);
            secondResult.PlayerId.ShouldBe(_secondExpectedPlayer.Id);
            secondResult.PlayerName.ShouldBe(_secondExpectedPlayer.Name);
        }

        [Test]
        public void It_Doesnt_Set_The_Current_User_Player_Since_It_Is_Pointless_When_Editing_A_Game()
        {
            //--arrange
            var currentUser = new ApplicationUser();

            //--act
            var result = autoMocker.ClassUnderTest.GetPlayersForEditingPlayedGame(_playedGameId, currentUser);

            //--assert
            result.UserPlayer.ShouldBeNull();
        }

        [Test]
        public void It_Throws_An_Entity_Not_Found_Exception_If_The_Played_Game_Does_Not_Exist()
        {
            //--arrange

            //--act

            //--assert

        }
    }
}
