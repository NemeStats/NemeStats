using System;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Players
{
    public class PlayerDeleter : IPlayerDeleter
    {
        private readonly IDataContext _dataContext;

        public PlayerDeleter(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void DeletePlayer(int playerId, ApplicationUser currentUser)
        {
            var playerToDelete = _dataContext.GetQueryable<Player>()
                .Include(p=>p.PlayerGameResults)
                .FirstOrDefault(p=>p.Id == playerId);

            if (playerToDelete == null)
            {
                throw new ArgumentException("Player not exists", nameof(playerId));
            }
            if (playerToDelete.PlayerGameResults.Any())
            {
                throw new Exception("You can not delete players with any played game");
            }

            _dataContext.DeleteById<Player>(playerId, currentUser);
            _dataContext.CommitAllChanges();
        }
        
    }
}