using System;
using System.Collections.Generic;
using BusinessLogic.Logic;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerQuickStatsRetrieverTests
{
    [TestFixture]
    public class GetHomePagePlayerSummaryForUserTests
    {
        private RhinoAutoMocker<HomePagePlayerSummaryRetriever> _autoMocker;

        private int _gamingGroupId = 1;
        private string _applicationUserId = "some user id";
        private HomePagePlayerSummary _expectedHomePagePlayerSummary;
        private PlayerQuickStats _expectedPlayerQuickStats;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<HomePagePlayerSummaryRetriever>();

            _expectedPlayerQuickStats = new PlayerQuickStats();
            _autoMocker.Get<IPlayerRetriever>().Expect(mock =>
                    mock.GetPlayerQuickStatsForUser(Arg<string>.Is.Anything, Arg<int>.Is.Anything))
                .Return(_expectedPlayerQuickStats);

            _expectedHomePagePlayerSummary = new HomePagePlayerSummary();
            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<HomePagePlayerSummary>(_expectedPlayerQuickStats))
                .Return(_expectedHomePagePlayerSummary);
        }

        [Test]
        public void It_Returns_Player_Quick_Stats_Plus_Latest_Played_Game_Info()
        {
            var expectedBggGameDefinitionId = 2;

            var expectedPlayedGame = MockRepository.GeneratePartialMock<PlayedGame>();
            expectedPlayedGame.DatePlayed = new DateTime();
            expectedPlayedGame.GameDefinitionId = 1;
            expectedPlayedGame.WinnerType = WinnerTypes.PlayerWin;
            expectedPlayedGame.GameDefinition = new GameDefinition
            {
                Name = "some game definition name",
                BoardGameGeekGameDefinitionId = expectedBggGameDefinitionId,
                BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
                {
                    Name = "bgg name",
                    Thumbnail = "some thumbnail",
                    Id = expectedBggGameDefinitionId
                }
            };
            var playedGames = new List<PlayedGame>
            {
                expectedPlayedGame
            };
            var expectedWinningPlayer = new Player
            {
                Id = 93,
                Name = "some winning player name"
            };
            expectedPlayedGame.Expect(mock => mock.WinningPlayer).Repeat.Any().Return(expectedWinningPlayer);
            _autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentGames(1, _gamingGroupId))
                .Return(playedGames);

            //--act
            var result = _autoMocker.ClassUnderTest.GetHomePagePlayerSummaryForUser(_applicationUserId, _gamingGroupId);

            _autoMocker.Get<IPlayerRetriever>().AssertWasCalled(mock => mock.GetPlayerQuickStatsForUser(Arg<string>.Is.Equal(_applicationUserId), Arg<int>.Is.Equal(_gamingGroupId)));
            _autoMocker.Get<ITransformer>().AssertWasCalled(mock => mock.Transform<HomePagePlayerSummary>(Arg<PlayerQuickStats>.Is.Same(_expectedPlayerQuickStats)));

            result.ShouldBeSameAs(_expectedHomePagePlayerSummary);
            var lastGamingGroupGame = result.LastGamingGroupPlayedGame;
            Assert.That(lastGamingGroupGame, Is.Not.Null);
            Assert.That(lastGamingGroupGame.DatePlayed, Is.EqualTo(expectedPlayedGame.DatePlayed));
            Assert.That(lastGamingGroupGame.GameDefinitionId, Is.EqualTo(expectedPlayedGame.GameDefinitionId));
            Assert.That(lastGamingGroupGame.GameDefinitionName, Is.EqualTo(expectedPlayedGame.GameDefinition.Name));
            Assert.That(lastGamingGroupGame.PlayedGameId, Is.EqualTo(expectedPlayedGame.Id));
            var expectedBggUri = BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(expectedBggGameDefinitionId);
            Assert.That(lastGamingGroupGame.BoardGameGeekUri, Is.EqualTo(expectedBggUri));
            Assert.That(lastGamingGroupGame.ThumbnailImageUrl, Is.EqualTo(expectedPlayedGame.GameDefinition.BoardGameGeekGameDefinition.Thumbnail));
            Assert.That(lastGamingGroupGame.WinnerType, Is.EqualTo(expectedPlayedGame.WinnerType));
            Assert.That(lastGamingGroupGame.WinningPlayerName, Is.EqualTo(expectedWinningPlayer.Name));
            Assert.That(lastGamingGroupGame.WinningPlayerId, Is.EqualTo(expectedWinningPlayer.Id));
        }
    }
}
