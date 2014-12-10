using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Models.User;
using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.ChampionsTests
{
    [TestFixture]
    public class RecalculateChampionTests
    {
        [Test, Ignore("Integration tests")]
        public void RecalculateForSingleGame()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                SecuredEntityValidatorFactory factory = new SecuredEntityValidatorFactory();

                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, factory))
                {
                    IChampionRepository championRepository = new ChampionRepository(dataContext);

                    IChampionRecalculator championRecalculator = new ChampionRecalculator(dataContext, championRepository);
                    ApplicationUser user = new ApplicationUser
                    {
                        Id = "80629c07-b8df-4deb-a9e3-5b503ce7d7df",
                        CurrentGamingGroupId = 1
                    };
                    championRecalculator.RecalculateChampion(2005, user);
                }
            }
        }
    }
}
