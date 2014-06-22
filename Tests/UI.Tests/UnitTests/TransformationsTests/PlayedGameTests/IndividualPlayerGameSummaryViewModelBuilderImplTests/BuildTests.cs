using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.PlayedGame;
using UI.Transformations.Player;

namespace UI.Tests.UnitTests.TransformationsTests.PlayedGameTests.IndividualPlayerGameSummaryViewModelBuilderImplTests
{
    [TestFixture]
    public class BuildTests
    {
        IndividualPlayerGameSummaryViewModelBuilderImpl builder;
        IndividualPlayerGameSummaryViewModel viewModelResult;
        PlayerGameResult playerGameResult;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            builder = new IndividualPlayerGameSummaryViewModelBuilderImpl();

            playerGameResult = new PlayerGameResult()
            {
                PlayedGame = new PlayedGame()
                {
                    GameDefinition = new GameDefinition() {  Name = "Spin the bottle" }
                },
                PlayedGameId = 1351
            };

            viewModelResult = builder.Build(playerGameResult);
        }

        //TODO need consistent naming for these kinds of tests.
        [Test]
        public void PlayerGameResultCannotBeNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                    builder.Build(null));

            Assert.AreEqual("playerGameResult", exception.ParamName);
        }

        [Test]
        public void ItRequiresAPlayedGame()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(new PlayerGameResult()));

            Assert.AreEqual(IndividualPlayerGameSummaryViewModelBuilderImpl.EXCEPTION_PLAYED_GAME_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItRequiresAGameDefinition()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                    builder.Build(new PlayerGameResult() { PlayedGame = new PlayedGame() }));

            Assert.AreEqual(IndividualPlayerGameSummaryViewModelBuilderImpl.EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL, exception.Message);
        }

        [Test]
        public void ItCopiesGameRank()
        {
            Assert.AreEqual(playerGameResult.GameRank, viewModelResult.GameRank);
        }

        [Test]
        public void ItCopiesGordonPoints()
        {
            Assert.AreEqual(playerGameResult.GordonPoints, viewModelResult.GordonPoints);
        }

        [Test]
        public void ItCopiesPlayedGameId()
        {
            Assert.AreEqual(playerGameResult.PlayedGameId, viewModelResult.PlayedGameId);
        }

        [Test]
        public void ItCopiesGameDefinitionId()
        {
            Assert.AreEqual(playerGameResult.PlayedGame.GameDefinitionId, viewModelResult.GameDefinitionId);
        }

        [Test]
        public void ItCopiesGameName()
        {
            Assert.AreEqual(playerGameResult.PlayedGame.GameDefinition.Name, viewModelResult.GameName);
        }
    }
}
