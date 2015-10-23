using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Service;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.GameDefinitions
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
                                                                 .Where(game => game.BoardGameGeekObjectId != null
                                                                                && game.ThumbnailImageUrl == null)
                                                                 .ToList();

                    List<int?> boardGameGeekObjectIdsNeedingThumbnails = gameDefinitionsNeedingUpdates
                                                                   .Select(y => y.BoardGameGeekObjectId)
                                                                   .Distinct()
                                                                   .ToList();

                    var client = new BoardGameGeekClient(new ApiDownloaderService());
                    foreach (int? boardGameGeekObjectId in boardGameGeekObjectIdsNeedingThumbnails)
                    {
                        System.Threading.Thread.Sleep(500);
                        var gameDetails = client.GetGameDetails(boardGameGeekObjectId.Value);

                        if (gameDetails == null || string.IsNullOrEmpty(gameDetails.Thumbnail))
                        {
                            continue;
                        }
                        int? id = boardGameGeekObjectId;
                        var gameDefinitionsForThisId = gameDefinitionsNeedingUpdates.Where(x => x.BoardGameGeekObjectId == id.Value);

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
