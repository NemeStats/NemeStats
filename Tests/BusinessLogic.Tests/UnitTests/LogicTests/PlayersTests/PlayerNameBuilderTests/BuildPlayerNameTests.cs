using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Players;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerNameBuilderTests
{
    [TestFixture]
    public class BuildPlayerNameTests
    {
        [Test]
        public void ItDoesntChangeTheNameIfThePlayerIsActive()
        {
            string playerName = "some player name";
            var result = PlayerNameBuilder.BuildPlayerName(playerName, true);

            Assert.That(result, Is.EqualTo(playerName));
        }

        [Test]
        public void ItAddsAnInactiveSuffixToThePlayerNameIfThePlayerIsInactive()
        {
            string playerName = "x";
            var result = PlayerNameBuilder.BuildPlayerName(playerName, false);

            Assert.That(result, Is.EqualTo("x (INACTIVE)"));
        }
    }
}
