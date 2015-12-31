using BusinessLogic.Logic.OneTimeJobs;
using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.BoardGameGeekThumbnailAttacherTests
{
    [TestFixture, Category("Integration")]
    public class CleanUpExistingRecordsTests
    {
        [Test]
        [Ignore("Does a huge amount of updates to the database. Run only very deliberately.")]
        public void DoIt()
        {
            //new BoardGameGeekThumbnailAttacher().CleanUpExistingRecords();
        }

    }
}
