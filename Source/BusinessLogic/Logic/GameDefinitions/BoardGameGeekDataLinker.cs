using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class BoardGameGeekDataLinker
    {
        public void CleanUpExistingRecords()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, new SecuredEntityValidatorFactory()))
                {
                    List<GameDefinition> games = dataContext.GetQueryable<GameDefinition>()
                                                            .Where(game => game.BoardGameGeekObjectId == null)
                                                            .ToList();

                    BoardGameGeekSearcher bggSearcher = new BoardGameGeekSearcher();
                    int updateCount = 0;

                    foreach (GameDefinition game in games)
                    {
                        List<BoardGameGeekSearchResult> bggResults = bggSearcher.SearchForBoardGames(game.Name.Trim());

                        if (bggResults.Count == 1)
                        {
                            game.BoardGameGeekObjectId = bggResults[0].BoardGameId;
                            ApplicationUser user = new ApplicationUser
                            {
                                CurrentGamingGroupId = game.GamingGroupId
                            };
                            dataContext.Save(game, user);
                            dataContext.CommitAllChanges();
                            Console.WriteLine(game.Name + " had exactly one match and was updated.");
                            updateCount++;
                        }
                    }

                    Console.WriteLine("Updated " + updateCount + " records.");
                }
            }
        }
    }
}
