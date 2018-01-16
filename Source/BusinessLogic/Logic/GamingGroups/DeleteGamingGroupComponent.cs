using System.Linq;
using BusinessLogic.Components;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.GamingGroups
{
    public class DeleteGamingGroupComponent : TransactionalComponentBase<int>, IDeleteGamingGroupComponent
    {
        private readonly IDataContext _dataContext;
        private readonly ISecuredEntityValidator _securedEntityValidator;

        public DeleteGamingGroupComponent(IDataContext dataContext, ISecuredEntityValidator securedEntityValidator)
        {
            _dataContext = dataContext;
            _securedEntityValidator = securedEntityValidator;
        }

        public override void Execute(int gamingGroupId, ApplicationUser currentUser, IDataContext dataContext)
        {
            _dataContext.SetCommandTimeout(300);

            var gamingGroup =
                _securedEntityValidator.RetrieveAndValidateAccess<GamingGroup>(gamingGroupId, currentUser);

            DeletePlayerGameResults(gamingGroupId, currentUser);
            DeletePlayedGames(gamingGroupId, currentUser);
            DeletePlayerAchievements(gamingGroupId, currentUser);
            DeleteChampionsAndGameDefinitions(gamingGroupId, currentUser);
            DeleteNemeses(gamingGroupId, currentUser);
            DeletePlayers(gamingGroup, currentUser);
        }

        internal virtual void DeletePlayerGameResults(int gamingGroupId, ApplicationUser currentUser)
        {
            var playerGameResultIds = _dataContext.GetQueryable<PlayerGameResult>()
                .Where(x => x.PlayedGame.GamingGroupId == gamingGroupId)
                .Select(x => x.Id)
                .ToList();
            playerGameResultIds.ForEach(x => _dataContext.DeleteById<PlayerGameResult>(x, currentUser));
        }

        internal virtual void DeletePlayedGames(int gamingGroupId, ApplicationUser currentUser)
        {
            var playedGames = _dataContext.GetQueryable<PlayedGame>()
                .Where(x => x.GamingGroupId == gamingGroupId)
                .Select(x => x.Id)
                .ToList();

            playedGames.ForEach(x => _dataContext.DeleteById<PlayedGame>(x, currentUser));
        }

        internal virtual void DeletePlayerAchievements(int gamingGroupId, ApplicationUser currentUser)
        {
            var playerGameResultIds = _dataContext.GetQueryable<PlayerAchievement>()
                .Where(x => x.Player.GamingGroupId == gamingGroupId)
                .Select(x => x.Id)
                .ToList();
            playerGameResultIds.ForEach(x => _dataContext.DeleteById<PlayerAchievement>(x, currentUser));
        }

        internal virtual void DeleteChampionsAndGameDefinitions(int gamingGroupId, ApplicationUser currentUser)
        {
            _dataContext.MakeScarySqlAlteration("ALTER TABLE GameDefinition NOCHECK CONSTRAINT ALL");

            var gameDefinitions = _dataContext.GetQueryable<GameDefinition>()
                .Where(x => x.GamingGroupId == gamingGroupId)
                .ToList();

            foreach (var gameDefinition in gameDefinitions)
            {
                gameDefinition.ChampionId = null;
                gameDefinition.PreviousChampionId = null;
                _dataContext.Save(gameDefinition, currentUser);
            }

            var championIds = _dataContext.GetQueryable<Champion>()
                .Where(x => x.Player.GamingGroupId == gamingGroupId || x.GameDefinition.GamingGroupId == gamingGroupId)
                .Select(x => x.Id)
                .ToList();

            championIds.ForEach(x => _dataContext.DeleteById<Champion>(x, currentUser));

            gameDefinitions.ForEach(x => _dataContext.DeleteById<GameDefinition>(x, currentUser));

            _dataContext.MakeScarySqlAlteration("ALTER TABLE GameDefinition WITH CHECK CHECK CONSTRAINT ALL");
        }

        internal virtual void DeleteNemeses(int gamingGroupId, ApplicationUser currentUser)
        {
            var players = _dataContext.GetQueryable<Player>()
                .Where(x => x.GamingGroupId == gamingGroupId
                            && (x.PreviousNemesisId != null || x.NemesisId != null))
                .ToList();

            foreach (var player in players)
            {
                player.PreviousNemesisId = null;
                player.NemesisId = null;
                _dataContext.Save(player, currentUser);
            }

            var nemesisIds = _dataContext.GetQueryable<Nemesis>()
                .Where(x => x.MinionPlayer.GamingGroupId == gamingGroupId ||
                            x.NemesisPlayer.GamingGroupId == gamingGroupId)
                .Select(x => x.Id)
                .ToList();

            nemesisIds.ForEach(x => _dataContext.DeleteById<Nemesis>(x, currentUser));
        }

        internal virtual void DeletePlayers(GamingGroup gamingGroup, ApplicationUser currentUser)
        {
            gamingGroup.GamingGroupChampionPlayerId = null;
            _dataContext.Save(gamingGroup, currentUser);

            var playerIds = _dataContext.GetQueryable<Player>()
                .Where(x => x.GamingGroupId == gamingGroup.Id)
                .Select(x => x.Id)
                .ToList();

            playerIds.ForEach(x => _dataContext.DeleteById<Player>(x, currentUser));
        }

        internal virtual void UnassociateUsers(int gamingGroupId, ApplicationUser currentUser)
        {
            throw new System.NotImplementedException();
        }

        internal virtual void DeleteGamingGroupInvitations(int gamingGroupId, ApplicationUser currentUser)
        {
            throw new System.NotImplementedException();
        }

        internal virtual void DeleteGamingGroup(int gamingGroupId, ApplicationUser currentUser)
        {
            throw new System.NotImplementedException();
        }
    }
}