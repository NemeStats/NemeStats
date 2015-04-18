using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameRetrieverTests
{
    [TestFixture]
    public class SearchPlayedGamesTests
    {
        private RhinoAutoMocker<PlayedGameRetriever> autoMocker;
        private List<PlayedGame> playedGames;
            
        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<PlayedGameRetriever>();

            playedGames = new List<PlayedGame>
            {
                new PlayedGame
                {

                }
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGames.AsQueryable());
        }

        [Test]
        public void ItReturnsAnEmptyListIfThereAreNoSearchResults()
        {
            var results = autoMocker.ClassUnderTest.SearchPlayedGames(new PlayedGameFilter(), new ApplicationUser());

            Assert.That(results.Count, Is.EqualTo(0));
        }
    }
}
