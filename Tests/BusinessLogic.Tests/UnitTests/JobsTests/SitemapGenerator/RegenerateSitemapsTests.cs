using System.Collections.Generic;
using BusinessLogic.Jobs.SitemapGenerator;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using NSubstitute;
using NUnit.Framework;
using NSubstituteAutoMocker;

namespace BusinessLogic.Tests.UnitTests.JobsTests.SitemapGenerator
{
    [TestFixture]
    public class RegenerateSitemapsTests
    {
        private NSubstituteAutoMocker<SitemapGeneratorService> _autoMocker;
        private List<int> _expectedUniversalGameIds;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new NSubstituteAutoMocker<SitemapGeneratorService>();

            var gameRetrieverMock = _autoMocker.Get<IUniversalGameRetriever>();

            _expectedUniversalGameIds = new List<int>
            {
                1,
                2
            };
            gameRetrieverMock.GetAllActiveBoardGameGeekGameDefinitionIds().Returns(_expectedUniversalGameIds);
        }

        [Test]

        public void It_Creates_Sitemaps_For_Universal_Game_Details()
        {
            //--arrange


            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
            _autoMocker.Get<IUniversalGameRetriever>().Received().GetAllActiveBoardGameGeekGameDefinitionIds();

        }
    }
}
