using System;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;

namespace BusinessLogic.Logic.Champions
{
    public class GamingGroupChampionRecalculator : IGamingGroupChampionRecalculator
    {
        public const int MINIMUM_POINTS_TO_BE_GAMING_GROUP_CHAMPION = 250;

        private readonly IDataContext _dataContext;

        public GamingGroupChampionRecalculator(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void RecalculateGamingGroupChampion(int playedGameId)
        {
            int gamingGroupId = _dataContext.GetQueryable<PlayerGameResult>()
                .Where(x => x.PlayedGameId == playedGameId)
                .Select(x => x.PlayedGame.GamingGroupId)
                .FirstOrDefault();

            if (gamingGroupId == default(int))
            {
                throw new ArgumentException($"PlayedGame with id '{playedGameId}' does not exist.");
            }

            var topPlayer = (from playerGameResult in _dataContext.GetQueryable<PlayerGameResult>()
                    where playerGameResult.PlayedGame.GamingGroupId == gamingGroupId
                    group playerGameResult by playerGameResult.PlayerId
                    into groupedResults
                    select
                    new
                    {
                        TotalPoints = groupedResults.Sum(x => x.TotalPoints),
                        PlayerId = groupedResults.Key
                    })
                    .Where(x => x.TotalPoints >= MINIMUM_POINTS_TO_BE_GAMING_GROUP_CHAMPION)
                .OrderByDescending(r => r.TotalPoints)
                .ThenBy(x => x.PlayerId)
                .FirstOrDefault();

            if (topPlayer != null)
            {
                var gamingGroup = _dataContext.GetQueryable<GamingGroup>().First(x => x.Id == gamingGroupId);
                gamingGroup.GamingGroupChampionPlayerId = topPlayer.PlayerId;
                _dataContext.AdminSave(gamingGroup);
            }
        }
    }
}