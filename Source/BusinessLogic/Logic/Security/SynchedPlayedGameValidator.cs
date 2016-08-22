using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.Security
{
    public class SynchedPlayedGameValidator : ISynchedPlayedGameValidator
    {
        private readonly IDataContext _dataContext;

        public SynchedPlayedGameValidator(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Validate(NewlyCompletedGame newlyCompletedGame)
        {
            if (string.IsNullOrEmpty(newlyCompletedGame.ExternalSourceApplicationName))
            {
                return;
            }

            if (string.IsNullOrEmpty(newlyCompletedGame.ExternalSourceEntityId))
            {
                return;
            }

            var entityWithThisSynchKeyAlreadyExists = _dataContext.GetQueryable<PlayedGame>()
                .Any(x => x.ExternalSourceApplicationName == newlyCompletedGame.ExternalSourceApplicationName
                          && x.ExternalSourceEntityId == newlyCompletedGame.ExternalSourceEntityId
                          && x.GamingGroupId == newlyCompletedGame.GamingGroupId);

            if (entityWithThisSynchKeyAlreadyExists)
            {
                throw new EntityAlreadySynchedException(newlyCompletedGame, newlyCompletedGame.GamingGroupId.Value);
            }
        }
    }
}