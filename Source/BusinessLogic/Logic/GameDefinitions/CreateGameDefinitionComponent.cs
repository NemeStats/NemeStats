using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Components;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class CreateGameDefinitionComponent : TransactionalComponentBase<CreateGameDefinitionRequest, GameDefinition>, ICreateGameDefinitionComponent
    {
        public override GameDefinition Execute(CreateGameDefinitionRequest inputParameter, ApplicationUser currentUser, IDataContext dataContextWithTransaction)
        {
            throw new NotImplementedException();
        }
    }
}
