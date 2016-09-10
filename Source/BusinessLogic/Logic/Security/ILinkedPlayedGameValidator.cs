using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.Security
{
    public interface ILinkedPlayedGameValidator
    {
        void Validate(NewlyCompletedGame newlyCompletedGame);
    }
}
