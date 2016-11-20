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
        private CacheableGameData _expectedCacheableData;
        private BoardGameGeekGameSummary _expectedBoardGameGeekGameSummary;
        private GameDefinitionSummary _expectedGameDefinitionSummary;
        private GameDefinition _expectedGameDefinition;

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

            _expectedCacheableData = new CacheableGameData();
            _autoMocker.Get<ICacheableGameDataRetriever>().Expect(mock => mock.GetResults(Arg<int>.Is.Anything)).Return(_expectedCacheableData);

            _expectedBoardGameGeekGameSummary = new BoardGameGeekGameSummary();
            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<BoardGameGeekGameSummary>(Arg<CacheableGameData>.Is.Anything))
                .Return(_expectedBoardGameGeekGameSummary);

            _expectedGameDefinitionSummary = new GameDefinitionSummary();
        }

        [Test]
        public void It_Returns_Board_Game_Geek_Summary_Data()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetBoardGameGeekGameSummary(_boardGameGeekGameDefinitionId, _currentUser);

            //--assert
            _autoMocker.Get<ICacheableGameDataRetriever>().AssertWasCalled(mock => mock.GetResults(_boardGameGeekGameDefinitionId));
            _autoMocker.Get<ITransformer>().AssertWasCalled(mock => mock.Transform<BoardGameGeekGameSummary>(_expectedCacheableData));
            result.ShouldBeSameAs(_expectedBoardGameGeekGameSummary);
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
    }
}
