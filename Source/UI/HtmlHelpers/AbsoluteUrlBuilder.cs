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
    }
}