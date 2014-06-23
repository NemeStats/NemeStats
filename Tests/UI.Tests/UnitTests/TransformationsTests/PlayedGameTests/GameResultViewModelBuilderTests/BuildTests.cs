using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayedGameTests.GameResultViewModelBuilderTests
{
    [TestFixture]
    public class GameResultViewModelBuilderImplTests
    {
        GameResultViewModelBuilderImpl builder;
        PlayerGameResult playerGameResult;
        GameResultViewModel playerGameResultDetails;

        [SetUp]
        public void SetUp()
        {
            builder = new GameResultViewModelBuilderImpl();
            GameDefinition gameDefinition = new GameDefinition()
            {
                Id = 15131,
                Name = "Yodle-masters 2014"
            };
            PlayedGame playedGame = new PlayedGame()
            {
                GameDefinition = gameDefinition
            };
            playerGameResult = new PlayerGameResult()
            {
                GameRank = 1,
                GordonPoints = 2,
                Id = 151,
                PlayerId = 15135,
                Player = new Player()
                {
                    Name = "Test Player"
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

            Assert.AreEqual(GameResultViewModelBuilderImpl.EXCEPTION_PLAYER_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresAPlayedGameOnThePlayedGameResult()
        {
            PlayerGameResult playerGameResultWithNoPlayedGame = new PlayerGameResult() { Player = new Player() };

            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(playerGameResultWithNoPlayedGame)
                );

            Assert.AreEqual(GameResultViewModelBuilderImpl.EXCEPTION_PLAYED_GAME_CANNOT_BE_NULL, exception.Message);
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

            Assert.AreEqual(GameResultViewModelBuilderImpl.EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesThePlayerId()
        {
            Assert.AreEqual(playerGameResult.PlayerId, playerGameResultDetails.PlayerId);
        }

        [Test]
        public void ItCopiesThePlayerName()
        {
            Assert.AreEqual(playerGameResult.Player.Name, playerGameResultDetails.PlayerName);
        }

        [Test]
        public void ItCopiesTheGameRank()
        {
            Assert.AreEqual(playerGameResult.GameRank, playerGameResultDetails.GameRank);
        }

        [Test]
        public void ItCopiesTheGordonPoints()
        {
            Assert.AreEqual(playerGameResult.GordonPoints, playerGameResultDetails.GordonPoints);
        }

        [Test]
        public void ItCopiesThePlayedGameId()
        {
            Assert.AreEqual(playerGameResult.PlayedGameId, playerGameResultDetails.PlayedGameId);
        }
    }
}
