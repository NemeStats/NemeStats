using BusinessLogic.Models;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.NemesisTests
{
    [TestFixture]
    public class EqualsTests
    {
        [Test]
        public void ComparingToAnObjectWhichIsNotANemesisReturnsFalse()
        {
            Nemesis nemesis1 = new Nemesis();
            Assert.False(nemesis1.Equals(new object()));
        }

        [Test]
        public void NewObjectsAreEqual()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis();

            Assert.True(nemesis1.Equals(nemesis2));
        }

        [Test]
        public void ANullObjectIsNotEqual()
        {
            Nemesis nemesis1 = new Nemesis();

            Assert.False(nemesis1.Equals(null));
        }

        [Test]
        public void ItReturnsFalseIfTheMinionPlayerIdsAreDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { MinionPlayerId = 1 };

            Assert.False(nemesis1.Equals(nemesis2));
        }

        [Test]
        public void ItReturnsFalseIfTheNemesisPlayerIdsAreDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { NemesisPlayerId = 1 };

            Assert.False(nemesis1.Equals(nemesis2));
        }

        [Test]
        public void ItReturnsFalseIfTheNumberOfGamesLostAreDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { NumberOfGamesLost = 1 };

            Assert.False(nemesis1.Equals(nemesis2));
        }

        [Test]
        public void ItReturnsFalseIfTheLossPercentageIsDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { LossPercentage = 1 };

            Assert.False(nemesis1.Equals(nemesis2));
        }
    }
}
