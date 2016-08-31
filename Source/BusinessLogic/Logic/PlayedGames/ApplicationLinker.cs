using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.PlayedGames
{
    public class ApplicationLinker : IApplicationLinker
    {
        public const string APPLICATION_NAME_NEMESTATS = "NemeStats";

        private readonly IDataContext _dataContext;

        public ApplicationLinker(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// LinkedPlayedGameValidator should be called before calling this method since validation can occur before saving the PlayedGame and this
        /// method requires the PlayedGame to be saved first.
        /// </summary>
        /// <param name="playedGameId"></param>
        /// <param name="applicationName"></param>
        /// <param name="entityId"></param>
        public void LinkApplication(int playedGameId, string applicationName, string entityId)
        {
            var applicationLinkage = new PlayedGameApplicationLinkage
            {
                ApplicationName = applicationName,
                EntityId = entityId,
                PlayedGameId = playedGameId
            };

            _dataContext.Save(applicationLinkage, new AnonymousApplicationUser());
        }
    }
}