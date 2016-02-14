using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public interface IBoardGameGeekGamesImporter
    {
        int? ImportBoardGameGeekGames(ApplicationUser applicationUser);
    }
}