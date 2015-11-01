using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Service;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.OneTimeJobs
{
    public class BoardGameGeekThumbnailAttacher
    {
        public void CleanUpExistingRecords()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, new SecuredEntityValidatorFactory()))
                {
                    List<GameDefinition> gameDefinitionsNeedingUpdates = dataContext.GetQueryable<GameDefinition>()
                                                                 .Where(game => game.BoardGameGeekGameDefinitionId != null
                                                                                && game.ThumbnailImageUrl == null)
                                                                 .ToList();

                    List<int?> BoardGameGeekGameDefinitionIdsNeedingThumbnails = gameDefinitionsNeedingUpdates
                                                                   .Select(y => y.BoardGameGeekGameDefinitionId)
                                                                   .Distinct()
                                                                   .ToList();

                    var client = new BoardGameGeekClient(new ApiDownloaderService());
                    foreach (int? BoardGameGeekGameDefinitionId in BoardGameGeekGameDefinitionIdsNeedingThumbnails)
                    {
                        System.Threading.Thread.Sleep(500);
                        var gameDetails = client.GetGameDetails(BoardGameGeekGameDefinitionId.Value);

                        if (gameDetails == null || string.IsNullOrEmpty(gameDetails.Thumbnail))
                        {
                            continue;
                        }
                        int? id = BoardGameGeekGameDefinitionId;
                        var gameDefinitionsForThisId = gameDefinitionsNeedingUpdates.Where(x => x.BoardGameGeekGameDefinitionId == id.Value);

                        foreach (var gameDefinition in gameDefinitionsForThisId)
                        {
                            gameDefinition.ThumbnailImageUrl = gameDetails.Thumbnail;
                            var applicationUserWhoWillHaveSecurityToUpdate = new ApplicationUser
                            {
                                CurrentGamingGroupId = gameDefinition.GamingGroupId
                            };
                            dataContext.Save(gameDefinition, applicationUserWhoWillHaveSecurityToUpdate);
                            dataContext.CommitAllChanges();
                        }

                    }
                }
            }
        }
    }
}
