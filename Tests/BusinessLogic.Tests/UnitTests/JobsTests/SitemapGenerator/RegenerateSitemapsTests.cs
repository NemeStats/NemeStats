using BusinessLogic.Jobs.SitemapGenerator;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using NUnit.Framework;
using StructureMap.AutoMocking.Moq;

namespace BusinessLogic.Tests.UnitTests.JobsTests.SitemapGenerator
{
    [TestFixture]
    public class RegenerateSitemapsTests
    {
        private MoqAutoMocker<SitemapGeneratorService> _autoMocker;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new MoqAutoMocker<SitemapGeneratorService>();

            //_autoMocker.Get<IUniversalGameRetriever>().Setup()
        }

        [Test]

        public void It_Creates_Sitemaps_For_Universal_Game_Details()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.RegenerateSitemaps();

            //--assert
        }

    }
}
