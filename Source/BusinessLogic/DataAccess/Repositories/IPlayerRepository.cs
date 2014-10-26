using BusinessLogic.Models.Nemeses;
namespace BusinessLogic.DataAccess.Repositories
{
    public interface IPlayerRepository
    {
        NemesisData GetNemesisData(int playerId);
    }
}
