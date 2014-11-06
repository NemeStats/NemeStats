using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public interface IBoardGameGeekSearcher
    {
        List<BoardGameGeekSearchResult> SearchForBoardGames(string searchText, bool exactMatch);
    }
}
