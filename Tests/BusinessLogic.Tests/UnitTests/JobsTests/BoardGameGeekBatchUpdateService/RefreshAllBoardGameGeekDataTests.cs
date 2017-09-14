using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.DataAccess;
using BusinessLogic.Jobs.BoardGameGeekBatchUpdate;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.JobsTests.BoardGameGeekBatchUpdateService
{
    [TestFixture]
    public class RefreshAllBoardGameGeekDataTests
    {
        private RhinoAutoMocker<BoardGameGeekBatchUpdateJobService> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<BoardGameGeekBatchUpdateJobService>();  
        }

        [Test]
        public void ItRefreshesEveryExistingBoardGameGeekGameDefinition()
        {
            //--arrange
            int expectedBoardGameGeekGameDefinitionId1 = 1;
            int expectedBoardGameGeekGameDefinitionId2 = 2;

            var allBoardGameGeekGameDefinitionsQueryable = new List<BoardGameGeekGameDefinition>
            {
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinitionId1
                },
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinitionId2
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>())
                .Return(allBoardGameGeekGameDefinitionsQueryable);
            var expectedBggGameDetails1 = new GameDetails
            {
                GameId = expectedBoardGameGeekGameDefinitionId1,
                AverageWeight = 1,
                Description = "some description 1",
                Thumbnail = "some thumbnail",
                MinPlayers = 3,
                MaxPlayers = 4,
                MinPlayTime = 5,
                MaxPlayTime = 6
            };
            _autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(expectedBoardGameGeekGameDefinitionId1))
                       .Return(expectedBggGameDetails1);
            var expectedBggGameDetails2 = new GameDetails
            {
                GameId = expectedBoardGameGeekGameDefinitionId2,
                AverageWeight = 10,
                Description = "some description 2",
                Thumbnail = "some thumbnail",
                MinPlayers = 30,
                MaxPlayers = 40,
                MinPlayTime = 50,
                MaxPlayTime = 60
            };
            _autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(expectedBoardGameGeekGameDefinitionId2))
                       .Return(expectedBggGameDetails2);

            //--act
            _autoMocker.ClassUnderTest.RefreshAllBoardGameGeekData();

            //--assert
            AssertBoardGameGeekGameDefinitionSaved(expectedBggGameDetails1);
            AssertBoardGameGeekGameDefinitionSaved(expectedBggGameDetails2);
        }

        private void AssertBoardGameGeekGameDefinitionSaved(GameDetails expectedGameDetails)
        {
            var argumentsForCallsMadeOn = _autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(
                                                                                                     mock => mock.AdminSave(Arg<BoardGameGeekGameDefinition>.Is.Anything));
            Assert.That(argumentsForCallsMadeOn, Is.Not.Null);
            BoardGameGeekGameDefinition actualBoardGameGeekGameDefinition = null;
            foreach (var call in argumentsForCallsMadeOn)
            {
                actualBoardGameGeekGameDefinition = call[0] as BoardGameGeekGameDefinition;
                Assert.That(actualBoardGameGeekGameDefinition, Is.Not.Null);

                if (actualBoardGameGeekGameDefinition.Id == expectedGameDetails.GameId)
                {
                    break;
                }
            }

            Assert.That(actualBoardGameGeekGameDefinition, Is.Not.Null);
            Assert.That(actualBoardGameGeekGameDefinition.AverageWeight, Is.EqualTo(expectedGameDetails.AverageWeight));
            Assert.That(actualBoardGameGeekGameDefinition.Description, Is.EqualTo(expectedGameDetails.Description));
            Assert.That(actualBoardGameGeekGameDefinition.MaxPlayTime, Is.EqualTo(expectedGameDetails.MaxPlayTime));
            Assert.That(actualBoardGameGeekGameDefinition.MinPlayTime, Is.EqualTo(expectedGameDetails.MinPlayTime));
            Assert.That(actualBoardGameGeekGameDefinition.MaxPlayers, Is.EqualTo(expectedGameDetails.MaxPlayers));
            Assert.That(actualBoardGameGeekGameDefinition.MinPlayers, Is.EqualTo(expectedGameDetails.MinPlayers));
            Assert.That(actualBoardGameGeekGameDefinition.Name, Is.EqualTo(expectedGameDetails.Name));
            Assert.That(actualBoardGameGeekGameDefinition.Thumbnail, Is.EqualTo(expectedGameDetails.Thumbnail));
        }

        [Test]
        public void ItSkipsUpdatingRecordsThatNoLongerHaveAMatchingRecordInBoardGameGeek()
        {
            //--arrange
            int expectedBoardGameGeekGameDefinitionId1 = 1;

            var allBoardGameGeekGameDefinitionsQueryable = new List<BoardGameGeekGameDefinition>
            {
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinitionId1
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>())
                .Return(allBoardGameGeekGameDefinitionsQueryable);

            //--act
            _autoMocker.ClassUnderTest.RefreshAllBoardGameGeekData();

            //--assert
            _autoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.Save(
                Arg<BoardGameGeekGameDefinition>.Is.Anything, 
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItReturnsTheTotalNumberOfBoardGameGeekGameDefinitionsThatWereSuccessfullyUpdated()
        {
            //--arrange
            int expectedBoardGameGeekGameDefinitionId1 = 1;
            int expectedBoardGameGeekGameDefinitionId2 = 2;

            var allBoardGameGeekGameDefinitionsQueryable = new List<BoardGameGeekGameDefinition>
            {
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinitionId1
                },
                new BoardGameGeekGameDefinition
                {
                    Id = expectedBoardGameGeekGameDefinitionId2
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>())
                .Return(allBoardGameGeekGameDefinitionsQueryable);
            
            _autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(expectedBoardGameGeekGameDefinitionId1))
                       .Return(new GameDetails());
            _autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(expectedBoardGameGeekGameDefinitionId2))
                       .Return(new GameDetails());

            //--act
            var actualNumberUpdated = _autoMocker.ClassUnderTest.RefreshAllBoardGameGeekData();

            //--assert
            Assert.That(actualNumberUpdated, Is.EqualTo(2));
        }
    }
}
