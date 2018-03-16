using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Champions;

namespace BusinessLogic.Logic.Champions
{
    public class RecentChampionRetriever : IRecentChampionRetriever
    {
        private readonly IDataContext _dataContext;

        public RecentChampionRetriever(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IList<ChampionChange> GetRecentChampionChanges(GetRecentChampionChangesFilter filter)
        {
            var filterDate = DateTime.UtcNow.AddDays(-1 * filter.NumberOfRecentChangesToShow);
            return _dataContext.GetQueryable<GameDefinition>()
                .Where(x => x.Active
                            && x.GamingGroupId == filter.GamingGroupId
                            && x.ChampionId.HasValue)
                .Select(x => new ChampionChange
                {
                    NewChampionPlayerId = x.Champion.PlayerId,
                    NewChampionPlayerName = x.Champion.Player.Name,
                    DateCreated = x.Champion.DateCreated,
                    PreviousChampionPlayerId = x.PreviousChampion.PlayerId,
                    PreviousChampionPlayerName = x.PreviousChampion.Player.Name,
                    GameDefinitionId = x.Id,
                    GameName = x.Name
                })
                .OrderByDescending(x => x.DateCreated)
                .Take(filter.NumberOfRecentChangesToShow)
                .ToList();
        }
    }
}