using BusinessLogic.Models.Players;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTests.TopPlayerViewModelBuilderTests
{
    [TestFixture]
    public class BuildTests
    {
        private TopPlayerViewModelBuilder builder;
        private TopPlayer topPlayer;
        private TopPlayerViewModel topPlayerViewModel;

        [SetUp]
        public void SetUp()
        {
            builder = new TopPlayerViewModelBuilder();
            topPlayer = new TopPlayer()
            {
                PlayerId = 1,
                PlayerName = "player name",
                TotalNumberOfGamesPlayed = 100,
                TotalPoints = 1234,
                WinPercentage = 75
            };

            topPlayerViewModel = builder.Build(topPlayer);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Assert.AreEqual(topPlayer.PlayerId, topPlayerViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            Assert.AreEqual(topPlayer.PlayerName, topPlayerViewModel.PlayerName);
        }

        [Test]
        public void ItCopiesTheTotalNumberOfGamesPlayed()
        {
            Assert.AreEqual(topPlayer.TotalNumberOfGamesPlayed, topPlayerViewModel.TotalNumberOfGamesPlayed);
        }

        [Test]
        public void ItCopiesTheTotalPoints()
        {
            Assert.AreEqual(topPlayer.TotalPoints, topPlayerViewModel.TotalPoints);
        }

        [Test]
        public void ItCopiesTheWinPercentage()
        {
            Assert.AreEqual(topPlayer.WinPercentage, topPlayerViewModel.WinPercentage);
        }
    }
}
