using System.Linq;
using BusinessLogic.Logic.GamingGroups;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit2;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.GamingGroupTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetTopGamingGroupsTests
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        //[Test, AutoData]
        public void ItReturnsTheSpecifiedNumberOfResults(GamingGroupRetriever retriever, int numberOfGamingGroups)
        {
            var results = retriever.GetTopGamingGroups(numberOfGamingGroups);


        }
    }
}
