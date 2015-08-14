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

using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using StructureMap.AutoMocking;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GameDefinitionDetailsViewModelBuilderTests
    {
        private RhinoAutoMocker<GameDefinitionDetailsViewModelBuilder> autoMocker; 
        private GameDefinitionSummary gameDefinitionSummary;
        private GameDefinitionDetailsViewModel viewModel;
        private PlayedGameDetailsViewModel playedGameDetailsViewModel1;
        private PlayedGameDetailsViewModel playedGameDetailsViewModel2;
        private ApplicationUser currentUser;
        private int gamingGroupid = 135;
        private Champion champion;
        private Champion previousChampion;
        private float championWinPercentage = 100;
        private int championNumberOfGames = 6;
        private int championNumberOfWins = 4;
        private string championName = "Champion Name";
        private int championPlayerId = 999;
        private string previousChampionName = "Previous Champion Name";
        private int previousChampionPlayerId = 998;
        private Player championPlayer;
        private Player previousChampionPlayer;
        private PlayerWinRecord playerWinRecord1;
        private PlayerWinRecord playerWinRecord2;
        private PlayerSummaryViewModel expectedPlayerSummary1;
        private PlayerSummaryViewModel expectedPlayerSummary2;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            autoMocker = new RhinoAutoMocker<GameDefinitionDetailsViewModelBuilder>();
            autoMocker.PartialMockTheClassUnderTest();

            List<PlayedGame> playedGames = new List<PlayedGame>();
            playedGames.Add(new PlayedGame
            {
                    Id = 10
                });
            playedGameDetailsViewModel1 = new PlayedGameDetailsViewModel();
            playedGames.Add(new PlayedGame
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
            playerWinRecord1 = new PlayerWinRecord
            {
                GamesWon = 1,
                GamesLost = 2,
                Name = "player name",
                WinPercentage = 33,
                PlayerId = 3
            };
            playerWinRecord2 = new PlayerWinRecord();

            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<PlayerWinRecord, PlayerSummaryViewModel>(playerWinRecord1))
                       .Return(expectedPlayerSummary1);
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<PlayerWinRecord, PlayerSummaryViewModel>(playerWinRecord2))
                 .Return(expectedPlayerSummary2); 

            gameDefinitionSummary = new GameDefinitionSummary
            {
                Id = 1,
                Name = "game definition name",
                Description = "game definition description",
                GamingGroupId = gamingGroupid,
                GamingGroupName = "gaming group name",
                PlayedGames = playedGames,
                BoardGameGeekObjectId = 123,
                Champion = champion,
                PreviousChampion = previousChampion,
                PlayerWinRecords = new List<PlayerWinRecord>
                {
                    playerWinRecord1,
                    playerWinRecord2
                } 
            };
            currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = gamingGroupid
            };
            autoMocker.Get<IPlayedGameDetailsViewModelBuilder>().Expect(mock => mock.Build(gameDefinitionSummary.PlayedGames[0], currentUser))
                .Return(playedGameDetailsViewModel1);
            autoMocker.Get<IPlayedGameDetailsViewModelBuilder>().Expect(mock => mock.Build(gameDefinitionSummary.PlayedGames[1], currentUser))
                .Return(playedGameDetailsViewModel2);

            viewModel = autoMocker.ClassUnderTest.Build(gameDefinitionSummary, currentUser);
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

            GameDefinitionDetailsViewModel actualViewModel = autoMocker.ClassUnderTest.Build(gameDefinitionSummary, currentUser);

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
            Assert.True(viewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheyDoNotShareGamingGroups()
        {
            currentUser.CurrentGamingGroupId = -1;
            GameDefinitionDetailsViewModel actualViewModel = autoMocker.ClassUnderTest.Build(gameDefinitionSummary, currentUser);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void TheUserCanNotEditViewModelIfTheUserIsUnknown()
        {
            GameDefinitionDetailsViewModel actualViewModel = autoMocker.ClassUnderTest.Build(gameDefinitionSummary, null);

            Assert.False(actualViewModel.UserCanEdit);
        }

        [Test]
        public void ItSetsTheChampionNameWhenThereIsAChampion()
        {
            Assert.That(viewModel.ChampionName, Is.EqualTo(championName));
        }

        [Test]
        public void ItSetsTheChampionWinPercentageWhenThereIsAChampion()
        {
            Assert.That(viewModel.WinPercentage, Is.EqualTo(championWinPercentage));
        }

        [Test]
        public void ItSetsTheChampionGamesPlayedWhenThereIsAChampion()
        {
            Assert.That(viewModel.NumberOfGamesPlayed, Is.EqualTo(championNumberOfGames));
        }

        [Test]
        public void ItSetsTheChampionGamesWonWhenThereIsAChampion()
        {
            Assert.That(viewModel.NumberOfWins, Is.EqualTo(championNumberOfWins));
        }

        [Test]
        public void ItSetsTheChampionPlayerIdWhenThereIsAChampion()
        {
            Assert.That(viewModel.ChampionPlayerId, Is.EqualTo(championPlayerId));
        }

        [Test]
        public void ItSetsThePreviousChampionNameWhenThereIsAPreviousChampion()
        {
            Assert.That(viewModel.PreviousChampionName, Is.EqualTo(previousChampionName));
        }

        [Test]
        public void ItSetsThePreviousChampionPlayerIdWhenThereIsAPreviousChampion()
        {
            Assert.That(viewModel.PreviousChampionPlayerId, Is.EqualTo(previousChampionPlayerId));
        }

        [Test]
        public void ItSetsTheWinLossHeader()
        {
            Assert.That(viewModel.PlayersSummary.WinLossHeader, Is.EqualTo("Win - Loss Record"));
        }

        [Test]
        public void ItBuildsThePlayerSummaryViewModels()
        {
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<PlayerWinRecord, PlayerSummaryViewModel>(playerWinRecord1))
                       .Return(expectedPlayerSummary1);
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<PlayerWinRecord, PlayerSummaryViewModel>(playerWinRecord2))
                 .Return(expectedPlayerSummary2); 

            var actualResult = autoMocker.ClassUnderTest.Build(gameDefinitionSummary, currentUser);

            Assert.That(actualResult.PlayersSummary.PlayerSummaries.Count, Is.EqualTo(2));
            Assert.That(actualResult.PlayersSummary.PlayerSummaries, Contains.Item(expectedPlayerSummary1));
            Assert.That(actualResult.PlayersSummary.PlayerSummaries, Contains.Item(expectedPlayerSummary2));
        }
    }
}
 