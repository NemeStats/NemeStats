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
using System.Linq;
using BusinessLogic.Models;
using NUnit.Framework;
using UI.Models.PlayedGame;
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
            this.builder = new GameResultViewModelBuilder();
            GameDefinition gameDefinition = new GameDefinition()
            {
                Id = 15131,
                Name = "Yodle-masters 2014"
            };
            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinition = gameDefinition,
                DatePlayed = new DateTime(2014, 09, 15)
            };
            this.playerGameResult = new PlayerGameResult()
            {
                GameRank = 1,
                NemeStatsPointsAwarded = 2,
                Id = 151,
                PlayerId = 15135,
                Player = new Player()
                {
                    Name = "Test Player"
                },
                PlayedGameId = 1432,
                PlayedGame = playedGame
            };

            this.playerGameResultDetails = this.builder.Build(this.playerGameResult);
        }

        [Test]
        public void ItRequiresAPlayerGameResult()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                    this.builder.Build(null)
                );

            Assert.AreEqual("playerGameResult", exception.ParamName);
        }

        [Test]
        public void ItRequiresAPlayerOnThePlayedGameResult()
        {
            PlayerGameResult playerGameResultWithNoPlayer = new PlayerGameResult();

            var exception = Assert.Throws<ArgumentException>(() =>
                    this.builder.Build(playerGameResultWithNoPlayer)
                );

            Assert.AreEqual(GameResultViewModelBuilder.EXCEPTION_PLAYER_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresAPlayedGameOnThePlayedGameResult()
        {
            PlayerGameResult playerGameResultWithNoPlayedGame = new PlayerGameResult() { Player = new Player() };

            var exception = Assert.Throws<ArgumentException>(() =>
                    this.builder.Build(playerGameResultWithNoPlayedGame)
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
                    this.builder.Build(playerGameResultWithNoGameDefinition)
                );

            Assert.AreEqual(GameResultViewModelBuilder.EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Assert.AreEqual(this.playerGameResult.PlayerId, this.playerGameResultDetails.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            Assert.AreEqual(this.playerGameResult.Player.Name, this.playerGameResultDetails.PlayerName);
        }

        [Test]
        public void ItCopiesTheGameRank()
        {
            Assert.AreEqual(this.playerGameResult.GameRank, this.playerGameResultDetails.GameRank);
        }

        [Test]
        public void ItCopiesThenemeStatsPoints()
        {
            Assert.AreEqual(this.playerGameResult.NemeStatsPointsAwarded, this.playerGameResultDetails.NemeStatsPoints);
        }

        [Test]
        public void ItCopiesThePlayedGameId()
        {
            Assert.AreEqual(this.playerGameResult.PlayedGameId, this.playerGameResultDetails.PlayedGameId);
        }

        [Test]
        public void ItCopiesTheDatePlayed()
        {
            Assert.AreEqual(this.playerGameResult.PlayedGame.DatePlayed, this.playerGameResultDetails.DatePlayed);
        }
    }
}
