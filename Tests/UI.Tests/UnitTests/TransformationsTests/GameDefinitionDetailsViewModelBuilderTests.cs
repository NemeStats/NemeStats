#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
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
                BoardGameGeekObjectId = 123,
                Champion = champion,
                PreviousChampion = previousChampion
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
