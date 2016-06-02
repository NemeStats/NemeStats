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
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Points;
using NUnit.Framework;
using UI.Models.PlayedGame;
using UI.Models.Points;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests.GameResultViewModelBuilderTests
{
    [TestFixture]
    public class GameResultViewModelBuilderTests
    {
        GameResultViewModelBuilder builder;
        PlayerGameResult playerGameResult;
        GameResultViewModel playerGameResultDetails;

        [SetUp]
        public void SetUp()
        {
            builder = new GameResultViewModelBuilder();
            GameDefinition gameDefinition = new GameDefinition()
            {
                Id = 15131,
                Name = "Yodle-masters 2014"
            };
            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinition = gameDefinition,
                DatePlayed = new DateTime(2014, 09, 15),
                WinnerType = WinnerTypes.TeamWin
            };
            playerGameResult = new PlayerGameResult()
            {
                GameRank = 1,
                NemeStatsPointsAwarded = 2,
                Id = 151,
                PlayerId = 15135,
                Player = new Player()
                {
                    Name = "Test Player",
                    Active = false
                },
                PlayedGameId = 1432,
                PlayedGame = playedGame
            };

            playerGameResultDetails = builder.Build(playerGameResult);
        }

        [Test]
        public void ItRequiresAPlayerGameResult()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                    builder.Build(null)
                );

            Assert.AreEqual("playerGameResult", exception.ParamName);
        }

        [Test]
        public void ItRequiresAPlayerOnThePlayedGameResult()
        {
            PlayerGameResult playerGameResultWithNoPlayer = new PlayerGameResult();

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerGameResultWithNoPlayer)
                );

            Assert.AreEqual(GameResultViewModelBuilder.EXCEPTION_PLAYER_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresAPlayedGameOnThePlayedGameResult()
        {
            PlayerGameResult playerGameResultWithNoPlayedGame = new PlayerGameResult() { Player = new Player() };

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerGameResultWithNoPlayedGame)
                );

            Assert.AreEqual(GameResultViewModelBuilder.EXCEPTION_PLAYED_GAME_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresAGameDefinitionOnThePlayedGameResult()
        {
            PlayerGameResult playerGameResultWithNoGameDefinition = new PlayerGameResult() 
            {
                Player = new Player(),
                PlayedGame = new PlayedGame()
            };

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerGameResultWithNoGameDefinition)
                );

            Assert.AreEqual(GameResultViewModelBuilder.EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Assert.AreEqual(playerGameResult.PlayerId, playerGameResultDetails.PlayerId);
        }

        [Test]
        public void ItBuildsThePlayerName()
        {
            Assert.AreEqual(PlayerNameBuilder.BuildPlayerName(playerGameResult.Player.Name, playerGameResult.Player.Active), playerGameResultDetails.PlayerName);
        }

        [Test]
        public void ItCopiesTheGameRank()
        {
            Assert.AreEqual(playerGameResult.GameRank, playerGameResultDetails.GameRank);
        }

        [Test]
        public void ItCopiesTheNemeStatsPoints()
        {
            var expected = new NemePointsSummaryViewModel(playerGameResult.NemeStatsPointsAwarded, playerGameResult.GameDurationBonusPoints, playerGameResult.GameWeightBonusPoints);
            Assert.AreEqual(expected, playerGameResultDetails.NemePointsSummary);
        }

        [Test]
        public void ItCopiesThePlayedGameId()
        {
            Assert.AreEqual(playerGameResult.PlayedGameId, playerGameResultDetails.PlayedGameId);
        }

        [Test]
        public void ItCopiesTheDatePlayed()
        {
            Assert.AreEqual(playerGameResult.PlayedGame.DatePlayed, playerGameResultDetails.DatePlayed);
        }

        [Test]
        public void ItCopiesTheWinnerTypeFromThePlayedGame()
        {
            Assert.AreEqual(playerGameResult.PlayedGame.WinnerType, playerGameResultDetails.WinnerType);
        }
    }
}
