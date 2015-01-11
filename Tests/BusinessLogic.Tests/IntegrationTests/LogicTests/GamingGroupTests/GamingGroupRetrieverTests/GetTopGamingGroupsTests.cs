using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.NUnit2;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.GamingGroupTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetTopGamingGroupsTests
    {
        //private IDataContext dataContext;
        //private IPlayerRetriever playerRetriever;
        //private IGameDefinitionRetriever gameDefinitionRetriever;
        //private IPlayedGameRetriever playedGameRetriever;

        //[SetUp]
        //public void SetUp()
        //{
        //    dataContext = MockRepository.GenerateMock<IDataContext>();
        //    playerRetriever = MockRepository.GenerateMock<IPlayerRetriever>();
        //    gameDefinitionRetriever = MockRepository.GenerateMock<IGameDefinitionRetriever>();
        //    playedGameRetriever = MockRepository.GenerateMock<IPlayedGameRetriever>();
        //}

        //[Test]
        //public void ItReturnsTheSpecifiedNumberOfResults()
        //{
        //    //var autoMocker = new RhinoAutoMocker<GamingGroupRetriever>(MockMode.AAA);
        //    GamingGroupRetriever retriever = new GamingGroupRetriever();

        //    //var results = retriever.GetTopGamingGroups(1);


        //}
    }
}
