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
    public class IndexHttpGetTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsAnIndexView()
        {
            gameDefinitionRetrieverMock.Expect(repo => repo.GetAllGameDefinitions(Arg<int>.Is.Anything));

            ViewResult viewResult = gameDefinitionControllerPartialMock.Index(currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void TheIndexViewHasAllGameDefinitions()
        {
            List<GameDefinition> games = new List<GameDefinition>();
            gameDefinitionRetrieverMock.Expect(repo => repo.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value))
                .Repeat.Once()
                .Return(games);

            ViewResult viewResult = gameDefinitionControllerPartialMock.Index(currentUser) as ViewResult;
            List<GameDefinition> viewModel = (List<GameDefinition>)viewResult.ViewData.Model;

            Assert.AreSame(games, viewModel);
        }
    }
}
