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
    public class ConsumeInvitationTests : AccountControllerTestBase
    {
        private string gamingGroupInvitationId = "invitation id";

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        } 

        [Test]
        public void ItConsumesTheInvitation()
        {
            accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser);

            gamingGroupInviteConsumerMock.AssertWasCalled(mock => mock.ConsumeInvitation(gamingGroupInvitationId));
        }

        [Test]
        public void ItRedirectsToTheGamingGroupPageIfThePlayerWasAddedDirectlyToAGamingGroupWithoutHavingToEnterInformation()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeInvitation(Arg<string>.Is.Anything))
                                         .Return(true);

            RedirectToRouteResult redirectResult = accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser) as RedirectToRouteResult;

            Assert.That(redirectResult.RouteValues["action"].Equals(MVC.GamingGroup.ActionNames.Index));
        }

        [Test]
        public void ItRedirectsToTheRegisterAgainstExistingGamingGroupActionIfTheUserDoesntAlreadyExist()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeInvitation(Arg<string>.Is.Anything))
                                         .Return(false);

            RedirectToRouteResult redirectResult = accountControllerPartialMock.ConsumeInvitation(gamingGroupInvitationId, currentUser) as RedirectToRouteResult;

            Assert.That(redirectResult.RouteValues["action"].Equals(MVC.Account.ActionNames.RegisterAgainstExistingGamingGroup));
        }
    }
}
