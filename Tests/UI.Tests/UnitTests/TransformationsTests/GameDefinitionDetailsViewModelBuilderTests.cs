using System;
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
    public class GameDefinitionDetailsViewModelBuilderTests
    {
        protected GameDefinitionDetailsViewModelBuilder transformer;
        protected IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;
        protected GameDefinitionSummary gameDefinitionSummary;
        protected GameDefinitionDetailsViewModel viewModel;
        protected PlayedGameDetailsViewModel playedGameDetailsViewModel1;
        protected PlayedGameDetailsViewModel playedGameDetailsViewModel2;
        protected ApplicationUser currentUser;
        protected int gamingGroupid = 135;
        protected Champion champion;
        protected Champion previousChampion;
        protected float championWinPercentage = 100;
        protected string championName = "Champion Name";
        protected string previousChampionName = "Previous Champion Name";
        protected Player championPlayer;
        protected Player previousChampionPlayer;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            playedGameDetailsViewModelBuilder = MockRepository.GenerateMock<IPlayedGameDetailsViewModelBuilder>();
            transformer = new GameDefinitionDetailsViewModelBuilder(playedGameDetailsViewModelBuilder);            

            List<PlayedGame> playedGames = new List<PlayedGame>();
            playedGames.Add(new PlayedGame()
                {
                    Id = 10
                });
            playedGameDetailsViewModel1 = new PlayedGameDetailsViewModel();
            playedGames.Add(new PlayedGame()
            {
                Id = 11
            });
            playedGameDetailsViewModel2 = new PlayedGameDetailsViewModel();
            championPlayer = new Player
            {
                Name = championName
            };
            previousChampionPlayer = new Player
            {
                Name = previousChampionName
            };
            champion = new Champion
            {
                Player = championPlayer,
                WinPercentage = championWinPercentage
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
                PreviousChampion = previousChampion,
                BoardGameGeekObjectId = 123
            };
            currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = gamingGroupid
            };
            playedGameDetailsViewModelBuilder.Expect(mock => mock.Build(gameDefinitionSummary.PlayedGames[0], currentUser))
                .Return(playedGameDetailsViewModel1);
            playedGameDetailsViewModelBuilder.Expect(mock => mock.Build(gameDefinitionSummary.PlayedGames[1], currentUser))
                .Return(playedGameDetailsViewModel2);

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
        public void ItTransformsThePlayedGamesIntoPlayedGameDetailViewModelsAndSetsOnTheViewModel()
        {
            Assert.AreEqual(playedGameDetailsViewModel1, viewModel.PlayedGames[0]);
            Assert.AreEqual(playedGameDetailsViewModel2, viewModel.PlayedGames[1]);
        }

        [Test]
        public void ItSetsThePlayedGamesToAnEmptyListIfThereAreNone()
        {
            gameDefinitionSummary.PlayedGames = null;

            GameDefinitionDetailsViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.AreEqual(new List<PlayedGameDetailsViewModel>(), actualViewModel.PlayedGames);
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
        public void ItCopiesTheBoardGameGeekObjectId()
        {
            Assert.That(gameDefinitionSummary.BoardGameGeekObjectId, Is.EqualTo(viewModel.BoardGameGeekObjectId));
        }

        [Test]
        public void ItCopiesTheBoardGameGeekUri()
        {
            Assert.That(gameDefinitionSummary.BoardGameGeekUri, Is.EqualTo(viewModel.BoardGameGeekUri));
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            GameDefinitionDetailsViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.True(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            GameDefinitionDetailsViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            GameDefinitionDetailsViewModel actualViewModel = transformer.Build(gameDefinitionSummary, null);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void ItSetsTheChampionNameWhenThereIsAChampion()
        {
            GameDefinitionDetailsViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.ChampionName, Is.EqualTo(championName));
        }

        [Test]
        public void ItSetsTheChampionWinPercentageWhenThereIsAChampion()
        {
            GameDefinitionDetailsViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.WinPercentage, Is.EqualTo(championWinPercentage));
        }

        [Test]
        public void ItSetsThePreviousChampionNameWhenThereIsAPreviousChampion()
        {
            GameDefinitionDetailsViewModel actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.PreviousChampionName, Is.EqualTo(previousChampionName));
        }
    }
}
