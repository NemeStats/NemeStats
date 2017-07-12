using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Events;
using BusinessLogic.Events.Handlers;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.EventsTests
{
    [TestFixture]
    public class PlayedGameEventHandlerTests
    {
        private RhinoAutoMocker<PlayedGameEventHandler> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<PlayedGameEventHandler>();
        }

        [Test]
        public void It_Processes_Analytics_Events_Then_Badges_Then_Champions_Then_Nemeses_Then_Achievements()
        {
            //--arrange
            var playersInGame = new List<int> {10, 11};
            var playedGameEvent = new PlayedGameCreatedEvent(1, 2, playersInGame, TransactionSource.RestApi);

            var anonymousApplicationUser = new AnonymousApplicationUser();

            //--act
            _autoMocker.ClassUnderTest.Handle(playedGameEvent);

            //--assert

            //--make sure event tracking was called but not yet the champion recalculator
            _autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayedGame(
                Arg<ApplicationUser>.Is.Equal(anonymousApplicationUser),
                Arg<TransactionSource>.Is.Equal(playedGameEvent.TransactionSource)),
                options => options.WhenCalled(y => _autoMocker.Get<IChampionRecalculator>().AssertWasNotCalled(
                    mock => mock.RecalculateChampion(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything, Arg<IDataContext>.Is.Anything))));

            //--make sure the champion recalculator was called but not yet the nemesis recalculator
            _autoMocker.Get<IChampionRecalculator>().AssertWasCalled(
                mock => mock.RecalculateChampion(
                    Arg<int>.Is.Equal(playedGameEvent.GameDefinitionId),
                    Arg<ApplicationUser>.Is.Equal(anonymousApplicationUser),
                    Arg<IDataContext>.Is.Anything,
                    Arg<bool>.Is.Anything),
                    options => options.WhenCalled(y => _autoMocker.Get<INemesisRecalculator>().AssertWasNotCalled(
                        mock => mock.RecalculateNemesis(Arg<int>.Is.Equal(playersInGame[0]),
                        Arg<ApplicationUser>.Is.Equal(anonymousApplicationUser),
                        Arg<IDataContext>.Is.Anything))));

            //--make sure the nemesis recalculator was called for the first player in the game, but not yet the achievement processor
            _autoMocker.Get<INemesisRecalculator>().AssertWasCalled(
                mock => mock.RecalculateNemesis(
                    Arg<int>.Is.Equal(playersInGame[0]), 
                    Arg<ApplicationUser>.Is.Equal(anonymousApplicationUser), 
                    Arg<IDataContext>.Is.Anything),
                    options => options.WhenCalled(y => _autoMocker.Get<IAchievementProcessor>().AssertWasNotCalled(
                        mock => mock.ProcessAchievements(playedGameEvent.TriggerEntityId))));

            //--make sure the nemesis recalculator was called for the second player in the game
            _autoMocker.Get<INemesisRecalculator>().AssertWasCalled(
                mock => mock.RecalculateNemesis(
                    Arg<int>.Is.Equal(playersInGame[1]),
                    Arg<ApplicationUser>.Is.Equal(anonymousApplicationUser),
                    Arg<IDataContext>.Is.Anything));

            //--make sure the achievement processor gets called
            _autoMocker.Get<IAchievementProcessor>().AssertWasCalled(
                mock => mock.ProcessAchievements(playedGameEvent.TriggerEntityId));
        }
    }
}
