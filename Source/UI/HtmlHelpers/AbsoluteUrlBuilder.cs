using System.Configuration;

namespace UI.HtmlHelpers
{
    public static class AbsoluteUrlBuilder
    {
        private static readonly string _baseUrl;
        
        static AbsoluteUrlBuilder()
        {
            _baseUrl = ConfigurationManager.AppSettings["urlRoot"];
        }

        public static string GetGamingGroupDetailsUrl(int gamingGroupId)
        {
            return $"{_baseUrl}/{ MVC.GamingGroup.Name}/{ MVC.GamingGroup.ActionNames.Details}/{gamingGroupId}";
        }

        public static string GetPlayerDetailsUrl(int playerId)
        {
            return $"{_baseUrl}/{ MVC.Player.Name}/{ MVC.Player.ActionNames.Details}/{playerId}";
        }

        public static string GetPlayedGameDetailsUrl(int playedGameId)
        {
            return $"{_baseUrl}/{ MVC.PlayedGame.Name}/{ MVC.PlayedGame.ActionNames.Details}/{playedGameId}";
        }
    }
}