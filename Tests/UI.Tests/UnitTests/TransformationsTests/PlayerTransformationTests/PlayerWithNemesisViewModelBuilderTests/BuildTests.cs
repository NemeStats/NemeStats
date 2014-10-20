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
            player = new Player
            {
                Id = 100,
                Name = "player name",
                Nemesis = new Nemesis
                {
                    NemesisPlayerId = 1,
                    NemesisPlayer = new Player
                    {
                        Name = "current nemesis player name"
                    }
                },
                PreviousNemesis = new Nemesis
                {
                    NemesisPlayerId = 2,
                    NemesisPlayer = new Player
                    {
                        Name = "previous nemesis player name"
                    }
                }
            };
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
            Player playerWithNoNemesisPlayer = new Player()
            {
                Nemesis = new Nemesis()
            };
            Exception expectedException = new ArgumentException(
                PlayerWithNemesisViewModelBuilder.EXCEPTION_MESSAGE_NEMESIS_PLAYER_CANNOT_BE_NULL);

            Exception actualException = Assert.Throws<ArgumentException>(() => builder.Build(playerWithNoNemesisPlayer));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfThePlayerHasAPreviousNemesisButTheNemesisPlayerIsNull()
        {
            Player playerWithNoPreviousNemesisSet = new Player()
            {
                PreviousNemesis = new Nemesis()
            };
            Exception expectedException = new ArgumentException(
                PlayerWithNemesisViewModelBuilder.EXCEPTION_MESSAGE_PREVIOUS_NEMESIS_PLAYER_CANNOT_BE_NULL);

            Exception actualException = Assert.Throws<ArgumentException>(() => builder.Build(playerWithNoPreviousNemesisSet));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.Id, actualViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.Name, actualViewModel.PlayerName);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.Nemesis.NemesisPlayerId, actualViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.Nemesis.NemesisPlayer.Name, actualViewModel.NemesisPlayerName);
        }

        [Test]
        public void ItCopiesThePreviousNemesisPlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.PreviousNemesis.NemesisPlayerId, actualViewModel.PreviousNemesisPlayerId);
        }

        [Test]
        public void ItCopiesThePreviousNemesisPlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player);

            Assert.AreEqual(player.PreviousNemesis.NemesisPlayer.Name, actualViewModel.PreviousNemesisPlayerName);
        }
    }
}
