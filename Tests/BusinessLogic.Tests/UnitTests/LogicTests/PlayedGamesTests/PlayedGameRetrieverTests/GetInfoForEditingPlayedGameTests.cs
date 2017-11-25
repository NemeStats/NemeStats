using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameRetrieverTests
{
    [TestFixture()]
    public class GetInfoForEditingPlayedGameTests
    {
        private RhinoAutoMocker<PlayedGameRetriever> _autoMocker;
        private int _playedGameId = 1;
        private ApplicationUser _currentUser;
        private PlayedGame _expectedPlayedGame;
        private HomePagePlayerSummary _expectedHomePagePlayerSummary;
        private PlayersToCreateModel _expectedPlayersToCreateModel;
        private PlayerGameResult _expectedPlayerGameResult1;
        private PlayerGameResult _expectedPlayerGameResult2;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<PlayedGameRetriever>();

            _currentUser = new ApplicationUser();

            _expectedPlayerGameResult1 = new PlayerGameResult
            {
                GameRank = 1,
                PlayerId = 5,
                PointsScored = 6,
                Player = new Player
                {
                    Name = "player 1"
                }
            };
            _expectedPlayerGameResult2 = new PlayerGameResult
            {
                GameRank = 2,
                PlayerId = 7,
                PointsScored = 3,
                Player = new Player
                {
                    Name = "player 2"
                }
            };

            _expectedPlayedGame = new PlayedGame
            {
                Id = _playedGameId,
                DatePlayed = DateTime.Now,
                Notes = "some notes",
                GameDefinitionId = 2,
                GameDefinition = new GameDefinition
                {
                    Name = "some game definition name",
                    BoardGameGeekGameDefinitionId = 4
                },
                PlayerGameResults = new List<PlayerGameResult>
                {
                    _expectedPlayerGameResult2,
                    _expectedPlayerGameResult1
                }
            };
            var playedGamesQueryable = new List<PlayedGame>
            {
                _expectedPlayedGame
            }.AsQueryable();

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>())
                .Return(playedGamesQueryable);

            _expectedHomePagePlayerSummary = new HomePagePlayerSummary();
            _autoMocker.Get<IHomePagePlayerSummaryRetriever>().Expect(mock =>
                    mock.GetHomePagePlayerSummaryForUser(Arg<string>.Is.Anything, Arg<int>.Is.Anything))
                .Return(_expectedHomePagePlayerSummary);

            _expectedPlayersToCreateModel = new PlayersToCreateModel
            {
                UserPlayer = new PlayerInfoForUser(),
                RecentPlayers = new List<PlayerInfoForUser>(),
                OtherPlayers = new List<PlayerInfoForUser>()
            };
            _autoMocker.Get<IPlayerRetriever>().Expect(mock =>
                    mock.GetPlayersForEditingPlayedGame(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedPlayersToCreateModel);
        }

        [Test]
        public void It_Returns_An_EditPlayedGameInfo_For_The_Specified_Played_Game()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetInfoForEditingPlayedGame(_playedGameId, _currentUser);

            //--assert
            result.DatePlayed.ShouldBe(_expectedPlayedGame.DatePlayed);
            result.GameDefinitionId.ShouldBe(_expectedPlayedGame.GameDefinitionId);
            result.Notes.ShouldBe(_expectedPlayedGame.Notes);
            result.GameDefinitionName.ShouldBe(_expectedPlayedGame.GameDefinition.Name);
            result.BoardGameGeekGameDefinitionId.ShouldBe(_expectedPlayedGame.GameDefinition.BoardGameGeekGameDefinitionId);
            
            _autoMocker.Get<IPlayerRetriever>().AssertWasCalled(mock => mock.GetPlayersForEditingPlayedGame(Arg<int>.Is.Equal(_playedGameId), Arg<ApplicationUser>.Is.Same(_currentUser)));
            result.OtherPlayers.ShouldBeSameAs(_expectedPlayersToCreateModel.OtherPlayers);
            result.RecentPlayers.ShouldBeSameAs(_expectedPlayersToCreateModel.RecentPlayers);
            result.UserPlayer.ShouldBeSameAs(_expectedPlayersToCreateModel.UserPlayer);

            result.PlayerRanks.ShouldNotBeNull();
            result.PlayerRanks.Count.ShouldBe(2);

            var firstPlayerRank = result.PlayerRanks[0];
            firstPlayerRank.PlayerName.ShouldBe(_expectedPlayerGameResult1.Player.Name);
            firstPlayerRank.PlayerId.ShouldBe(_expectedPlayerGameResult1.PlayerId);
            firstPlayerRank.GameRank.ShouldBe(_expectedPlayerGameResult1.GameRank);
            firstPlayerRank.PointsScored.ShouldBe(_expectedPlayerGameResult1.PointsScored);

            var secondPlayerRank = result.PlayerRanks[1];
            secondPlayerRank.PlayerName.ShouldBe(_expectedPlayerGameResult2.Player.Name);
            secondPlayerRank.PlayerId.ShouldBe(_expectedPlayerGameResult2.PlayerId);
            secondPlayerRank.GameRank.ShouldBe(_expectedPlayerGameResult2.GameRank);
            secondPlayerRank.PointsScored.ShouldBe(_expectedPlayerGameResult2.PointsScored);
        }

        [Test]
        public void It_Throws_An_EntityNotFoundException_If_The_Played_Game_Doesnt_Exist()
        {
            //--arrange
            var playedGameIdThatDoesntExist = -1;
            var expectedException = new EntityDoesNotExistException<PlayedGame>(playedGameIdThatDoesntExist);

            //--act
            var exception = Assert.Throws<EntityDoesNotExistException<PlayedGame>>(() => _autoMocker.ClassUnderTest.GetInfoForEditingPlayedGame(playedGameIdThatDoesntExist, _currentUser));

            //--assert
            exception.Message.ShouldBe(expectedException.Message);
        }
    }
}
