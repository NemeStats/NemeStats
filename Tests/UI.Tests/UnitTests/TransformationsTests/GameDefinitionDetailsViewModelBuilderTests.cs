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
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using BusinessLogic.Logic;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.Players;
using StructureMap.AutoMocking;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Models.UniversalGameModels;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GameDefinitionDetailsViewModelBuilderTests
    {
        private RhinoAutoMocker<GameDefinitionDetailsViewModelBuilder> _autoMocker; 
        private GameDefinitionSummary _gameDefinitionSummary;
        private GameDefinitionDetailsViewModel2 _viewModel;
        private PlayedGameDetailsViewModel _playedGameDetailsViewModel1;
        private PlayedGameDetailsViewModel _playedGameDetailsViewModel2;
        private ApplicationUser _currentUser;
        private readonly int _gamingGroupid = 135;
        private Champion _champion;
        private Champion _previousChampion;
        private readonly float _championWinPercentage = 100;
        private readonly int _championNumberOfGames = 6;
        private readonly int _championNumberOfWins = 4;
        private readonly string _championName = "Champion Name";
        private readonly int _championPlayerId = 999;
        private readonly string _previousChampionName = "Previous Champion Name";
        private readonly int _previousChampionPlayerId = 998;
        private Player _championPlayer;
        private Player _previousChampionPlayer;
        private PlayerWinRecord _playerWinRecord1;
        private PlayerWinRecord _playerWinRecord2;
        private GameDefinitionPlayerSummaryViewModel _expectedPlayerSummary1;
        private GameDefinitionPlayerSummaryViewModel _expectedPlayerSummary2;
        private BoardGameGeekInfoViewModel _expectedBoardGameGeekInfo;

        [OneTimeSetUpAttribute]
        public void FixtureSetUp()
        {
            _autoMocker = new RhinoAutoMocker<GameDefinitionDetailsViewModelBuilder>();
            _autoMocker.PartialMockTheClassUnderTest();

            _expectedPlayerSummary1 = new GameDefinitionPlayerSummaryViewModel();
            _expectedPlayerSummary2 = new GameDefinitionPlayerSummaryViewModel();

            List<PlayedGame> playedGames = new List<PlayedGame>();
            playedGames.Add(new PlayedGame
            {
                    Id = 10
                });
            _playedGameDetailsViewModel1 = new PlayedGameDetailsViewModel();
            playedGames.Add(new PlayedGame
            {
                Id = 11
            });
            _playedGameDetailsViewModel2 = new PlayedGameDetailsViewModel();
            _championPlayer = new Player
            {
                Name = _championName,
                Id = _championPlayerId,
                Active = true
            };
            _previousChampionPlayer = new Player
            {
                Name = _previousChampionName,
                Id = _previousChampionPlayerId,
                Active = false
            };
            _champion = new Champion
            {
                Player = _championPlayer,
                WinPercentage = _championWinPercentage,
                NumberOfGames = _championNumberOfGames,
                NumberOfWins = _championNumberOfWins
            };
            _previousChampion = new Champion
            {
                Player = _previousChampionPlayer
            };
            _playerWinRecord1 = new PlayerWinRecord
            {
                GamesWon = 1,
                GamesLost = 2,
                PlayerName = "player name",
                WinPercentage = 33,
                PlayerId = 3
            };
            _playerWinRecord2 = new PlayerWinRecord();

            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GameDefinitionPlayerSummaryViewModel>(_playerWinRecord1))
                       .Return(_expectedPlayerSummary1);
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GameDefinitionPlayerSummaryViewModel>(_playerWinRecord2))
                 .Return(_expectedPlayerSummary2); 

            _gameDefinitionSummary = new GameDefinitionSummary
            {
                Id = 1,
                Name = "game definition name",
                Description = "game definition description",
                GamingGroupId = _gamingGroupid,
                GamingGroupName = "gaming group name",
                PlayedGames = playedGames,
                TotalNumberOfGamesPlayed = 3,
                AveragePlayersPerGame = 2.2M,
                BoardGameGeekGameDefinitionId = 123,
                BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition() { Id = 123},
                Champion = _champion,
                PreviousChampion = _previousChampion,
                PlayerWinRecords = new List<PlayerWinRecord>
                {
                    _playerWinRecord1,
                    _playerWinRecord2
                },
                BoardGameGeekInfo = new BoardGameGeekInfo()
            };
            _currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = _gamingGroupid
            };
            _autoMocker.Get<IPlayedGameDetailsViewModelBuilder>().Expect(mock => mock.Build(_gameDefinitionSummary.PlayedGames[0], _currentUser))
                .Return(_playedGameDetailsViewModel1);
            _autoMocker.Get<IPlayedGameDetailsViewModelBuilder>().Expect(mock => mock.Build(_gameDefinitionSummary.PlayedGames[1], _currentUser))
                .Return(_playedGameDetailsViewModel2);

            _expectedBoardGameGeekInfo = new BoardGameGeekInfoViewModel();
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<BoardGameGeekInfoViewModel>(_gameDefinitionSummary.BoardGameGeekInfo))
                .Return(_expectedBoardGameGeekInfo);

            _viewModel = _autoMocker.ClassUnderTest.Build(_gameDefinitionSummary, _currentUser);
        }

        [Test]
        public void ItCopiesTheId()
        {
            Assert.AreEqual(_gameDefinitionSummary.Id, _viewModel.GameDefinitionId);
        }

        [Test]
        public void ItCopiesTheName()
        {
            Assert.AreEqual(_gameDefinitionSummary.Name, _viewModel.GameDefinitionName);
        }

        [Test]
        public void ItCopiesTheTotalNumberOfGamesPlayed()
        {
            Assert.AreEqual(_gameDefinitionSummary.TotalNumberOfGamesPlayed, _viewModel.TotalNumberOfGamesPlayed);
        }

        [Test]
        public void ItCopiesTheAveragePlayersPerGame()
        {
            var expectedValue = $"{_gameDefinitionSummary.AveragePlayersPerGame:0.#}";
            Assert.AreEqual(expectedValue, _viewModel.AveragePlayersPerGame);
        }

        [Test]
        public void ItTransformsThePlayedGamesIntoPlayedGameDetailViewModelsAndSetsOnTheViewModel()
        {
            Assert.AreEqual(_playedGameDetailsViewModel1, _viewModel.PlayedGames[0]);
            Assert.AreEqual(_playedGameDetailsViewModel2, _viewModel.PlayedGames[1]);
        }

        [Test]
        public void ItSetsThePlayedGamesToAnEmptyListIfThereAreNone()
        {
            _gameDefinitionSummary.PlayedGames = null;

            var actualViewModel = _autoMocker.ClassUnderTest.Build(_gameDefinitionSummary, _currentUser);

            Assert.AreEqual(new List<PlayedGameDetailsViewModel>(), actualViewModel.PlayedGames);
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
            Assert.True(_viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            _currentUser.CurrentGamingGroupId = -1;
            var actualViewModel = _autoMocker.ClassUnderTest.Build(_gameDefinitionSummary, _currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            var actualViewModel = _autoMocker.ClassUnderTest.Build(_gameDefinitionSummary, null);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void ItSetsTheChampionNameWhenThereIsAChampion()
        {
            Assert.That(_viewModel.ChampionName, Is.EqualTo(PlayerNameBuilder.BuildPlayerName(_championPlayer.Name, _championPlayer.Active)));
        }

        [Test]
        public void ItSetsTheChampionWinPercentageWhenThereIsAChampion()
        {
            Assert.That(_viewModel.WinPercentage, Is.EqualTo(_championWinPercentage));
        }

        [Test]
        public void ItSetsTheChampionGamesPlayedWhenThereIsAChampion()
        {
            Assert.That(_viewModel.NumberOfGamesPlayed, Is.EqualTo(_championNumberOfGames));
        }

        [Test]
        public void ItSetsTheChampionGamesWonWhenThereIsAChampion()
        {
            Assert.That(_viewModel.NumberOfWins, Is.EqualTo(_championNumberOfWins));
        }

        [Test]
        public void ItSetsTheChampionPlayerIdWhenThereIsAChampion()
        {
            Assert.That(_viewModel.ChampionPlayerId, Is.EqualTo(_championPlayerId));
        }

        [Test]
        public void ItSetsThePreviousChampionNameWhenThereIsAPreviousChampion()
        {
            Assert.That(_viewModel.PreviousChampionName, Is.EqualTo(PlayerNameBuilder.BuildPlayerName(_previousChampionPlayer.Name, _previousChampionPlayer.Active)));
        }

        [Test]
        public void ItSetsThePreviousChampionPlayerIdWhenThereIsAPreviousChampion()
        {
            Assert.That(_viewModel.PreviousChampionPlayerId, Is.EqualTo(_previousChampionPlayerId));
        }

        [Test]
        public void It_Sets_The_BoardGameGeekInfo()
        {
            //--arrange

            //--act
            Assert.That(_viewModel.BoardGameGeekInfo, Is.Not.Null);
            Assert.That(_viewModel.BoardGameGeekInfo, Is.SameAs(_expectedBoardGameGeekInfo));

            //--assert
        }


        [Test]
        public void ItBuildsThePlayerSummaryViewModels()
        {
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GameDefinitionPlayerSummaryViewModel>(_playerWinRecord1))
                       .Return(_expectedPlayerSummary1);
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<GameDefinitionPlayerSummaryViewModel>(_playerWinRecord2))
                 .Return(_expectedPlayerSummary2); 

            var actualResult = _autoMocker.ClassUnderTest.Build(_gameDefinitionSummary, _currentUser);

            Assert.That(actualResult.GameDefinitionPlayersSummary.Count, Is.EqualTo(2));
            Assert.That(actualResult.GameDefinitionPlayersSummary, Contains.Item(_expectedPlayerSummary1));
            Assert.That(actualResult.GameDefinitionPlayersSummary, Contains.Item(_expectedPlayerSummary2));
        }
    }
}
 