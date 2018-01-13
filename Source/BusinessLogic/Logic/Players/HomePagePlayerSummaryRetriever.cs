using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Players;

namespace BusinessLogic.Logic.Players
{
    public class HomePagePlayerSummaryRetriever : IHomePagePlayerSummaryRetriever
    {
        private readonly IPlayedGameRetriever _playedGameRetriever;
        private readonly IPlayerRetriever _playerRetriever;
        private readonly ITransformer _transformer;

        public HomePagePlayerSummaryRetriever(IPlayedGameRetriever playedGameRetriever, IPlayerRetriever playerRetriever, ITransformer transformer)
        {
            _playedGameRetriever = playedGameRetriever;
            _playerRetriever = playerRetriever;
            _transformer = transformer;
        }

        public virtual HomePagePlayerSummary GetHomePagePlayerSummaryForUser(string applicationUserId, int gamingGroupId)
        {
            var quickStats = _playerRetriever.GetPlayerQuickStatsForUser(applicationUserId, gamingGroupId);

            var homePagePlayerSummary = _transformer.Transform<HomePagePlayerSummary>(quickStats);

            var lastPlayedGameForGamingGroupList = _playedGameRetriever.GetRecentGames(1, gamingGroupId);
            if (lastPlayedGameForGamingGroupList.Count == 1)
            {
                var lastGame = lastPlayedGameForGamingGroupList[0];
                homePagePlayerSummary.LastGamingGroupPlayedGame = new PlayedGameQuickStats
                {
                    DatePlayed = lastGame.DatePlayed,
                    GameDefinitionName = lastGame.GameDefinition.Name,
                    GameDefinitionId = lastGame.GameDefinitionId,
                    PlayedGameId = lastGame.Id,
                    WinnerType = lastGame.WinnerType
                };

                if (lastGame.WinningPlayer != null)
                {
                    homePagePlayerSummary.LastGamingGroupPlayedGame.WinningPlayerId = lastGame.WinningPlayer.Id;
                    homePagePlayerSummary.LastGamingGroupPlayedGame.WinningPlayerName = lastGame.WinningPlayer.Name;
                }

                var bggGameDefinition = lastGame.GameDefinition.BoardGameGeekGameDefinition;

                if (bggGameDefinition != null)
                {
                    homePagePlayerSummary.LastGamingGroupPlayedGame.BoardGameGeekUri = BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(bggGameDefinition.Id);
                    homePagePlayerSummary.LastGamingGroupPlayedGame.ThumbnailImageUrl = bggGameDefinition.Thumbnail;
                }
            }

            return homePagePlayerSummary;
        }
    }
}
