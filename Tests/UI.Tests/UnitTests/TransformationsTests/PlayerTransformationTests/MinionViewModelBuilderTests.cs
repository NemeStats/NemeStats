using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests
{
    [TestFixture]
    public class MinionViewModelBuilderTests
    {
        private MinionViewModelBuilder builder;
        private Player player;
        private MinionViewModel viewModelResult;

        [SetUp]
        public void SetUp()
        {
            builder = new MinionViewModelBuilder();
            player = new Player()
            {
                Id = 999,
                Name = "minion player name",
                Nemesis = new Nemesis()
                {
                    LossPercentage = 15,
                    NumberOfGamesLost = 150
                }
            };

            viewModelResult = builder.Build(player);
        }

        [Test]
        public void ItRequiresThatThePlayerHasNemesisDataPopulated()
        {
            Player player = new Player();
            MinionViewModelBuilder builder = new MinionViewModelBuilder();

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(player));

            Assert.AreEqual(MinionViewModelBuilder.EXCEPTION_NEMESIS_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItSetsTheMinionPlayerId()
        {
            Assert.AreEqual(player.Id, viewModelResult.MinionPlayerId);
        }

        [Test]
        public void ItSetsTheMinionPlayerName()
        {
            Assert.AreEqual(player.Name, viewModelResult.MinionName);
        }

        [Test]
        public void ItSetsTheWinPercentageVsThisMinion()
        {
            Assert.AreEqual(player.Nemesis.LossPercentage, viewModelResult.WinPercentageVersusMinion);
        }

        [Test]
        public void ItSetsTheNumberOfGamesWonVsThisMinion()
        {
            Assert.AreEqual(player.Nemesis.NumberOfGamesLost, viewModelResult.NumberOfGamesWonVersusMinion);
        }
    }
}
