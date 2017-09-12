using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Events;
using BusinessLogic.Events.Handlers;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using NUnit.Framework;
using Rhino.Mocks;
using RollbarSharp;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.EventsTests
{
    public class PlayedGameEventHandlerIntegrationTests : IntegrationTestBase
    {
        [Test]
        [Category("Integration")]
        [Explicit("this is a slow test and should be run manually so as to not slow down the normal unit test runs")]
        public void It_Doesnt_Get_Exceptions_While_Processing_Lots_Of_Events_For_The_Same_Player()
        {
            //--arrange

            //--act
            //--any playedGame from the integration test base should be sufficient to generate at least one achievement
            var playedGameEvent = new PlayedGameCreatedEvent(
                testPlayedGames[0].Id, 
                testPlayedGames[0].GameDefinitionId, 
                testPlayedGames[0].PlayerGameResults.Select(x => x.PlayerId).ToList(), 
                TransactionSource.RestApi,
                testUserWithDefaultGamingGroup);

            var taskList = new List<Task<bool>>();
            var dataContexts = new List<NemeStatsDataContext>();
            int NUMBER_OF_CALLS = 3;
            for (int i = 0; i < NUMBER_OF_CALLS; i++)
            {
                var dataContext = new NemeStatsDataContext();
                dataContexts.Add(dataContext);
            }

            for (int i = 0; i < NUMBER_OF_CALLS; i++)
            {
                var achievementsEventHandler = new PlayedGameEventHandler(
                    dataContexts[i], 
                    MockRepository.GenerateMock<INemeStatsEventTracker>(),
                    MockRepository.GenerateMock<IAchievementProcessor>(),
                    MockRepository.GenerateMock<IChampionRecalculator>(),
                    MockRepository.GenerateMock<INemesisRecalculator>(),
                    MockRepository.GenerateMock<IGamingGroupChampionRecalculator>(),
                    MockRepository.GenerateMock<IRollbarClient>());

                var lastTask = new Task<bool>(() => achievementsEventHandler.Handle(playedGameEvent));
                lastTask.Start();
                taskList.Add(lastTask);
            }

            try
            {
                if (taskList.Any(task => !task.Result))
                {
                    throw new AssertionException("There was an exception from one of the tasks.");
                }
            }
            finally
            {
                foreach (var dataContext in dataContexts)
                {
                    try
                    {
                        dataContext.Dispose();
                    }
                    catch (Exception)
                    {
                        //squish
                    }
                }
            }
        }
    }
}
