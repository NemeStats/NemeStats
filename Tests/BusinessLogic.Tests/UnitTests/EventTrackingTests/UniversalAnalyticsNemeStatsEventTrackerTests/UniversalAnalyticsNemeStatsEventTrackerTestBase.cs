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
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Tests.UnitTests.EventTrackingTests.UniversalAnalyticsNemeStatsEventTrackerTests
{
    [TestFixture]
    public class UniversalAnalyticsNemeStatsEventTrackerTestBase
    {
        protected IEventTracker eventTrackerMock;
        protected IUniversalAnalyticsEventFactory eventFactoryMock;
        protected IUniversalAnalyticsEvent analyticsEvent;
        protected UniversalAnalyticsNemeStatsEventTracker tracker;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            eventTrackerMock = MockRepository.GenerateMock<IEventTracker>();
            eventFactoryMock = MockRepository.GenerateMock<IUniversalAnalyticsEventFactory>();
            analyticsEvent = MockRepository.GenerateMock<IUniversalAnalyticsEvent>();

            tracker = new UniversalAnalyticsNemeStatsEventTracker(eventTrackerMock, eventFactoryMock);
            currentUser = new ApplicationUser()
            {
                AnonymousClientId = "anonymous id"
            };
        }
    }
}
