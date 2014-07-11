using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface GameDefinitionRepository
    {
        List<GameDefinition> GetAllGameDefinitions(NemeStatsDbContext dbContext, UserContext userContext);
        GameDefinition GetGameDefinition(int gameDefinitionId, NemeStatsDbContext dbContext, UserContext userContext);
    }
}
