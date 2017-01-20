using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.MVP
{
    public interface IMVPRecalculator
    {
        Models.MVP RecalculateMVP(int gameDefinitionId, ApplicationUser applicationUser, bool allowedToClearExistingMVP = true);
    }
}