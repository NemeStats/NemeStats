using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class AddExistingUserToGamingGroup : AccountControllerTestBase
    {
        private string gamingGroupInvitationId = "invitation id";
        private string email;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        } 

        [Test]
        public void ItConsumesTheInvitation()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.AddExistingUserToGamingGroup(Arg<string>.Is.Anything))
                              .Return(new AddUserToGamingGroupResult());

            accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser);

            gamingGroupInviteConsumerMock.AssertWasCalled(mock => mock.AddExistingUserToGamingGroup(this.gamingGroupInvitationId));
        }

        [Test]
        public void ItRedirectsToTheGamingGroupPageIfThePlayerWasAddedDirectlyToAGamingGroupWithoutHavingToEnterInformation()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.AddExistingUserToGamingGroup(Arg<string>.Is.Anything))
                                         .Return(new AddUserToGamingGroupResult{ UserAddedToExistingGamingGroup = true });

            RedirectToRouteResult redirectResult = accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser) as RedirectToRouteResult;

            Assert.That(redirectResult.RouteValues["action"], Is.EqualTo(MVC.GamingGroup.ActionNames.Index));
        }

        [Test]
        public void ItShowsTheRegisterAgainstExistingGamingGroupViewIfTheUserDoesntAlreadyExist()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.AddExistingUserToGamingGroup(Arg<string>.Is.Anything))
                                         .Return(new AddUserToGamingGroupResult { UserAddedToExistingGamingGroup = false });

            ViewResult viewResult = accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser) as ViewResult;

            Assert.That(MVC.Account.Views.RegisterAgainstExistingGamingGroup, Is.EqualTo(viewResult.ViewName));
        }
    }
}
