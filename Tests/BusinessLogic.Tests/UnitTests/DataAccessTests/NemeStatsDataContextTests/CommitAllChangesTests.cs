using System.Linq;
using BusinessLogic.DataAccess;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class CommitAllChangesTests
    {
        private RhinoAutoMocker<NemeStatsDataContext> autoMocker;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<NemeStatsDataContext>();
            autoMocker.Inject(MockRepository.GenerateMock<NemeStatsDbContext>());
        }

        [Test]
        public void ItSavesAllChangesOnTheDbContext()
        {
            autoMocker.ClassUnderTest.CommitAllChanges();

            autoMocker.Get<NemeStatsDbContext>().AssertWasCalled(mock => mock.SaveChanges());
        }
    }
}
