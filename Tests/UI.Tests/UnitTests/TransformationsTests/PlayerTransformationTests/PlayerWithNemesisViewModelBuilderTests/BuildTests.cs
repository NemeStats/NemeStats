using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Linq;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests.PlayerWithNemesisViewModelBuilderTests
{
    [TestFixture]
    public class BuildTests
    {
        private PlayerWithNemesisViewModelBuilder builder;
        private Player player;
        private int gamingGroupId = 1;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            builder = new PlayerWithNemesisViewModelBuilder();
            player = new Player
            {
                Id = 100,
                ApplicationUserId = "application user id",
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
                },
                GamingGroupId = gamingGroupId
            };

            currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = gamingGroupId
            };
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerIsNull()
        {
            Exception actualException = Assert.Throws<ArgumentNullException>(() => builder.Build(null, currentUser));

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

            Exception actualException = Assert.Throws<ArgumentException>(() => builder.Build(playerWithNoNemesisPlayer, currentUser));

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

            Exception actualException = Assert.Throws<ArgumentException>(() => builder.Build(playerWithNoPreviousNemesisSet, currentUser));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.AreEqual(player.Id, actualViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.AreEqual(player.Name, actualViewModel.PlayerName);
        }

        [Test]
        public void ItSetsThePlayerRegisteredFlagToTrueIfThePlayerHasAnApplicationUserId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.AreEqual(true, actualViewModel.PlayerRegistered);
        }

        [Test]
        public void ItSetsThePlayerRegisteredFlagToFalseIfThePlayerDoesNotHaveAnApplicationUserId()
        {
            player.ApplicationUserId = null;
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.AreEqual(false, actualViewModel.PlayerRegistered);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.AreEqual(player.Nemesis.NemesisPlayerId, actualViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.AreEqual(player.Nemesis.NemesisPlayer.Name, actualViewModel.NemesisPlayerName);
        }

        [Test]
        public void ItCopiesThePreviousNemesisPlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.AreEqual(player.PreviousNemesis.NemesisPlayerId, actualViewModel.PreviousNemesisPlayerId);
        }

        [Test]
        public void ItCopiesThePreviousNemesisPlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.AreEqual(player.PreviousNemesis.NemesisPlayer.Name, actualViewModel.PreviousNemesisPlayerName);
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.True(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(player, null);

            Assert.False(actualViewModel.UserCanEdit);
        }
    }
}
