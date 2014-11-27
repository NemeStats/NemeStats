using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using NUnit.Framework;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests
{
    public class PlayerEditViewModelBuilderTests
    {
        private Player player;
        private PlayerEditViewModel actualModel;

        [SetUp]
        public void SetUp()
        {
            player = new Player
            {
                Active = false,
                Id = 123,
                Name = "the name",
                GamingGroupId = 789
            };
            actualModel = new PlayerEditViewModelBuilder().Build(player);
        }

        [Test]
        public void ItCopiesTheActiveFlag()
        {
            Assert.That(actualModel.Active, Is.EqualTo(player.Active));
        }

        [Test]
        public void ItCopiesTheId()
        {
            Assert.That(actualModel.Id, Is.EqualTo(player.Id));
        }

        [Test]
        public void ItCopiesTheName()
        {
            Assert.That(actualModel.Name, Is.EqualTo(player.Name));
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.That(actualModel.GamingGroupId, Is.EqualTo(player.GamingGroupId));
        }
    }
}
