using System.Linq;
using BusinessLogic.Components;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Events;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Exceptions;
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
        private readonly IBusinessLogicEventSender _businessLogicEventSender;

        public CreatePlayedGameComponent(
            ISecuredEntityValidator securedEntityValidator,
            ILinkedPlayedGameValidator linkedPlayedGameValidator, 
            IPlayedGameSaver playedGameSaver, 
            IBusinessLogicEventSender businessLogicEventSender)
        {
            _securedEntityValidator = securedEntityValidator;
            _linkedPlayedGameValidator = linkedPlayedGameValidator;
            _playedGameSaver = playedGameSaver;
            _businessLogicEventSender = businessLogicEventSender;
        }

        public override PlayedGame Execute(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser, IDataContext dataContext)
        {
            var gamingGroupId = ValidateAndGetGamingGroupId(newlyCompletedGame.GamingGroupId, currentUser);

            var gameDefinition = _securedEntityValidator.RetrieveAndValidateAccess<GameDefinition>(newlyCompletedGame.GameDefinitionId, currentUser);

            _linkedPlayedGameValidator.Validate(newlyCompletedGame);

            _playedGameSaver.ValidateAccessToPlayers(newlyCompletedGame.PlayerRanks, gamingGroupId, currentUser, dataContext);

            var playerGameResults = _playedGameSaver.MakePlayerGameResults(newlyCompletedGame, gameDefinition.BoardGameGeekGameDefinitionId, dataContext);

            var playedGame = _playedGameSaver.TransformNewlyCompletedGameIntoPlayedGame(
                newlyCompletedGame,
                gamingGroupId,
                currentUser.Id,
                playerGameResults);

            playedGame = dataContext.Save(playedGame, currentUser);

            _playedGameSaver.CreateApplicationLinkages(newlyCompletedGame.ApplicationLinkages, playedGame.Id, dataContext);

            PostExecuteAction = () =>
            {
                return _businessLogicEventSender.SendEvent(
                        new PlayedGameCreatedEvent(playedGame.Id,
                            playedGame.GameDefinitionId,
                            playerGameResults.Select(x => x.PlayerId).ToList(),
                            newlyCompletedGame.TransactionSource,
                            currentUser));
            };

            return playedGame;
        }

        private int ValidateAndGetGamingGroupId(int? requestedGamingGroupId, ApplicationUser applicationUser)
        {
            if (requestedGamingGroupId.HasValue)
            {
                return requestedGamingGroupId.Value;
            }

            if (applicationUser.CurrentGamingGroupId.HasValue)
            {
                return applicationUser.CurrentGamingGroupId.Value;
            }

            throw new NoValidGamingGroupException(applicationUser.Id);
        }
    }
}
