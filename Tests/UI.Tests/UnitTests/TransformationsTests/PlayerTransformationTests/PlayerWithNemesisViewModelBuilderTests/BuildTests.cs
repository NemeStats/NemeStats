using BusinessLogic.Models;
using BusinessLogic.Models.Players;
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
        private PlayerWithNemesis playerWithNemesis;
        private int gamingGroupId = 1;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            builder = new PlayerWithNemesisViewModelBuilder();
            this.playerWithNemesis = new PlayerWithNemesis
            {
                PlayerId = 100,
                PlayerRegistered = true,
                PlayerName = "player name",
                NemesisPlayerId = 300,
                NemesisPlayerName = "nemesis player name",
                PreviousNemesisPlayerId = 400,
                PreviousNemesisPlayerName = "previous nemesis player name",
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
            Exception actualException = Assert.Throws<ArgumentNullException>(() => builder.Build(null, this.currentUser));

            Assert.AreEqual(new ArgumentNullException("player").Message, actualException.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.AreEqual(this.playerWithNemesis.PlayerId, actualViewModel.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.AreEqual(this.playerWithNemesis.PlayerName, actualViewModel.PlayerName);
        }

        [Test]
        public void ItSetsThePlayerRegisteredFlagToTrueIfThePlayerHasAnApplicationUserId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.AreEqual(true, actualViewModel.PlayerRegistered);
        }

        [Test]
        public void ItCopiesThePlayerRegisteredFlag()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.AreEqual(this.playerWithNemesis.PlayerRegistered, actualViewModel.PlayerRegistered);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.AreEqual(this.playerWithNemesis.NemesisPlayerId, actualViewModel.NemesisPlayerId);
        }

        [Test]
        public void ItCopiesTheNemesisPlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.AreEqual(this.playerWithNemesis.NemesisPlayerName, actualViewModel.NemesisPlayerName);
        }

        [Test]
        public void ItCopiesThePreviousNemesisPlayerId()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.AreEqual(this.playerWithNemesis.PreviousNemesisPlayerId, actualViewModel.PreviousNemesisPlayerId);
        }

        [Test]
        public void ItCopiesThePreviousNemesisPlayerName()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.AreEqual(this.playerWithNemesis.PreviousNemesisPlayerName, actualViewModel.PreviousNemesisPlayerName);
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.True(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, this.currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            PlayerWithNemesisViewModel actualViewModel = builder.Build(this.playerWithNemesis, null);

            Assert.False(actualViewModel.UserCanEdit);
        }
    }
}
