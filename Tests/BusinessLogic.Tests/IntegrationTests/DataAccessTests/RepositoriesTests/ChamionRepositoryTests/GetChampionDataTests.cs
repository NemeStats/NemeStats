using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.ChamionRepositoryTests
{
    [TestFixture]
    public class GetChampionTests : IntegrationTestBase
    {
        private IDataContext dataContext;
        private IGameDefinitionRetriever gameDefinitionRetriever;
        private GameDefinition gameDefinition;
        private GameDefinition inactiveChampionGameDefinition;
        private GameDefinition championlessGameDefinition;
        private int championPlayerIdForGameDefinition;
        private int otherChampionPlayerIdForGameDefinition;


        [TestFixtureSetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            dataContext = new NemeStatsDataContext();
            gameDefinitionRetriever = new GameDefinitionRetriever(dataContext);

            gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(testGameDefinitionWithOtherGamingGroupId.Id,
                0);
            championlessGameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(testGameDefinition.Id, 0);

            // Player ID 1 has a winning percentage high enough to be considered the champion
            championPlayerIdForGameDefinition = testPlayer7WithOtherGamingGroupId.Id;

            // Player ID 9 has a higher winning percentage than player 7, but is not active
            otherChampionPlayerIdForGameDefinition = testPlayer9.Id;
        }

        [Test]
        public void ItGetsThePlayerWithTheHighestWinPercentage()
        {
            // Player 7 won 75% of the GameDefinition's total games played
            Assert.That(gameDefinition.Champion.PlayerId, Is.EqualTo(championPlayerIdForGameDefinition));
        }

        [Test]
        public void AChampionMustBeActive()
        {
            // Player 8 won 100% of the GameDefinition's total games played, but is inactive
            Assert.That(otherChampionPlayerIdForGameDefinition, Is.Not.EqualTo(gameDefinition.Champion.PlayerId));
        }

        [Test]
        public void ItReturnsANullChampionIfThereIsntAChampion()
        {
            // The game definition has recorded games, but none have qualified as being champion
            Assert.That(championlessGameDefinition.Champion, Is.InstanceOf<NullChampion>());
        }

        [Test]
        public void ItSetsTheWinPercentageForTheChampion()
        {
            // The champion won 83% of all games played
            Assert.That(gameDefinition.Champion.WinPercentage, Is.EqualTo(83));
        }

        [TearDown]
        public override void FixtureTearDown()
        {
            base.FixtureTearDown();
            dataContext.Dispose();
        }
    }
}
