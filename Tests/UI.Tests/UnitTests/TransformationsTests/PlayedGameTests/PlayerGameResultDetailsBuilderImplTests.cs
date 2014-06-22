using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayedGameTests
{
    [TestFixture]
    public class PlayerGameResultDetailsBuilderImplTests
    {
        PlayerGameResultDetailsViewModelBuilderImpl builder;
        PlayerGameResult playerGameResult;
        PlayerGameResultDetailsViewModel playerGameResultDetails;

        [SetUp]
        public void SetUp()
        {
            builder = new PlayerGameResultDetailsViewModelBuilderImpl();

            playerGameResult = new PlayerGameResult()
            {
                GameRank = 1,
                GordonPoints = 2,
                Id = 151,
                PlayerId = 15135,
                Player = new Player()
                {
                    Name = "Test Player"
                }
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

            Assert.AreEqual(PlayerGameResultDetailsViewModelBuilderImpl.EXCEPTION_PLAYER_CANNOT_BE_NULL, exception.Message);
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
    }
}
