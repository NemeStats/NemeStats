using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests.PlayerWithNemesisViewModelBuilderTests
{
    [TestFixture]
    public class BuildTests
    {
        private PlayerWithNemesisViewModelBuilder builder;
        private Player player;

        [SetUp]
        public void SetUp()
        {
            builder = new PlayerWithNemesisViewModelBuilder();
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerIsNull()
        {
            Exception actualException = Assert.Throws<ArgumentNullException>(() => builder.Build(null));

            Assert.AreEqual(new ArgumentNullException("player").Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfThePlayerHasANemesisButTheNemesisPlayerIsNull()
        {
            Player player = new Player()
            {
                Nemesis = new Nemesis()
            };
            Exception expectedException = new ArgumentException(PlayerWithNemesisViewModelBuilder.EXCEPTION_MESSAGE_NEMESIS_PLAYER_CANNOT_BE_NULL);
            Exception actualException = Assert.Throws<ArgumentException>(() => builder.Build(player));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Player player = new Player() { Id = 1 };

            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.Id, actualViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            Player player = new Player() { Name = "player name" };

            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.Name, actualViewModel.PlayerName);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerId()
        {
            Player player = new Player();
            player.Nemesis = new Nemesis();
            player.Nemesis.NemesisPlayerId = 123;
            player.Nemesis.NemesisPlayer = new Player();

            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.Nemesis.NemesisPlayerId, actualViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerName()
        {
            Player player = new Player();
            player.Nemesis = new Nemesis();
            player.Nemesis.NemesisPlayer = new Player() { Name = "nemesis player name" };

            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.Nemesis.NemesisPlayer.Name, actualViewModel.NemesisPlayerName);
        }
    }
}
