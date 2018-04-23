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
using System.Linq;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.UniversalAnalyticsNemeStatsEventTrackerTests
{
    [TestFixture]
    public class TrackGamingGroupCreationTests : UniversalAnalyticsNemeStatsEventTrackerTestBase
    {
        [Test]
        public void ItSetsTheAnonymousClientId()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Equal(UniversalAnalyticsNemeStatsEventTracker.DEFAULT_ANONYMOUS_CLIENT_ID),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackGamingGroupCreation(TransactionSource.RestApi);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheEventCategoryToUsers()
        {
            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(EventCategoryEnum.GamingGroups.ToString()),
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackGamingGroupCreation(TransactionSource.RestApi);

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

            _autoMocker.ClassUnderTest.TrackGamingGroupCreation(TransactionSource.RestApi);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }

        [Test]
        public void ItSetsTheLabelToTheSpecifiedRegistrationSource()
        {
            TransactionSource registrationSource = TransactionSource.RestApi;

            _autoMocker.Get<IUniversalAnalyticsEventFactory>().Expect(mock => mock.MakeUniversalAnalyticsEvent(
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Anything,
                Arg<string>.Is.Equal(registrationSource.ToString()),
                Arg<string>.Is.Anything, 
                Arg<string>.Is.Anything,
                Arg<bool>.Is.Anything))
                .Return(analyticsEvent);

            _autoMocker.ClassUnderTest.TrackGamingGroupCreation(registrationSource);

            _autoMocker.Get<IEventTracker>().AssertWasCalled(mock => mock.TrackEvent(analyticsEvent));
        }
    }
}
