using System.Collections.Generic;
using BusinessLogic.Models.Champions;

namespace BusinessLogic.Logic.Champions
{
    public interface IRecentChampionRetriever
    {
        IList<ChampionChange> GetRecentChampionChanges(GetRecentChampionChangesFilter filter);
    }
}
