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
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.NemesesTests.NemesisHistoryRetrieverTests
{
    [TestFixture]
    public class GetNemesisHistoryTests
    {
        private IDataContext dataContextMock;
        private NemesisHistoryRetriever nemesisHistoryRetriever;
        private int playerIdUnderTest = 151;
        private int playerIdWithOneNemesisOnly = 999;
        private int playerIdOfCurrentNemesis = 1;
        private int playerIdOfPreviousNemesis = 2;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            nemesisHistoryRetriever = new NemesisHistoryRetriever(dataContextMock);

            List<Nemesis> nemeses = new List<Nemesis>();
            nemeses.Add(new Nemesis
            {
                Id = 4567,
                MinionPlayerId = playerIdWithOneNemesisOnly,
                MinionPlayer = new Player{ NemesisId = 4567 },
                NemesisPlayer = new Player(),
                DateCreated = new DateTime(2010, 1, 1)

            });
            nemeses.Add(new Nemesis
            {
                MinionPlayerId = playerIdUnderTest,
                MinionPlayer = new Player(),
                NemesisPlayer = new Player(),
                DateCreated = new DateTime(2011, 1, 1)
            });
            nemeses.Add(new Nemesis
            {
                MinionPlayerId = playerIdUnderTest,
                MinionPlayer = new Player{ Id = playerIdOfPreviousNemesis },
                NemesisPlayer = new Player(),
                DateCreated = new DateTime(2012, 1, 1)
            });
            nemeses.Add(new Nemesis
            {
                Id = 1234,
                MinionPlayerId = playerIdUnderTest,
                MinionPlayer = new Player { Id = playerIdOfCurrentNemesis, NemesisId = 1234 },
                NemesisPlayer = new Player(),
                DateCreated = new DateTime(2013, 1, 1)
            });

            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                           .Return(nemeses.AsQueryable());
        }

        [Test]
        public void ItReturnsAnEmptyListIfThePlayerHasNeverHadANemesis()
        {
            NemesisHistoryData historyData = nemesisHistoryRetriever.GetNemesisHistory(16531, 5);

            Assert.AreEqual(0, historyData.PreviousNemeses.Count);
        }

        [Test]
        public void ItReturnsAnEmptyListIfThePlayerOnlyHasACurrentNemesisButNoneOtherPriorToThisOne()
        {
            NemesisHistoryData historyData = nemesisHistoryRetriever.GetNemesisHistory(playerIdWithOneNemesisOnly, 5);

            Assert.AreEqual(0, historyData.PreviousNemeses.Count);
        }

        [Test]
        public void ItReturnsANullNemesisForTheCurrentNemesisIfThePlayerHasNoNemesis()
        {
            NemesisHistoryData historyData = nemesisHistoryRetriever.GetNemesisHistory(16531, 5);

            Assert.True(historyData.CurrentNemesis is NullNemesis);
        }

        [Test]
        public void ThePreviousNemesesAreForTheSpecifiedPlayer()
        {
            NemesisHistoryData historyData = nemesisHistoryRetriever.GetNemesisHistory(playerIdUnderTest, 5);

            Assert.True(historyData.PreviousNemeses.All(nemesis => nemesis.MinionPlayerId == playerIdUnderTest));
        }

        [Test]
        public void TheCurrentNemesisIsForTheSpecifiedPlayer()
        {
            NemesisHistoryData historyData = nemesisHistoryRetriever.GetNemesisHistory(playerIdUnderTest, 5);

            Assert.AreEqual(playerIdUnderTest, historyData.CurrentNemesis.MinionPlayerId);
        }

        [Test]
        public void ItPopulatesTheMinionPlayerData()
        {
            NemesisHistoryData historyData = nemesisHistoryRetriever.GetNemesisHistory(playerIdUnderTest, 5);

            Assert.NotNull(historyData.CurrentNemesis.MinionPlayer);
        }

        [Test]
        public void ItOnlyRetrievesTheSpecifiedNumberOfNemeses()
        {
            int numberOfHistoricalRecords = 1;
            NemesisHistoryData historyData = nemesisHistoryRetriever.GetNemesisHistory(playerIdUnderTest, numberOfHistoricalRecords);

            Assert.True(historyData.PreviousNemeses.All(nemesis => nemesis.MinionPlayerId == playerIdUnderTest));
            Assert.AreEqual(numberOfHistoricalRecords, historyData.PreviousNemeses.Count);
        }
    }
}
