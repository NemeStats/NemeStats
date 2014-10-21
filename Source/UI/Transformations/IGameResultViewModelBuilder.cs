using BusinessLogic.Models;
using System.Linq;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public interface IGameResultViewModelBuilder
    {
        GameResultViewModel Build(PlayerGameResult playerGameResult);
    }
}