using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using NUnit.Framework;
using UI.Models;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class ChampionViewModelBuilderTests
    {
        private IChampionViewModelBuilder championViewModelBuilder;
        private Champion champion;
        private GameDefinition championGameDefinition;
        private string gameDefinitionName = "Game Definition Name";
        private int gameDefinitionId = 1;
        private int numberOfGames = 500;
        private int numberOfWins = 505;
        private float winPercentage = 99;

        [SetUp]
        public void SetUp()
        {
            championViewModelBuilder = new ChampionViewModelBuilder();

            championGameDefinition = new GameDefinition
            {
                Name = gameDefinitionName,
                Id = gameDefinitionId
            };
            champion = new Champion
            {
                GameDefinition = championGameDefinition,
                NumberOfGames = numberOfGames,
                NumberOfWins = numberOfWins,
                WinPercentage = winPercentage
            };
        }

        [Test]
        public void ItSetsTheGameDefinitionName()
        {
            ChampionViewModel actual = championViewModelBuilder.Build(champion);

            Assert.That(actual.GameDefinitionName, Is.EqualTo(gameDefinitionName));
        }

        [Test]
        public void ItSetsTheGameDefinitionId()
        {
            ChampionViewModel actual = championViewModelBuilder.Build(champion);

            Assert.That(actual.GameDefinitionId, Is.EqualTo(gameDefinitionId));
        }

        [Test]
        public void ItSetsTheNumberOfGames()
        {
            ChampionViewModel actual = championViewModelBuilder.Build(champion);

            Assert.That(actual.NumberOfGames, Is.EqualTo(numberOfGames));
        }

        [Test]
        public void ItSetsTheNumberOfWins()
        {
            ChampionViewModel actual = championViewModelBuilder.Build(champion);

            Assert.That(actual.NumberOfWins, Is.EqualTo(numberOfWins));
        }

        [Test]
        public void ItSetsTheWinPercentage()
        {
            ChampionViewModel actual = championViewModelBuilder.Build(champion);

            Assert.That(actual.WinPercentage, Is.EqualTo(winPercentage));
        }
    }
}
