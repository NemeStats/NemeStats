using BusinessLogic.DataAccess;
using BusinessLogic.Logic.BoardGameGeek;
using NUnit.Framework;
using StructureMap.AutoMocking;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.Exceptions;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.BoardGameGeekGameDefinitionAttacherTests
{
    [TestFixture]
    public class CreateBoardGameGeekGameDefinitionTests
    {
        private RhinoAutoMocker<BoardGameGeekGameDefinitionCreator> autoMocker;
        private int boardGameGeekGameDefinitionId = 1;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<BoardGameGeekGameDefinitionCreator>();
            currentUser = new ApplicationUser();
        }

        [Test]
        public void ItDoesntCreateANewRecordIfOneAlreadyExists()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId))
                .Return(new BoardGameGeekGameDefinition());

            autoMocker.ClassUnderTest.CreateBoardGameGeekGameDefinition(boardGameGeekGameDefinitionId, currentUser);

            autoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.Save(
                Arg<BoardGameGeekGameDefinition>.Is.Anything, 
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItDoesntCreateANewRecordIfTheBoardGameGeekApiDoesntReturnAResult()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId))
               .Throw(new EntityDoesNotExistException(typeof(BoardGameGeekGameDefinition), boardGameGeekGameDefinitionId)); 
            autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(boardGameGeekGameDefinitionId))
                .Return(null);

            autoMocker.ClassUnderTest.CreateBoardGameGeekGameDefinition(boardGameGeekGameDefinitionId, currentUser);

            autoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.Save(
                Arg<BoardGameGeekGameDefinition>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItCreatesANewRecordIfOneDoesNotExist()
        {
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId))
                .Throw(new EntityDoesNotExistException(typeof(BoardGameGeekGameDefinition), boardGameGeekGameDefinitionId));
            var expectedGameDetails = new GameDetails
            {
                Thumbnail = "some thumbnail",
                Name = "some name"
            };
            autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(boardGameGeekGameDefinitionId))
                .Return(expectedGameDetails);

            autoMocker.ClassUnderTest.CreateBoardGameGeekGameDefinition(boardGameGeekGameDefinitionId, currentUser);

            var args = autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(
                Arg<BoardGameGeekGameDefinition>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));

            Assert.That(args.Count, Is.GreaterThan(0));
            var actualBoardGameGeekGameDefinition = args[0][0] as BoardGameGeekGameDefinition;
            Assert.That(actualBoardGameGeekGameDefinition, Is.Not.Null);
            Assert.That(actualBoardGameGeekGameDefinition.Id, Is.EqualTo(boardGameGeekGameDefinitionId));
            Assert.That(actualBoardGameGeekGameDefinition.Thumbnail, Is.EqualTo(expectedGameDetails.Thumbnail));
            Assert.That(actualBoardGameGeekGameDefinition.Name, Is.EqualTo(expectedGameDetails.Name));
        }
    }
}
