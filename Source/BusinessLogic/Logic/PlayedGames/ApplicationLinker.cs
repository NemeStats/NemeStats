using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.PlayedGames
{
    public class ApplicationLinker : IApplicationLinker
    {
        public const string APPLICATION_NAME_NEMESTATS = "NemeStats";

        /// <summary>
        /// LinkedPlayedGameValidator should be called before calling this method since validation can occur before saving the PlayedGame and this
        /// method requires the PlayedGame to be saved first.
        /// </summary>
        /// <param name="playedGameId"></param>
        /// <param name="applicationName"></param>
        /// <param name="entityId"></param>
        /// <param name="dataContext"></param>
        public void LinkApplication(int playedGameId, string applicationName, string entityId, IDataContext dataContext)
        {
            var applicationLinkage = new PlayedGameApplicationLinkage
            {
                ApplicationName = applicationName,
                EntityId = entityId,
                PlayedGameId = playedGameId
            };

            dataContext.Save(applicationLinkage, new AnonymousApplicationUser());
        }
    }
}