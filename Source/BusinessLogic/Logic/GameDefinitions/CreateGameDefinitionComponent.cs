using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Components;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.GameDefinitions
{
    public class CreateGameDefinitionComponent : TransactionalComponentBase<CreateGameDefinitionRequest, GameDefinition>, ICreateGameDefinitionComponent
    {
        private readonly INemeStatsEventTracker _eventTracker;
        private readonly IBoardGameGeekGameDefinitionCreator _boardGameGeekGameDefinitionCreator;

        public CreateGameDefinitionComponent(INemeStatsEventTracker eventTracker, IBoardGameGeekGameDefinitionCreator boardGameGeekGameDefinitionCreator)
        {
            _eventTracker = eventTracker;
            _boardGameGeekGameDefinitionCreator = boardGameGeekGameDefinitionCreator;
        }

        public override GameDefinition Execute(CreateGameDefinitionRequest createGameDefinitionRequest, ApplicationUser currentUser, IDataContext dataContextWithTransaction)
        {
            ValidateNotNull(createGameDefinitionRequest);
            ValidateGameDefinitionNameIsNotNullOrWhitespace(createGameDefinitionRequest.Name);

            var gamingGroupId = ValidateAndGetGamingGroupId(createGameDefinitionRequest.GamingGroupId, currentUser);

            BoardGameGeekGameDefinition boardGameGeekGameDefinition = null;
            if (createGameDefinitionRequest.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekGameDefinition = _boardGameGeekGameDefinitionCreator.CreateBoardGameGeekGameDefinition(
                    createGameDefinitionRequest.BoardGameGeekGameDefinitionId.Value);
            }

            var existingGameDefinition = dataContextWithTransaction.GetQueryable<GameDefinition>()
                .FirstOrDefault(game => game.GamingGroupId == gamingGroupId
                        && game.Name == createGameDefinitionRequest.Name);

            if (existingGameDefinition == null)
            {
                var newGameDefinition = new GameDefinition
                {
                    Name = createGameDefinitionRequest.Name,
                    BoardGameGeekGameDefinitionId = boardGameGeekGameDefinition?.Id,
                    Description = createGameDefinitionRequest.Description,
                    GamingGroupId = gamingGroupId
                };

                new Task(() => _eventTracker.TrackGameDefinitionCreation(currentUser, createGameDefinitionRequest.Name)).Start();

                return dataContextWithTransaction.Save(newGameDefinition, currentUser);
            }

            ValidateNotADuplicateGameDefinition(existingGameDefinition);

            existingGameDefinition.Active = true;
            existingGameDefinition.BoardGameGeekGameDefinitionId = boardGameGeekGameDefinition?.Id;
            if (!string.IsNullOrWhiteSpace(createGameDefinitionRequest.Description))
            {
                existingGameDefinition.Description = createGameDefinitionRequest.Description;
            }
            return dataContextWithTransaction.Save(existingGameDefinition, currentUser);
        }

        private static void ValidateNotNull(CreateGameDefinitionRequest createGameDefinitionRequest)
        {
            if (createGameDefinitionRequest == null)
            {
                throw new ArgumentNullException(nameof(createGameDefinitionRequest));
            }
        }

        private static void ValidateGameDefinitionNameIsNotNullOrWhitespace(string gameDefinitionName)
        {
            if (string.IsNullOrWhiteSpace(gameDefinitionName))
            {
                throw new ArgumentException("createGameDefinitionRequest.Name cannot be null or whitespace.");
            }
        }

        private static void ValidateNotADuplicateGameDefinition(GameDefinition existingGameDefinition)
        {
            if (existingGameDefinition.Active)
            {
                string message = $"An active Game Definition with name '{existingGameDefinition.Name}' already exists in this Gaming Group.";
                throw new DuplicateKeyException(message);
            }
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
