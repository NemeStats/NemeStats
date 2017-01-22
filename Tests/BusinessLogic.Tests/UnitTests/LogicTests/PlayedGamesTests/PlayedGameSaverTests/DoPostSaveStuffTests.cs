using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Events;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NSubstitute.Exceptions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class DoPostSaveStuffTests : PlayedGameSaverTestBase
    {
        [Test]
        public void ItRecalculatesTheNemesisOfEveryPlayerInTheGame()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerThreeId = 3;
            var playerGameResults = new List<PlayerGameResult>
            {
                new PlayerGameResult
                {
                    PlayerId = playerOneId,
                    GameRank = 1
                },
                new PlayerGameResult
                {
                    PlayerId = playerTwoId,
                    GameRank = 2
                },
                new PlayerGameResult
                {
                    PlayerId = playerThreeId,
                    GameRank = 3
                }
            };
            int playedGameId = 999;

            AutoMocker.ClassUnderTest.DoPostSaveStuff(TransactionSource.WebApplication, CurrentUser, playedGameId, GameDefinition.Id, playerGameResults, AutoMocker.Get<IDataContext>());

            foreach (var playerGameResult in playerGameResults)
            {
                AutoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(playerGameResult.PlayerId, CurrentUser, AutoMocker.Get<IDataContext>()));
            }
        }

        [Test]
        public void ItRecalculatesTheChampionForTheGameButDoesntClearTheExistingChampion()
        {
            //--arrange
            int playedGameId = 44;

            //--act
            AutoMocker.ClassUnderTest.DoPostSaveStuff(TransactionSource.WebApplication, CurrentUser, playedGameId, GameDefinition.Id, new List<PlayerGameResult>(), AutoMocker.Get<IDataContext>());

            //--assert
            AutoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion(GameDefinition.Id, CurrentUser, AutoMocker.Get<IDataContext>(), false));
        }

        [Test]
        public void ItRecordsAGamePlayedEvent()
        {
            //--arrange
            int playedGameId = 1;
            var transactionSource = TransactionSource.RestApi;

            //--act
            AutoMocker.ClassUnderTest.DoPostSaveStuff(transactionSource, CurrentUser, playedGameId, GameDefinition.Id, new List<PlayerGameResult>(), AutoMocker.Get<IDataContext>());
            
            //--assert
            AutoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayedGame(CurrentUser, transactionSource));
        }

        [Test]
        public void It_Sends_A_PlayedGameCreatedEvent()
        {
            //--arrange
            int playedGameId = 1;

            //--act
            AutoMocker.ClassUnderTest.DoPostSaveStuff(
                TransactionSource.WebApplication, 
                CurrentUser, 
                playedGameId, 
                GameDefinition.Id, 
                new List<PlayerGameResult>(), 
                AutoMocker.Get<IDataContext>());

            //--assert
            var arguments = AutoMocker.Get<IBusinessLogicEventSender>().GetArgumentsForCallsMadeOn(mock => mock.SendEvents(
                Arg<IList<IBusinessLogicEvent>>.Is.Anything));
            var actualEvents = arguments.AssertFirstCallIsType<IList<IBusinessLogicEvent>>(0);
            actualEvents.Count.ShouldBe(1);
            var playedGameCreatedEvent = actualEvents[0] as PlayedGameCreatedEvent;
            playedGameCreatedEvent.ShouldNotBeNull();
            playedGameCreatedEvent.TriggerEntityId.ShouldBe(playedGameId);
        }
    }
}
