using BusinessLogic.Components;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Logic.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.PlayedGames
{
    public class CreatePlayedGameComponent : TransactionalComponentBase<NewlyCompletedGame, PlayedGame>, ICreatePlayedGameComponent
    {
        private readonly ILinkedPlayedGameValidator _linkedPlayedGameValidator;
        private readonly ISecuredEntityValidator _securedEntityValidator;
        private readonly IPlayedGameSaver _playedGameSaver;

        public CreatePlayedGameComponent(
            ISecuredEntityValidator securedEntityValidator,
            ILinkedPlayedGameValidator linkedPlayedGameValidator, 
            IPlayedGameSaver playedGameSaver)
        {
            _securedEntityValidator = securedEntityValidator;
            _linkedPlayedGameValidator = linkedPlayedGameValidator;
            _playedGameSaver = playedGameSaver;
        }

        public override PlayedGame Execute(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser, IDataContext dataContext)
        {
            if (newlyCompletedGame.GamingGroupId.HasValue && newlyCompletedGame.GamingGroupId != currentUser.CurrentGamingGroupId)
            {
                _securedEntityValidator.RetrieveAndValidateAccess<GamingGroup>(newlyCompletedGame.GamingGroupId.Value, currentUser);
            }

            var gameDefinition = _securedEntityValidator.RetrieveAndValidateAccess<GameDefinition>(newlyCompletedGame.GameDefinitionId, currentUser);

            _linkedPlayedGameValidator.Validate(newlyCompletedGame);

            var gamingGroupId = newlyCompletedGame.GamingGroupId ?? currentUser.CurrentGamingGroupId;

            _playedGameSaver.ValidateAccessToPlayers(newlyCompletedGame.PlayerRanks, gamingGroupId, currentUser, dataContext);

            var playerGameResults = _playedGameSaver.MakePlayerGameResults(newlyCompletedGame, gameDefinition.BoardGameGeekGameDefinitionId, dataContext);

            var playedGame = _playedGameSaver.TransformNewlyCompletedGameIntoPlayedGame(
                newlyCompletedGame,
                gamingGroupId,
                currentUser.Id,
                playerGameResults);

            playedGame = dataContext.Save(playedGame, currentUser);

            _playedGameSaver.CreateApplicationLinkages(newlyCompletedGame.ApplicationLinkages, playedGame.Id, dataContext);

            _playedGameSaver.DoPostSaveStuff(newlyCompletedGame.TransactionSource, currentUser, playedGame.Id, playedGame.GameDefinitionId, playerGameResults,
                dataContext);

            return playedGame;
        }
    }
}
