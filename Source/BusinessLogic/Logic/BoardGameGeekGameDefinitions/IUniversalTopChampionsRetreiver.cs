using System.Collections.Generic;
using BusinessLogic.Models.Champions;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public interface IUniversalTopChampionsRetreiver
    {
        List<ChampionData> GetFromSource(int boardGameGeekGameDefinitionId);
    }
}