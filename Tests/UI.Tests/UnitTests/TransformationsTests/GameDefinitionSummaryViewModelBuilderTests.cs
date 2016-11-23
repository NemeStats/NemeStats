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
using BusinessLogic.Logic.Points;
using UI.Models.GameDefinitionModels;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GameDefinitionSummaryViewModelBuilderTests
    {
        private GameDefinitionSummaryViewModelBuilder _transformer;
        private GameDefinitionSummary _gameDefinitionSummary;
        private GameDefinitionSummaryViewModel _viewModel;
        private ApplicationUser _currentUser;
        private int _gamingGroupid = 135;
        private Champion _champion;
        private Champion _previousChampion;
        private string _championName = "Champion Name";
        private int _championPlayerId = 999;
        private string _previousChampionName = "Previous Champion Name";
        private int _previousChampionPlayerId = 998;
        private Player _championPlayer;
        private Player _previousChampionPlayer;

        [OneTimeSetUpAttribute]
        public void FixtureSetUp()
        {
            _transformer = new GameDefinitionSummaryViewModelBuilder(new Transformer(), new WeightTierCalculator());            

            List<PlayedGame> playedGames = new List<PlayedGame>();
            playedGames.Add(new PlayedGame()
                {
                    Id = 10
                });
            playedGames.Add(new PlayedGame()
            {
                Id = 11
            });
            _championPlayer = new Player
            {
                Name = _championName,
                Id = _championPlayerId
            };
            _previousChampionPlayer = new Player
            {
                Name = _previousChampionName,
                Id = _previousChampionPlayerId
            };
            _champion = new Champion
            {
                Player = _championPlayer,
            };
            _previousChampion = new Champion
            {
                Player = _previousChampionPlayer
            };
            _gameDefinitionSummary = new GameDefinitionSummary()
            {
                Id = 1,
                Name = "game definition name",
                Description = "game definition description",
                GamingGroupId = _gamingGroupid,
                GamingGroupName = "gaming group name",
                PlayedGames = playedGames,
                Champion = _champion,
                PreviousChampion = _previousChampion
            };
            _currentUser = new ApplicationUser()
            {
                CurrentGamingGroupId = _gamingGroupid
            };

            _viewModel = _transformer.Build(_gameDefinitionSummary, _currentUser);
        }

        [Test]
        public void ItCopiesTheId()
        {
            Assert.AreEqual(_gameDefinitionSummary.Id, _viewModel.Id);
        }

        [Test]
        public void ItCopiesTheName()
        {
            Assert.AreEqual(_gameDefinitionSummary.Name, _viewModel.Name);
        }

        [Test]
        public void ItCopiesTheDescription()
        {
            Assert.AreEqual(_gameDefinitionSummary.Description, _viewModel.Description);
        }

        [Test]
        public void ItCopiesTheTotalNumberOfGamesPlayed()
        {
            Assert.AreEqual(_gameDefinitionSummary.TotalNumberOfGamesPlayed, _viewModel.TotalNumberOfGamesPlayed);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(_gameDefinitionSummary.GamingGroupName, _viewModel.GamingGroupName);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(_gameDefinitionSummary.GamingGroupId, _viewModel.GamingGroupId);
        }

        [Test]
        public void TheUserCanEditViewModelIfTheyShareGamingGroups()
        {
            var actualViewModel = _transformer.Build(_gameDefinitionSummary, _currentUser);

            Assert.True(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            _currentUser.CurrentGamingGroupId = -1;
            var actualViewModel = _transformer.Build(_gameDefinitionSummary, _currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            var actualViewModel = _transformer.Build(_gameDefinitionSummary, null);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void ItSetsTheChampionNameWhenThereIsAChampion()
        {
            var actualViewModel = _transformer.Build(_gameDefinitionSummary, _currentUser);

            Assert.That(actualViewModel.ChampionName, Is.EqualTo(_championName));
        }
        //copy
        [Test]
        public void ItSetsTheChampionPlayerIdWhenThereIsAChampion()
        {
            var actualViewModel = _transformer.Build(_gameDefinitionSummary, _currentUser);
            Assert.That(actualViewModel.ChampionPlayerId, Is.EqualTo(_championPlayerId));
        }
        //copy
        [Test]
        public void ItSetsThePreviousChampionNameWhenThereIsAPreviousChampion()
        {
            var actualViewModel = _transformer.Build(_gameDefinitionSummary, _currentUser);

            Assert.That(actualViewModel.PreviousChampionName, Is.EqualTo(_previousChampionName));
        }
        //copy
        [Test]
        public void ItSetsThePreviousChampionPlayerIdWhenThereIsAPreviousChampion()
        {
            var actualViewModel = _transformer.Build(_gameDefinitionSummary, _currentUser);

            Assert.That(actualViewModel.PreviousChampionPlayerId, Is.EqualTo(_previousChampionPlayerId));
        }
    }
}
