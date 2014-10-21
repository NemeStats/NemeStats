using BusinessLogic.Models;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.NemesisTests
{
    [TestFixture]
    public class GetHashCodeTests
    {
        [Test]
        public void ANewNemesisDoesNotShareAHashCodeWithANewNonNemesisObject()
        {
            Nemesis nemesis1 = new Nemesis();
            Assert.False(nemesis1.GetHashCode() == (new object()).GetHashCode());
        }

        [Test]
        public void NewObjectsHaveTheSameHashCode()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis();

            Assert.True(nemesis1.GetHashCode() == nemesis2.GetHashCode());
        }

        [Test]
        public void TheHashCodeIsDifferentIfTheMinionPlayerIdsAreDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { MinionPlayerId = 1 };

            Assert.False(nemesis1.GetHashCode() == nemesis2.GetHashCode());
        }

        [Test]
        public void TheHashCodeIsDifferentIfTheNemesisPlayerIdsAreDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { NemesisPlayerId = 1 };

            Assert.False(nemesis1.GetHashCode() == nemesis2.GetHashCode());
        }

        [Test]
        public void TheHashCodeIsDifferentIfTheNumberOfGamesLostAreDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { NumberOfGamesLost = 1 };

            Assert.False(nemesis1.GetHashCode() == nemesis2.GetHashCode());
        }

        [Test]
        public void TheHashCodeIsDifferentIfTheLossPercentagesAreDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { LossPercentage = 1 };

            Assert.False(nemesis1.GetHashCode() == nemesis2.GetHashCode());
        }
    }
}
