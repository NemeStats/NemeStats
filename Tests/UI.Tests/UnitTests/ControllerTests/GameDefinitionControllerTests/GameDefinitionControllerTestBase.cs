using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class GameDefinitionControllerTestBase
    {
        protected GameDefinitionController gameDefinitionControllerPartialMock;
        protected GameDefinitionRepository gameDefinitionRepository;
        protected GameDefinitionRetriever gameDefinitionRetrieverMock;
        protected GameDefinitionToGameDefinitionViewModelTransformation gameDefinitionTransformation;
        protected ShowingXResultsMessageBuilder showingXResultsMessageBuilder;
        protected NemeStatsDataContext dataContext;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            gameDefinitionRepository = MockRepository.GenerateMock<GameDefinitionRepository>();
            dataContext = MockRepository.GenerateMock<NemeStatsDataContext>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<GameDefinitionRetriever>();
            gameDefinitionTransformation = MockRepository.GenerateMock<GameDefinitionToGameDefinitionViewModelTransformation>();
            showingXResultsMessageBuilder = MockRepository.GenerateMock<ShowingXResultsMessageBuilder>();
            gameDefinitionControllerPartialMock = MockRepository.GeneratePartialMock<GameDefinitionController>(
                dataContext, 
                gameDefinitionRepository,
                gameDefinitionRetrieverMock,
                gameDefinitionTransformation,
                showingXResultsMessageBuilder);
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 15151
            };
        }
    }
}
