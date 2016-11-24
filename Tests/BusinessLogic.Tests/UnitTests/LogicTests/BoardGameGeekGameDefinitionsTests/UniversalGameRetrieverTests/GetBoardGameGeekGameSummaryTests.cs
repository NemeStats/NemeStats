using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekGameDefinitionsTests.UniversalGameRetrieverTests
{
    [TestFixture]
    public class GetBoardGameGeekGameSummaryTests
    {
        private RhinoAutoMocker<UniversalGameRetriever> _autoMocker;

        private int _boardGameGeekGameDefinitionId = 1;
        private ApplicationUser _currentUser;
        private BoardGameGeekGameSummary _expectedBoardGameGeekGameSummary;
        private GameDefinitionSummary _expectedGameDefinitionSummary;
        private GameDefinition _expectedGameDefinition;
        private UniversalGameStats _expectedUniversalStats;
        private BoardGameGeekInfo _expectedBoardGameGeekInfo;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<UniversalGameRetriever>();
            _currentUser = new ApplicationUser
            {
                Id = "some user id"
            };

            _expectedGameDefinition = new GameDefinition
            {
                Id = 20,
                BoardGameGeekGameDefinitionId = _boardGameGeekGameDefinitionId
            };

            var gameDefinitionQueryable = new List<GameDefinition>
            {
                _expectedGameDefinition
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);

            _expectedBoardGameGeekInfo = new BoardGameGeekInfo();
            _autoMocker.Get<IBoardGameGeekGameDefinitionInfoRetriever>().Expect(mock => mock.GetResults(Arg<int>.Is.Anything)).Return(_expectedBoardGameGeekInfo);

            _expectedGameDefinitionSummary = new GameDefinitionSummary();

            _expectedUniversalStats = new UniversalGameStats();
            _autoMocker.Get<IUniversalStatsRetriever>().Expect(mock => mock.GetResults(Arg<int>.Is.Anything))
                .Return(_expectedUniversalStats);
        }

        [Test]
        public void It_Returns_The_BoardGameGeekInfo()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetBoardGameGeekGameSummary(_boardGameGeekGameDefinitionId, _currentUser);

            //--assert
            _autoMocker.Get<IBoardGameGeekGameDefinitionInfoRetriever>().AssertWasCalled(mock => mock.GetResults(_boardGameGeekGameDefinitionId));
            result.BoardGameGeekInfo.ShouldBeSameAs(_expectedBoardGameGeekInfo);
        }

        [Test]
        public void It_Returns_A_Null_GameDefinitionSummary_If_The_Current_User_Doesnt_Have_This_GameDefinition_In_Their_Gaming_Group()
        {
            //--arrange
            _currentUser.CurrentGamingGroupId = -1;

            //--act
            var result = _autoMocker.ClassUnderTest.GetBoardGameGeekGameSummary(_boardGameGeekGameDefinitionId, _currentUser);

            //--assert
            _autoMocker.Get<IGameDefinitionRetriever>().AssertWasNotCalled(mock => mock.GetGameDefinitionDetails(Arg<int>.Is.Anything, Arg<int>.Is.Anything));
            result.GamingGroupGameDefinitionSummary.ShouldBeNull();
        }

        [Test]
        public void It_Returns_A_GameDefinitionSummary_If_The_Current_Users_Gaming_Group_Has_This_Game_Definition()
        {
            //--arrange
            int numberOfGames = 6;
            _autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetGameDefinitionDetails(Arg<int>.Is.Anything, Arg<int>.Is.Anything))
                .Return(_expectedGameDefinitionSummary);

            //--act
            var result = _autoMocker.ClassUnderTest.GetBoardGameGeekGameSummary(_boardGameGeekGameDefinitionId, _currentUser, numberOfGames);

            //--assert
            _autoMocker.Get<IGameDefinitionRetriever>()
                .AssertWasCalled(
                    mock => mock.GetGameDefinitionDetails(Arg<int>.Is.Equal(_expectedGameDefinition.Id), Arg<int>.Is.Equal(numberOfGames)));
            result.GamingGroupGameDefinitionSummary.ShouldBe(_expectedGameDefinitionSummary);
        }

        [Test]
        public void It_Returns_The_Universal_Stats_For_The_Board_Game_Geek_Game()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetBoardGameGeekGameSummary(_boardGameGeekGameDefinitionId, _currentUser, 0);

            //--assert
            _autoMocker.Get<IUniversalStatsRetriever>()
                .AssertWasCalled(
                    mock => mock.GetResults(_boardGameGeekGameDefinitionId));
            result.UniversalGameStats.ShouldBe(_expectedUniversalStats);
        }
    }
}
