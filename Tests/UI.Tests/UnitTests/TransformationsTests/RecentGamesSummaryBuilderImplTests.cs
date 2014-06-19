using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class RecentGamesSummaryBuilderImplTests
    {
        private PlayedGameDetailsBuilderImpl builder;
        private List<PlayedGame> playedGames;
        private PlayedGameDetails summary;

        private int gameDefinitionId = 15113;

        [SetUp]
        public void SetUp()
        {
            builder = new PlayedGameDetailsBuilderImpl();

//            PlayedGame playedGame1 = new PlayedGame()
//            {
//                DatePlayed = 
//            };
//            playedGames = new List<PlayedGame>()
//            {
//                new PlayedGame()
//                {

//}
//            };
        }

        [Test]
        public void ItCopiesTheGameId()
        {
            Assert.AreEqual(gameDefinitionId, summary.GameDefinitionId);
        }
    }
}
