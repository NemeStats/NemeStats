using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class EditHttpPostTests : PlayerControllerTestBase
    {
        private readonly PlayerEditViewModel expectedViewModel = new PlayerEditViewModel();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            playerEditViewModelBuilderMock.Expect(mock => mock.Build(Arg<Player>.Is.Anything))
                                          .Return(expectedViewModel);
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndPlayersSectionAfterSaving()
        {
            string baseUrl = "base url";
            string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS;
            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);
            Player player = new Player()
            {
                Name = "player name"
            };

            RedirectResult redirectResult = playerController.Edit(player, currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }

        [Test]
        public void ItRemainsOnTheEditViewIfValidationFails()
        {
            playerController.ModelState.AddModelError("key", "message");

            ViewResult result = playerController.Edit(new Player(), currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerOnTheViewIfValidationFails()
        {
            Player player = new Player();
            playerController.ModelState.AddModelError("key", "message");

            ViewResult result = playerController.Edit(player, currentUser) as ViewResult;

            Assert.AreEqual(expectedViewModel, result.Model);
        }

        [Test]
        public void ItSavesThePlayer()
        {
            Player player = new Player()
            {
                Name = "player name"
            };

            playerController.Edit(player, currentUser);

            playerSaverMock.AssertWasCalled(mock => mock.Save(player, currentUser));
        }
    }
}
