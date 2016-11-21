using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;
using UI.Controllers;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Models.UniversalGameModels;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.UniversalGameControllerTests
{
    [TestFixture]
    public class DetailsTests
    {
        private RhinoAutoMocker<UniversalGameController> _autoMocker;

        private int _boardGameGeekGameDefinitionId = 1;
        private ApplicationUser _currentUser;
        private BoardGameGeekGameSummary _expectedBoardGameGeekGameSummary;
        private UniversalGameDetailsViewModel _expectedUniversalGameDetailsViewModel;
        private GameDefinitionSummary _expectedGameDefinitionSummary;
        private PlayedGame _expectedPlayedGame1;
        private PlayedGame _expectedPlayedGame2;

        private PlayerWinRecord _expectedPlayerWinRecord1;
        private PlayerWinRecord _expectedPlayerWinRecord2;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalGameController>();
            _currentUser = new ApplicationUser();

            _expectedPlayedGame1 = new PlayedGame();
            _expectedPlayedGame2 = new PlayedGame();
            _expectedPlayerWinRecord1 = new PlayerWinRecord();
            _expectedPlayerWinRecord2 = new PlayerWinRecord();
            _expectedGameDefinitionSummary = new GameDefinitionSummary
            {
                Id = 50,
                AveragePlayersPerGame = 2.59M,
                GamingGroupName = "some gaming group name",
                TotalNumberOfGamesPlayed = 3,
                PlayedGames = new List<PlayedGame>
                {
                    _expectedPlayedGame1,
                    _expectedPlayedGame2
                },
                PlayerWinRecords = new List<PlayerWinRecord>
                {
                    _expectedPlayerWinRecord1,
                    _expectedPlayerWinRecord2
                }
            };
            _expectedBoardGameGeekGameSummary = new BoardGameGeekGameSummary
            {
                GamingGroupGameDefinitionSummary = _expectedGameDefinitionSummary
            };
            _expectedUniversalGameDetailsViewModel = new UniversalGameDetailsViewModel();

            _autoMocker.Get<IUniversalGameRetriever>().Expect(mock => mock.GetBoardGameGeekGameSummary(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything, Arg<int>.Is.Anything))
                .Return(_expectedBoardGameGeekGameSummary);
            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<UniversalGameDetailsViewModel>(Arg<BoardGameGeekGameSummary>.Is.Anything))
                .Return(_expectedUniversalGameDetailsViewModel);
        }

        [Test]
        public void It_Returns_The_UniversalGameDetailsViewModel()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser) as ViewResult;

            //--assert
            _autoMocker.Get<IUniversalGameRetriever>().AssertWasCalled(mock => mock.GetBoardGameGeekGameSummary(_boardGameGeekGameDefinitionId, _currentUser));
            _autoMocker.Get<ITransformer>().AssertWasCalled(mock => mock.Transform<UniversalGameDetailsViewModel>(_expectedBoardGameGeekGameSummary));
            result.ShouldNotBeNull();
            result.ViewName.ShouldBe(MVC.UniversalGame.Views.Details);
            result.Model.ShouldBeSameAs(_expectedUniversalGameDetailsViewModel);
        }

        [Test]
        public void It_Populates_The_GamingGroupGameDefinitionSummary_Basic_Fields()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser) as ViewResult;

            //--assert
            var model = result.Model as UniversalGameDetailsViewModel;
            var gamingGroupSummary = model.GamingGroupGameDefinitionSummary;
            gamingGroupSummary.GamingGroupId.ShouldBe(_expectedGameDefinitionSummary.GamingGroupId);
            gamingGroupSummary.GamingGroupName.ShouldBe(_expectedGameDefinitionSummary.GamingGroupName);
            gamingGroupSummary.PlayedGamesPanelTitle.ShouldBe("Last " + _expectedGameDefinitionSummary.PlayedGames.Count + " Played Games");
        }

        [Test]
        public void It_Populates_The_GamingGroupGameDefinitionSummary_Game_Definition_Stats()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser) as ViewResult;

            //--assert
            var model = result.Model as UniversalGameDetailsViewModel;
            var gamingGroupSummary = model.GamingGroupGameDefinitionSummary;
            gamingGroupSummary.GamingGroupGameDefinitionStats.ShouldNotBeNull();
            gamingGroupSummary.GamingGroupGameDefinitionStats.AveragePlayersPerGame.ShouldBe("2.6");
            gamingGroupSummary.GamingGroupGameDefinitionStats.TotalNumberOfGamesPlayed.ShouldBe(_expectedGameDefinitionSummary.TotalNumberOfGamesPlayed);
        }

        [Test]
        public void It_Populates_The_Played_Games_For_The_GamingGroup()
        {
            //--arrange
            var expectedPlayedGameDetailViewModel1 = new PlayedGameDetailsViewModel();
            _autoMocker.Get<IPlayedGameDetailsViewModelBuilder>().Expect(mock => mock.Build(_expectedPlayedGame1, _currentUser))
                .Return(expectedPlayedGameDetailViewModel1);

            var expectedPlayedGameDetailViewModel2 = new PlayedGameDetailsViewModel();
            _autoMocker.Get<IPlayedGameDetailsViewModelBuilder>().Expect(mock => mock.Build(_expectedPlayedGame2, _currentUser))
                .Return(expectedPlayedGameDetailViewModel2);

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser) as ViewResult;

            //--assert
            var model = result.Model as UniversalGameDetailsViewModel;
            var gamingGroupSummary = model.GamingGroupGameDefinitionSummary;
            gamingGroupSummary.PlayedGames.ShouldContain(expectedPlayedGameDetailViewModel1);
            gamingGroupSummary.PlayedGames.ShouldContain(expectedPlayedGameDetailViewModel2);
        }

        [Test]
        public void It_Populates_The_Player_Summaries_For_The_GamingGroup()
        {
            //--arrange
            var expectedPlayerSummaryViewModel1 = new GameDefinitionPlayerSummaryViewModel();
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GameDefinitionPlayerSummaryViewModel>(_expectedPlayerWinRecord1))
                .Return(expectedPlayerSummaryViewModel1);

            var expectedPlayerSummaryViewModel2 = new GameDefinitionPlayerSummaryViewModel();
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GameDefinitionPlayerSummaryViewModel>(_expectedPlayerWinRecord2))
                .Return(expectedPlayerSummaryViewModel2);

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser) as ViewResult;

            //--assert
            var model = result.Model as UniversalGameDetailsViewModel;
            var gamingGroupSummary = model.GamingGroupGameDefinitionSummary;
            gamingGroupSummary.GameDefinitionPlayerSummaries.ShouldContain(expectedPlayerSummaryViewModel1);
            gamingGroupSummary.GameDefinitionPlayerSummaries.ShouldContain(expectedPlayerSummaryViewModel2);
        }

        [Test]
        public void It_Returns_A_Null_GamingGroupGameDefinitionSummary_If_The_Current_User_Doesnt_Have_The_Game_Definition()
        {
            //--arrange
            _autoMocker.Get<IUniversalGameRetriever>().BackToRecord();

            _expectedBoardGameGeekGameSummary.GamingGroupGameDefinitionSummary = null;
            _autoMocker.Get<IUniversalGameRetriever>().Expect(mock => mock.GetBoardGameGeekGameSummary(_boardGameGeekGameDefinitionId, _currentUser))
                .Return(_expectedBoardGameGeekGameSummary);

            //--act
            var result = _autoMocker.ClassUnderTest.Details(_boardGameGeekGameDefinitionId, _currentUser) as ViewResult;

            //--assert
            var model = result.Model as UniversalGameDetailsViewModel;
            model.GamingGroupGameDefinitionSummary.ShouldBeNull();
        }
    }
}
