using BusinessLogic.Models.MVPData;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface IMVPRepository
    {
        MVPData GetMVPData(int gameDefinitionId);
    }
}