using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class GameDefinitionRetrieverImpl : GameDefinitionRetriever
    {
        protected ApplicationDataContext dataContext;

        public GameDefinitionRetrieverImpl(ApplicationDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IList<GameDefinition> GetAllGameDefinitions(ApplicationUser currentUser)
        {
            return dataContext.GetQueryable<GameDefinition>().ToList();
        }
    }
}
