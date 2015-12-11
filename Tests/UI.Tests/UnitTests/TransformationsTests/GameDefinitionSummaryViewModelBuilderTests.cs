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
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GameDefinitionModels;
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
        protected string championName = "Champion Name";
        protected int championPlayerId = 999;
        protected string previousChampionName = "Previous Champion Name";
        protected int previousChampionPlayerId = 998;
        protected Player championPlayer;
        protected Player previousChampionPlayer;

        [OneTimeSetUpAttribute]
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
        //copy
        [Test]
        public void ItSetsTheChampionPlayerIdWhenThereIsAChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);
            Assert.That(actualViewModel.ChampionPlayerId, Is.EqualTo(championPlayerId));
        }
        //copy
        [Test]
        public void ItSetsThePreviousChampionNameWhenThereIsAPreviousChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.PreviousChampionName, Is.EqualTo(previousChampionName));
        }
        //copy
        [Test]
        public void ItSetsThePreviousChampionPlayerIdWhenThereIsAPreviousChampion()
        {
            var actualViewModel = transformer.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualViewModel.PreviousChampionPlayerId, Is.EqualTo(previousChampionPlayerId));
        }
    }
}
