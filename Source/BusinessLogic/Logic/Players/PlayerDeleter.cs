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

            // Delete Player Achievements
            var playerAchievementsToDelete = _dataContext.GetQueryable<PlayerAchievement>()
                .Where(p => p.PlayerId == playerId)
                .ToList();

            foreach (var achieve in playerAchievementsToDelete)
            {
                var achievementId = achieve.Id;
                _dataContext.DeleteById<PlayerAchievement>(achievementId, currentUser);
            }

            // Delete Player Champions
            // TODO: Recalculate Champions for affected games?
            var playerChampionsToDelete = _dataContext.GetQueryable<Champion>()
                .Where(p => p.PlayerId == playerId)
                .ToList();

            foreach (var champion in playerChampionsToDelete)
            {
                var championId = champion.Id; 

                var gameDefinitionsWithChampionToDelete = _dataContext.GetQueryable<GameDefinition>()
                    .Where(p => p.ChampionId == championId)
                    .ToList();

                foreach (var gameDef in gameDefinitionsWithChampionToDelete)
                {
                    gameDef.ChampionId = null;
                }
               
                _dataContext.DeleteById<Champion>(championId, currentUser);
            }

            // Delete Player Nemeses
            // TODO: Recalculate Nemeses for affected players?
            var playerNemesesToDelete = _dataContext.GetQueryable<Nemesis>()
                .Where(p => p.MinionPlayerId == playerId || p.NemesisPlayerId == playerId)
                .ToList();

            foreach (var nemesisRecord in playerNemesesToDelete)
            {
                var nemesisId = nemesisRecord.Id;

                var playersToUpdatePreviousNemesis = _dataContext.GetQueryable<Player>()
                    .Where(p => p.PreviousNemesisId == nemesisId)
                    .ToList();

                foreach (var player in playersToUpdatePreviousNemesis)
                {
                    player.PreviousNemesisId = null;
                }

                var playersToUpdateCurrentNemesis = _dataContext.GetQueryable<Player>()
                    .Where(p => p.NemesisId == nemesisId)
                    .ToList();

                foreach (var player in playersToUpdateCurrentNemesis)
                {
                    player.NemesisId = null;
                }

                _dataContext.DeleteById<Nemesis>(nemesisId, currentUser);
            }



            // Delete Player
            _dataContext.DeleteById<Player>(playerId, currentUser);
            _dataContext.CommitAllChanges();
        }
        
    }
}