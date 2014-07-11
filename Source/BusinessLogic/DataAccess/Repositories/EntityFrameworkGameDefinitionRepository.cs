using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.DataAccess.Repositories
{
    public class EntityFrameworkGameDefinitionRepository : GameDefinitionRepository
    {
        private NemeStatsDbContext dbContext;

        public EntityFrameworkGameDefinitionRepository(NemeStatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }



        public List<GameDefinition> GetAllGameDefinitions(NemeStatsDbContext dbContext, UserContext userContext)
        {
            return dbContext.GameDefinitions
                .Where(game => game.GamingGroupId == userContext.GamingGroupId)
                .ToList();
        }
    }
}
