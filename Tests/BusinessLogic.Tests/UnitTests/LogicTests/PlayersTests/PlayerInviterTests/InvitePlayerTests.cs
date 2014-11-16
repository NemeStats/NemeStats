using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerInviterTests
{
    [TestFixture]
    public class InvitePlayerTests
    {
        private PlayerInviter playerInviter;
        private IDataContext dataContextMock;
        private IIdentityMessageService emailServiceMock;
        private PlayerInvitation playerInvitation;
        private ApplicationUser currentUser;
        private Player player;
        private GamingGroup gamingGroup;
        private GamingGroupInvitation gamingGroupInvitation;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            emailServiceMock = MockRepository.GenerateMock<IIdentityMessageService>();
            playerInvitation = new PlayerInvitation
            {
                CustomEmailMessage = "custom message",
                EmailSubject = "email subject",
                InvitedPlayerEmail = "player email",
                InvitedPlayerId = 1
            };
            currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = 15,
                UserName = "Fergie Ferg"
            };
            player = new Player
            {
                Id = playerInvitation.InvitedPlayerId,
                GamingGroupId = 135
            };
            gamingGroup = new GamingGroup
            {
                Id = currentUser.CurrentGamingGroupId.Value,
                Name = "jake's Gaming Group"
            };
            gamingGroupInvitation = new GamingGroupInvitation
            {
                Id = Guid.NewGuid()
            };

            dataContextMock.Expect(mock => mock.FindById<GamingGroup>(currentUser.CurrentGamingGroupId.Value))
                           .Return(gamingGroup);

            dataContextMock.Expect(mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                           .Return(gamingGroupInvitation);

            emailServiceMock.Expect(mock => mock.SendAsync(Arg<IdentityMessage>.Is.Anything))
                            .Return(Task.FromResult<object>(null));

            playerInviter = new PlayerInviter(dataContextMock, emailServiceMock);
        }

        [Test]
        public void ItSavesAGamingGroupInvitation()
        {
            playerInviter.InvitePlayer(playerInvitation, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<GamingGroupInvitation>(Arg<GamingGroupInvitation>.Matches(
                invite => invite.PlayerId == playerInvitation.InvitedPlayerId
                && invite.DateSent.Date == DateTime.UtcNow.Date
                && invite.GamingGroupId == currentUser.CurrentGamingGroupId.Value
                && invite.InviteeEmail == playerInvitation.InvitedPlayerEmail
                && invite.InvitingUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItEmailsTheUser()
        {
            string expectedBody = string.Format(PlayerInviter.EMAIL_MESSAGE_INVITE_PLAYER,
                                                currentUser.UserName,
                                                gamingGroup.Name,
                                                "http://nerdscorekeeper.azurewebsites.net",
                                                currentUser.CurrentGamingGroupId.Value,
                                                playerInvitation.CustomEmailMessage,
                                                gamingGroupInvitation.Id);

            playerInviter.InvitePlayer(playerInvitation, currentUser);

            emailServiceMock.AssertWasCalled(mock => mock.SendAsync(Arg<IdentityMessage>.Matches(
                message => message.Subject == playerInvitation.EmailSubject
                && message.Body.StartsWith(expectedBody))));
        }
    }
}
