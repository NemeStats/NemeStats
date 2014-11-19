using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public static class BoardGameGeekUriBuilder
    {
        public const string BOARD_GAME_GEEK_BOARD_GAME_BASE_URI = "http://boardgamegeek.com/boardgame/{0}";

        public static Uri BuildBoardGameGeekGameUri(int? boardGameGeekBoardGameObjectId)
        {
            if (boardGameGeekBoardGameObjectId.HasValue)
            {
                return new Uri(string.Format(BOARD_GAME_GEEK_BOARD_GAME_BASE_URI, boardGameGeekBoardGameObjectId));
            }

            return null;
        }
    }
}
