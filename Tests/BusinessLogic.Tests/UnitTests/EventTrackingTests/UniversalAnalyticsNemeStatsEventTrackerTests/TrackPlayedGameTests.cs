#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using NUnit.Framework;
using Rhino.Mocks;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.UniversalAnalyticsNemeStatsEventTrackerTests
{
    [TestFixture]
    public class TrackPlayedGameTests : UniversalAnalyticsNemeStatsEventTrackerTestBase
    {
        [Test]
        public void ItSetsTheAnonymousClientId()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Equal(currentUser.AnonymousClientId),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackPlayedGame(currentUser, TransactionSource.WebApplication);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventCategoryToPlayedGames()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventCategoryEnum.PlayedGames.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);
            _autoMocker.ClassUnderTest.TrackPlayedGame(currentUser, TransactionSource.WebApplication);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventActionToCreated()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventActionEnum.Created.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackPlayedGame(currentUser, TransactionSource.WebApplication);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventLabelToTheTransactionSource()
        {
            TransactionSource transactionSource = TransactionSource.RestApi;
            
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(transactionSource.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackPlayedGame(currentUser, transactionSource);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheNonInteractionFlag()
        {
            TransactionSource transactionSource = TransactionSource.RestApi;

            _autoMocker.ClassUnderTest.Expect(mock => mock.IsNonInteractionEvent(transactionSource))
                .Return(true);

            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                    Arg<string>.Is.Anything,
                    Arg<string>.Is.Anything,
                    Arg<string>.Is.Anything,
                    Arg<string>.Is.Anything,
                    Arg<string>.Is.Anything,
                    Arg<string>.Is.Anything,
                    Arg<bool>.Is.Equal(true)))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackPlayedGame(currentUser, transactionSource);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }
    }
}
