using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetGameDefinitionDetailsTests : GameDefinitionRetrieverTestBase
    {
        //TODO integration testing instead. Will delete this if it pans out.
        //private List<GameDefinition> gameDefinitionsToQuery;

        //[Test]
        //public override void SetUp()
        //{
        //    base.SetUp();

        //    gameDefinitionsToQuery = new List<GameDefinition>();
        //    GameDefinition gameDefinition = new GameDefinition()
        //    {
        //        Id = 3,
        //        Name = "game definition name",
        //        Description = "game definition description",
        //        PlayedGames = new List<PlayedGame>()
        //    };
        //    gameDefinition.PlayedGames.Add(new PlayedGame(){ Id = 1,  })
        //    gameDefinitionsToQuery.Add(gameDefinition);
        //}

        //[Test]
        //public void ItGetsTheGameDefinitionDetails()
        //{
        //    int id = 1;
       
        //    GameDefinition gameDefinition = retriever.GetGameDefinitionDetails(id, currentUser);
        //}
    }
}
