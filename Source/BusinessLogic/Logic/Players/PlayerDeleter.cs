using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper.Internal;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Players
{
    public class PlayerDeleter : IPlayerDeleter
    {
        private readonly IDataContext _dataContext;
        private readonly IChampionRecalculator _championRecalculator;
        private readonly INemesisRecalculator _nemesisRecalculator;

        public PlayerDeleter(IDataContext dataContext, IChampionRecalculator championRecalculator, INemesisRecalculator nemesisRecalculator)
        {
            _dataContext = dataContext;
            _championRecalculator = championRecalculator;
            _nemesisRecalculator = nemesisRecalculator;
        }

        public void DeletePlayer(int playerId, ApplicationUser currentUser)
        {
            ValidatePlayer(playerId);

            DeletePlayerAchievements(playerId, currentUser);

            DeleteChampionRecords(playerId, currentUser);

            DeletePlayerNemesesRecords(playerId, currentUser);

            _dataContext.DeleteById<Player>(playerId, currentUser);

            _dataContext.CommitAllChanges();
        }

        private void ValidatePlayer(int playerId)
        {
            var playerToDelete = _dataContext.GetQueryable<Player>()
                .Include(p => p.PlayerGameResults)
                .FirstOrDefault(p => p.Id == playerId);

            if (playerToDelete == null)
            {
                throw new ArgumentException("Player not exists", nameof(playerId));
            }
            if (playerToDelete.PlayerGameResults.Any())
            {
                throw new Exception("You can not delete players with any played game");
            }
        }

        private void DeletePlayerAchievements(int playerId, ApplicationUser currentUser)
        {
            var playerAchievementIds = _dataContext.GetQueryable<PlayerAchievement>()
                 .Where(p => p.PlayerId == playerId)
                .Select(p => p.Id)
                .ToList();

            playerAchievementIds.ForEach(x => _dataContext.DeleteById<PlayerAchievement>(x, currentUser));
        }

        private void DeleteChampionRecords(int playerId, ApplicationUser currentUser)
        {
            var gameDefinitionIdsThatNeedChampionRecalculated = new HashSet<int>();
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
                    //--clear out the current champion since recalculating may not clear out the current Champion
                    gameDef.ChampionId = null;
                    _dataContext.Save(gameDef, currentUser);

                    gameDefinitionIdsThatNeedChampionRecalculated.Add(gameDef.Id);
                }

                _dataContext.DeleteById<Champion>(championId, currentUser);
            }

            RecalculateChampions(gameDefinitionIdsThatNeedChampionRecalculated, currentUser);
        }

        private void RecalculateChampions(IEnumerable<int> gameDefinitionIds, ApplicationUser currentUser)
        {
            gameDefinitionIds.Each(x => _championRecalculator.RecalculateChampion(x, currentUser, _dataContext));
        }

        private void DeletePlayerNemesesRecords(int playerId, ApplicationUser currentUser)
        {
            var playerIdsThatNeedNewNemesis = new HashSet<int>();
            var playerNemesesToDelete = _dataContext.GetQueryable<Nemesis>()
                .Where(p => p.MinionPlayerId == playerId || p.NemesisPlayerId == playerId)
                .ToList();

            foreach (var nemesisRecord in playerNemesesToDelete)
            {
                var nemesisId = nemesisRecord.Id;

                ClearOutPreviousNemesis(nemesisId, currentUser);

                ClearOutCurrentNemesis(nemesisId, currentUser, playerIdsThatNeedNewNemesis);

                _dataContext.DeleteById<Nemesis>(nemesisId, currentUser);
            }

            RecalculateNemesis(playerIdsThatNeedNewNemesis, currentUser);
        }

        private void ClearOutPreviousNemesis(int nemesisId, ApplicationUser currentUser)
        {
            var playersToUpdatePreviousNemesis = _dataContext.GetQueryable<Player>()
                .Where(p => p.PreviousNemesisId == nemesisId)
                .ToList();

            foreach (var player in playersToUpdatePreviousNemesis)
            {
                player.PreviousNemesisId = null;
                _dataContext.Save(player, currentUser);
            }
        }

        private void ClearOutCurrentNemesis(int nemesisId, ApplicationUser currentUser, HashSet<int> playerIdsToClear)
        {
            var playersToUpdateCurrentNemesis = _dataContext.GetQueryable<Player>()
                .Where(p => p.NemesisId == nemesisId)
                .ToList();

            foreach (var player in playersToUpdateCurrentNemesis)
            {
                player.NemesisId = null;
                _dataContext.Save(player, currentUser);
                playerIdsToClear.Add(player.Id);
            }
        }

        private void RecalculateNemesis(IEnumerable<int> playerIdsThatNeedNewNemesis, ApplicationUser currentUser)
        {
            playerIdsThatNeedNewNemesis.Each(x => _nemesisRecalculator.RecalculateNemesis(x, currentUser, _dataContext));
        }
    }
}