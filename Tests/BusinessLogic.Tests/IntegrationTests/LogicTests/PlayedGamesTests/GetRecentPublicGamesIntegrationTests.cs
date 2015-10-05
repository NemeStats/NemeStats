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
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGamesTests
{
    [TestFixture]
    public class GetRecentPublicGamesIntegrationTests : IntegrationTestBase
    {
        private const int NUMBER_OF_GAMES_TO_RETRIEVE = 3;

        List<PublicGameSummary> publicGameSummaryResults;

        [TestFixtureSetUp]
        public void LocalFixtureSetUp()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetriever retriever = new PlayedGameRetriever(dataContext);

                publicGameSummaryResults = retriever.GetRecentPublicGames(NUMBER_OF_GAMES_TO_RETRIEVE);
            }
        }

        [Test]
        public void ItReturnsTheSpecifiedNumberOfGames()
        {
            Assert.True(publicGameSummaryResults.Count == NUMBER_OF_GAMES_TO_RETRIEVE);
        }

        [Test]
        public void ItReturnsTheGamesOrderedByDatePlayedDescending()
        {
            DateTime lastPlayedDateTime = new DateTime(2099, 1, 1);

            foreach(PublicGameSummary summary in publicGameSummaryResults)
            {
                Assert.GreaterOrEqual(lastPlayedDateTime, summary.DatePlayed);
                lastPlayedDateTime = summary.DatePlayed;
            }
            Assert.True(publicGameSummaryResults.Count == NUMBER_OF_GAMES_TO_RETRIEVE);
        }
    }
}
