using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.Security
{
    public interface ISynchedPlayedGameValidator
    {
        void Validate(NewlyCompletedGame newlyCompletedGame);
    }
}
