using System.Diagnostics;
using BoardGameGeekApiClient.Service;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Jobs.BoardGameGeekCleanUpService;
using NUnit.Framework;
using Rhino.Mocks;
using RollbarSharp;

namespace BusinessLogic.Tests.IntegrationTests.JobsTests.BoardGameGeekBatchUpdateServiceTests
{
    [TestFixture]
    public class RefreshAllBoardGameGeekDataIntegrationTest
    {
        [Test, Ignore("This will do a major update to the database and will run for a long time.")]
        public void UpdateAllBoardGameGeekGameDefinitionData()
        {
            using (NemeStatsDbContext nemeStatsDbContext = new NemeStatsDbContext())
            {
                using (var dataContext = new NemeStatsDataContext(nemeStatsDbContext, new SecuredEntityValidatorFactory()))
                {
                    var apiDownloaderService = new ApiDownloaderService();
                    //API failures won't get logged!
                    var rollbarClient = MockRepository.GenerateMock<IRollbarClient>();
                    var boardGameGeekClient = new BoardGameGeekClient(apiDownloaderService, rollbarClient);
                    var batchUpdateService = new BoardGameGeekBatchUpdateService(dataContext, boardGameGeekClient, rollbarClient);

                    var totalRecordsUpdated = batchUpdateService.RefreshAllBoardGameGeekData();

                    Debug.WriteLine("Updated {0} total BoardGameGeekGameDefinition records.", totalRecordsUpdated);
                }
            }
  
        }
    }
}
