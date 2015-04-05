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
using BusinessLogic.Tests.UnitTests.EventTrackingTests.UniversalAnalyticsNemeStatsEventTrackerTests;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.PlayedGameTrackerTests
{
    [TestFixture]
    public class TrackPlayedGameTests : UniversalAnalyticsNemeStatsEventTrackerTestBase
    {
        [Test]
        public void ItSetsTheAnonymousClientId()
        {
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Equal(currentUser.AnonymousClientId),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackPlayedGame(currentUser, TransactionSource.WebApplication);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventCategoryToPlayedGames()
        {
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventCategoryEnum.PlayedGames.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);
            tracker.TrackPlayedGame(currentUser, TransactionSource.WebApplication);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventActionToCreated()
        {
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventActionEnum.Created.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackPlayedGame(currentUser, TransactionSource.WebApplication);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventLabelToTheTransactionSource()
        {
            TransactionSource transactionSource = TransactionSource.RestApi;
            
            eventFactoryMock.Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(transactionSource.ToString()),
                Arg<string>.Is.Anything))
                .Return(analyticsEvent);

            tracker.TrackPlayedGame(currentUser, transactionSource);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }
    }
}
