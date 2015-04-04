using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    [TestFixture]
    public class PlayedGamesControllerTests
    {
        private RhinoAutoMocker<PlayedGamesController> autoMocker;
        private ApplicationUser applicationUser;
        private PlayedGameMessage playedGameMessage;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<PlayedGamesController>();

            applicationUser = new ApplicationUser
            {
                Id = "application user id"
            };

            playedGameMessage = new PlayedGameMessage
            {
                DatePlayed = "2015-04-10",
                GameDefinitionId = 1,
                Notes = "some notes"
            };
        }

        [Test]
        public void ItRecordsThePlayedGameWithTheTransactionSourceSetToRestApi()
        {
            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Equal(TransactionSource.RestApi),
                Arg<ApplicationUser>.Is.Same(this.applicationUser)));
        }

        [Test]
        public void ItRecordsTheDatePlayed()
        {
            DateTime expectedDateTime = new DateTime(2015, 4, 10);

            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.DatePlayed.Date == expectedDateTime.Date),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheDatePlayedToTodayIfItIsNotSpecified()
        {
            playedGameMessage.DatePlayed = null;

            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.DatePlayed.Date == DateTime.UtcNow.Date),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheGameDefinitionId()
        {
            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.GameDefinitionId == playedGameMessage.GameDefinitionId),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheNotes()
        {
            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.Notes == playedGameMessage.Notes),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsThePlayerRanks()
        {
            playedGameMessage.PlayerRanks = new List<PlayerRank>
            {
                new PlayerRank() { GameRank = 1, PlayerId = 100 },
                new PlayerRank() { GameRank = 2, PlayerId = 200 }
            };
            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.Notes == playedGameMessage.Notes),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        //TODO DatePlayed defaults to today if not set
        //TODO record the transaction source as REST API
    }
}
