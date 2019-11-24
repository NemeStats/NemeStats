#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using System;
using System.Linq;
using BoardGameGeekApiClient.Service;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using RollbarSharp;

namespace BusinessLogic.Logic.OneTimeJobs
{
    public class BoardGameGeekDataLinker
    {
        public void CleanUpExistingRecords()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, new SecuredEntityValidatorFactory()))
                {
                    var games = dataContext.GetQueryable<GameDefinition>()
                                                            .Where(game => game.BoardGameGeekGameDefinitionId == null)
                                                            .ToList();

                    var bggSearcher = new BoardGameGeekClient(new ApiDownloaderService(), new RollbarClient());
                    int updateCount = 0;

                    foreach (GameDefinition game in games)
                    {
                        var bggResults = bggSearcher.SearchBoardGames(game.Name.Trim(), true);

                        if (bggResults.Count == 1)
                        {
                            game.BoardGameGeekGameDefinitionId = bggResults.First().BoardGameId;
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
