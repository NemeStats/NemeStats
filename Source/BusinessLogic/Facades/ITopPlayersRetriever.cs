using System.Collections.Generic;
using BusinessLogic.Models.Players;

namespace BusinessLogic.Facades
{
    public interface ITopPlayersRetriever
    {
        List<TopPlayer> GetResults(int numberOfPlayersToRetrieve);
    }
}
