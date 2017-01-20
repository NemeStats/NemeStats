using BusinessLogic.Models.MVPModels;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface IMVPRepository
    {
        MVPData GetMVPData(int gameDefinitionId);
    }
}