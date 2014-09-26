using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.NemesisTests
{
    [TestFixture]
    public class SameNemesisTests
    {
        [Test]
        public void NewObjectsAreTheSame()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis();

            Assert.True(nemesis1.SameNemesis(nemesis2));
        }

        [Test]
        public void ANullObjectIsNotTheSame()
        {
            Nemesis nemesis1 = new Nemesis();

            Assert.False(nemesis1.SameNemesis(null));
        }

        [Test]
        public void ItReturnsFalseIfTheNemesisPlayerIdIsDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { NemesisPlayerId = 1 };

            Assert.False(nemesis1.SameNemesis(nemesis2));
        }

        [Test]
        public void ItReturnsFalseIfTheMinionPlayerIdIsDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { MinionPlayerId = 1 };

            Assert.False(nemesis1.SameNemesis(nemesis2));
        }
    }
}
