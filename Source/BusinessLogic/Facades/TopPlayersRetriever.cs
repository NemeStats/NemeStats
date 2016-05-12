using System.Collections.Generic;
using BusinessLogic.Caching;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Players;

namespace BusinessLogic.Facades
{
    public class TopPlayersRetriever : Cacheable<int, List<TopPlayer>>,  ITopPlayersRetriever
    {
        private readonly IPlayerSummaryBuilder _playerSummaryBuilder;

        public TopPlayersRetriever(IPlayerSummaryBuilder playerSummaryBuilder, IDateUtilities dateUtilities, INemeStatsCacheManager cacheManager) : base(dateUtilities, cacheManager)
        {
            _playerSummaryBuilder = playerSummaryBuilder;
        }

        public override List<TopPlayer> GetFromSource(int numberOfPlayersToRetrieve)
        {
            return _playerSummaryBuilder.GetTopPlayers(numberOfPlayersToRetrieve);
        }
    }
}