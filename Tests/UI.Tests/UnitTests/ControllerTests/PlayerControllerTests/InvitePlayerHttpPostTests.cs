using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class InvitePlayerHttpPostTests : PlayerControllerTestBase
    {
        [Test]
        public void ItSendsThePlayerInvitation()
        {
            PlayerInvitationViewModel playerInvitationViewModel = new PlayerInvitationViewModel
            {
                EmailAddress = "email address",
                EmailBody = "email body",
                EmailSubject = "email subject",
                PlayerId = 1
            };
            ApplicationUser applicationUser = new ApplicationUser();

            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                .Return("some url");

            playerController.InvitePlayer(playerInvitationViewModel, applicationUser);

            playerInviterMock.AssertWasCalled(mock => mock.InvitePlayer(Arg<PlayerInvitation>.Matches(
                invite => invite.CustomEmailMessage == playerInvitationViewModel.EmailBody
                && invite.EmailSubject == playerInvitationViewModel.EmailSubject
                && invite.InvitedPlayerEmail == playerInvitationViewModel.EmailAddress
                && invite.InvitedPlayerId == playerInvitationViewModel.PlayerId),
                Arg<ApplicationUser>.Is.Same(applicationUser)));
        }
    }
}
