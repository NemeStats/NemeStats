using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class IndexHttpGetTests
    {
        private GameDefinitionController gameDefinitionController;
        private GameDefinitionRepository gameDefinitionRepository;
        private NemeStatsDbContext dbContext;
        private UserContext userContext;

        [SetUp]
        public void SetUp()
        {
            gameDefinitionRepository = MockRepository.GenerateMock<GameDefinitionRepository>();
            dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            gameDefinitionController = new GameDefinitionController(dbContext, gameDefinitionRepository);
            userContext = new UserContext()
            {
                ApplicationUserId = "user id",
                GamingGroupId = 15151
            };
        }

        [Test]
        public void ItReturnsAnIndexView()
        {
            gameDefinitionRepository.Expect(repo => repo.GetAllGameDefinitions(Arg<NemeStatsDbContext>.Is.Anything, Arg<UserContext>.Is.Anything));

            ViewResult viewResult = gameDefinitionController.Index(userContext) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void TheIndexViewHasAllGameDefinitions()
        {
            List<GameDefinition> games = new List<GameDefinition>();
            gameDefinitionRepository.Expect(repo => repo.GetAllGameDefinitions(dbContext, userContext))
                .Repeat.Once()
                .Return(games);

            ViewResult viewResult = gameDefinitionController.Index(userContext) as ViewResult;
            List<GameDefinition> viewModel = (List<GameDefinition>)viewResult.ViewData.Model;

            Assert.AreSame(games, viewModel);
        }
    }
}
