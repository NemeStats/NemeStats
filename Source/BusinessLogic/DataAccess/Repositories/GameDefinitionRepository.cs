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
        List<GameDefinition> GetAllGameDefinitions(UserContext userContext);
        GameDefinition GetGameDefinition(int gameDefinitionId, UserContext userContext);
        GameDefinition Save(GameDefinition gameDefinition, UserContext userContext);
        void Delete(int gameDefinitionId, UserContext userContext);
    }
}
