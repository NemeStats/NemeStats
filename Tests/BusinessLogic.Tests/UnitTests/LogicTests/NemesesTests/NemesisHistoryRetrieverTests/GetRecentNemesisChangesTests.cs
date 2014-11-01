using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.NemesesTests.NemesisHistoryRetrieverTests
{
    [TestFixture]
    public class GetRecentNemesisChangesTests
    {
        private IDataContext dataContextMock;
        private NemesisHistoryRetriever nemesisHistoryRetriever;
        private int repeatedMinionPlayerId = 10;
        private int nemesisPlayerId1 = 1;
        private string nemesisPlayerName1 = "nemesis player name 1";
        private int minionPlayerId1 = 200;
        private string repeatedMinionPlayerName1 = "minion player name 1";
        private int lossPercentage = 51;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            nemesisHistoryRetriever = new NemesisHistoryRetriever(dataContextMock);

            List<Nemesis> nemeses = new List<Nemesis>
            {
                BuildNemesisItem(nemesisPlayerId1, nemesisPlayerName1, repeatedMinionPlayerId, repeatedMinionPlayerName1, lossPercentage, 0),
                BuildNemesisItem(35, "another nemesis player name", repeatedMinionPlayerId, repeatedMinionPlayerName1, 59, 1),
                BuildNemesisItem(36, "nemesis3", 515, "minion3", 59, 3),
                BuildNemesisItem(37, "nemesis4", 516, "minion4", 100, 2),
                BuildNemesisItem(39, "nemesis5", 515116, "minion4", 70, 5)

            };

            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                           .Return(nemeses.AsQueryable());
        }

        private static Nemesis BuildNemesisItem(
            int nemesisPlayerId, 
            string nemesisPlayerName, 
            int minionPlayerId, 
            string minionPlayerName, 
            int lossPercentage,
            int daysAgo)
        {
            return new Nemesis
            {
                NemesisPlayerId = nemesisPlayerId,
                NemesisPlayer = new Player
                {
                    Name = nemesisPlayerName
                },
                MinionPlayerId = minionPlayerId,
                MinionPlayer = new Player
                {
                    Name = minionPlayerName
                },
                LossPercentage = lossPercentage,
                DateCreated = DateTime.UtcNow.AddDays(-1 * daysAgo)
            };
        }

        [Test]
        public void ItReturnsTheSpecifiedNumberOfNemesisChanges()
        {
            int numberOfNemesesToRetrieve = 3;

            List<NemesisChange> nemesisChanges = nemesisHistoryRetriever.GetRecentNemesisChanges(numberOfNemesesToRetrieve);

            Assert.AreEqual(numberOfNemesesToRetrieve, nemesisChanges.Count);
        }

        [Test]
        public void ItDoesntReturnTheSameMinionPlayerMoreThanOnce()
        {
            List<NemesisChange> nemesisChanges = nemesisHistoryRetriever.GetRecentNemesisChanges(4);

            Assert.AreEqual(1, nemesisChanges.Count(nemesis => nemesis.MinionPlayerId == repeatedMinionPlayerId));
        }

        [Test]
        public void ItReturnsNemesisChangesInOrderOfDateDescending()
        {
            List<NemesisChange> nemesisChanges = nemesisHistoryRetriever.GetRecentNemesisChanges(4);

            var sorted = nemesisChanges.OrderByDescending(s => s.DateCreated);
            CollectionAssert.AreEqual(sorted.ToList(), nemesisChanges);
        }

        [Test]
        public void ItSetsAllTheFields()
        {
            List<NemesisChange> nemesisChanges = nemesisHistoryRetriever.GetRecentNemesisChanges(1);

            NemesisChange nemesisChange = nemesisChanges[0];
            Assert.AreEqual(repeatedMinionPlayerId, nemesisChange.MinionPlayerId);
            Assert.AreEqual(repeatedMinionPlayerName1, nemesisChange.MinionPlayerName);
            Assert.AreEqual(nemesisPlayerId1, nemesisChange.NemesisPlayerId);
            Assert.AreEqual(nemesisPlayerName1, nemesisChange.NemesisPlayerName);
            Assert.AreEqual(lossPercentage, nemesisChange.LossPercentageVersusNemesis);
            Assert.AreEqual(DateTime.UtcNow.Date, nemesisChange.DateCreated.Date);
        }
    }
}
