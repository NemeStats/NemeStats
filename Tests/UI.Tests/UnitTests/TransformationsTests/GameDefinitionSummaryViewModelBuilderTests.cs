using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GameDefinitionSummaryViewModelBuilderTests
    {
        protected GameDefinitionSummaryViewModelBuilder transformer;
        protected GameDefinitionSummary gameDefinitionSummary;
        protected GameDefinitionSummaryViewModel viewModel;
        protected ApplicationUser currentUser;
        protected int gamingGroupid = 135;
        protected Champion champion;
        protected Champion previousChampion;
        protected float championWinPercentage = 100;
        protected int championNumberOfGames = 6;
        protected int championNumberOfWins = 4;
        protected string championName = "Champion Name";
        protected int championPlayerId = 999;
        protected string previousChampionName = "Previous Champion Name";
        protected int previousChampionPlayerId = 998;
        protected Player championPlayer;
        protected Player previousChampionPlayer;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            transformer = new GameDefinitionSummaryViewModelBuilder();            

            List<PlayedGame> playedGames = new List<PlayedGame>();
            playedGames.Add(new PlayedGame()
                {
                    Id = 10
                });
            playedGames.Add(new PlayedGame()
            {
                Id = 11
            });
            championPlayer = new Player
            {
                Name = championName,
                Id = championPlayerId
            };
            previousChampionPlayer = new Player
            {
                Name = previousChampionName,
                Id = previousChampionPlayerId
            };
            champion = new Champion
            {
                Player = championPlayer,
                WinPercentage = championWinPercentage,
                NumberOfGames = championNumberOfGames,
                NumberOfWins = championNumberOfWins
            };
            previousChampion = new Champion
            {
                Player = previousChampionPlayer
            };
            gameDefinitionSummary = new GameDefinitionSummary()
            {
                Id = 1,
                Name = "game definition name",
                Description = "game definition description",
                GamingGroupId = gamingGroupid,
                GamingGroupName = "gaming group name",
                PlayedGames = playedGames,
                Champion = champion,
                PreviousChampion = previousChampion
            };
            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = gamingGroupid
            };

            viewModel = transformer.Build(gameDefinitionSummary, currentUser);
        }

        [Test]
        public void ItCopiesTheId()
        {
            Assert.AreEqual(gameDefinitionSummary.Id, viewModel.Id);
        }

        [Test]
        public void ItCopiesTheName()
        {
            Assert.AreEqual(gameDefinitionSummary.Name, viewModel.Name);
        }

        [Test]
        public void ItCopiesTheDescription()
        {
            Assert.AreEqual(gameDefinitionSummary.Description, viewModel.Description);
        }

        [Test]
        public void ItCopiesTheTotalNumberOfGamesPlayed()
        {
            Assert.AreEqual(gameDefinitionSummary.TotalNumberOfGamesPlayed, viewModel.TotalNumberOfGamesPlayed);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(gameDefinitionSummary.GamingGroupName, viewModel.GamingGroupName);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(gameDefinitionSummary.GamingGroupId, viewModel.GamingGroupId);
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.True(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, null);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void ItSetsTheChampionNameWhenThereIsAChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.ChampionName, Is.EqualTo(championName));
        }

        [Test]
        public void ItSetsTheChampionWinPercentageWhenThereIsAChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.WinPercentage, Is.EqualTo(championWinPercentage));
        }

        [Test]
        public void ItSetsTheChampionGamesPlayedWhenThereIsAChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);
            Assert.That(actualViewModel.NumberOfGamesPlayed, Is.EqualTo(championNumberOfGames));
        }

        [Test]
        public void ItSetsTheChampionGamesWonWhenThereIsAChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);
            Assert.That(actualViewModel.NumberOfWins, Is.EqualTo(championNumberOfWins));
        }

        [Test]
        public void ItSetsTheChampionPlayerIdWhenThereIsAChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);
            Assert.That(actualViewModel.ChampionPlayerId, Is.EqualTo(championPlayerId));
        }

        [Test]
        public void ItSetsThePreviousChampionNameWhenThereIsAPreviousChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.PreviousChampionName, Is.EqualTo(previousChampionName));
        }

        [Test]
        public void ItSetsThePreviousChampionPlayerIdWhenThereIsAPreviousChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.PreviousChampionPlayerId, Is.EqualTo(previousChampionPlayerId));
        }
    }
}
