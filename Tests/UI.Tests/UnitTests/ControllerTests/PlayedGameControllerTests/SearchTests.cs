using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class SearchTests : PlayedGameControllerTestBase
    {
        [Test]
        public void ItReturnsTheCorrectViewModel()
        {
            var expectedSearchResults = new List<PlayedGameSearchResult>
            {
                new PlayedGameSearchResult
                {
                    BoardGameGeekObjectId = 1,
                    GameDefinitionId = 2,
                    GameDefinitionName = "some game definition name",
                    DatePlayed = new DateTime().Date,
                    PlayedGameId = 3,
                    PlayerGameResults = new List<PlayerGameResult>
                    {
                        new PlayerGameResult
                        {
                            GameRank = 1,
                            Id = 2,
                            NemeStatsPointsAwarded = 3,
                            PlayerId = 4,
                            Player = new Player
                            {
                                Name = "some player name"
                            },
                            PointsScored = 5
                        }
                    }
                }
            };
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.SearchPlayedGames(null))
                .IgnoreArguments()
                .Return(expectedSearchResults);

            var actualResults = autoMocker.ClassUnderTest.SearchPlayedGames(new PlayedGamesFilterViewModel(), currentUser);

            
        }

    }
}
