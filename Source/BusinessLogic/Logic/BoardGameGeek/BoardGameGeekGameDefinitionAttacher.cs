using BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekGameDefinitionAttacher : IBoardGameGeekGameDefinitionAttacher
    {
        private IDataContext dataContext;

        public BoardGameGeekGameDefinitionAttacher(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public int? CreateBoardGameGeekGameDefinition(int? boardGameGeekGameDefinitionId)
        {
            throw new NotImplementedException();
        }
    }
}
