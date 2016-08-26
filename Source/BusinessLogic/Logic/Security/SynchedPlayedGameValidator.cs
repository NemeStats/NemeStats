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
            foreach (var applicationLinkage in newlyCompletedGame.ApplicationLinkages)
            {
                if (string.IsNullOrEmpty(applicationLinkage.ApplicationName) || string.IsNullOrEmpty(applicationLinkage.EntityId))
                {
                    throw new InvalidSourceException(applicationLinkage.ApplicationName, applicationLinkage.EntityId);
                }

                var entityWithThisSynchKeyAlreadyExists = _dataContext.GetQueryable<PlayedGameApplicationLinkage>()
                    .Any(x => x.ApplicationName == applicationLinkage.ApplicationName
                        && x.EntityId == applicationLinkage.EntityId
                        && x.PlayedGame.GamingGroupId == newlyCompletedGame.GamingGroupId);

                if (entityWithThisSynchKeyAlreadyExists)
                {
                    throw new EntityAlreadySynchedException(applicationLinkage.ApplicationName, applicationLinkage.EntityId, newlyCompletedGame.GamingGroupId.Value);
                }
            }
        }
    }
}