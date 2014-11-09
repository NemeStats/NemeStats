using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GamingGroupViewModelBuilderTests
    {
        private GamingGroupViewModelBuilder transformer;
        private IGamingGroupInvitationViewModelBuilder invitationTransformerMock;
        private IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilderMock;
        private IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilderMock;
        private GamingGroupSummary gamingGroupSummary;
        private GamingGroupViewModel viewModel;
        private List<Player> players;
        private List<GameDefinitionSummary> gameDefinitionSummaries;
        private List<PlayedGame> playedGames;

        [SetUp]
        public void SetUp()
        {
            invitationTransformerMock = MockRepository.GenerateMock<IGamingGroupInvitationViewModelBuilder>();
            playerWithNemesisViewModelBuilderMock = MockRepository.GenerateMock<IPlayerWithNemesisViewModelBuilder>();
            playedGameDetailsViewModelBuilderMock = MockRepository.GenerateMock<IPlayedGameDetailsViewModelBuilder>();
            transformer = new GamingGroupViewModelBuilder(
                invitationTransformerMock,
                playedGameDetailsViewModelBuilderMock,
                playerWithNemesisViewModelBuilderMock);
            players = new List<Player>()
            {
                new Player(){ Id = 1 },
                new Player(){ Id = 2 }
            };
            gameDefinitionSummaries = new List<GameDefinitionSummary>();
            playedGames = new List<PlayedGame>();
            ApplicationUser owningUser = new ApplicationUser()
            {
                Id = "owning user user Id",
                Email = "owninguser@email.com",
                UserName = "username"
            };
            ApplicationUser registeredUser = new ApplicationUser()
            {
                Email = "registereduser@email.com",
                Id = "registered user id",
                UserName = "registered user name"
            };
            GamingGroupInvitation invitation = new GamingGroupInvitation()
            {
                DateRegistered = DateTime.UtcNow,
                RegisteredUserId = "registered user id",
                RegisteredUser = registeredUser
            };
            gamingGroupSummary = new GamingGroupSummary()
            {
                Id = 1,
                Name = "gaming group",
                OwningUserId = owningUser.Id,
                OwningUser = owningUser,
                GamingGroupInvitations = new List<GamingGroupInvitation>() { invitation },
                Players = players,
                GameDefinitionSummaries = gameDefinitionSummaries,
                PlayedGames = playedGames
            };

            playedGameDetailsViewModelBuilderMock.Expect(mock => mock.Build(
                Arg<PlayedGame>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything))
                .Return(new PlayedGameDetailsViewModel());

            foreach(Player player in players)
            {
                playerWithNemesisViewModelBuilderMock.Expect(mock => mock.Build(player))
                    .Return(new PlayerWithNemesisViewModel() { PlayerId = player.Id });
            }

            viewModel = transformer.Build(gamingGroupSummary, null);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(gamingGroupSummary.Id, viewModel.Id);
        }

        [Test]
        public void ItCopiesTheOwningUserId()
        {
            Assert.AreEqual(gamingGroupSummary.OwningUserId, viewModel.OwningUserId);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(gamingGroupSummary.Name, viewModel.Name);
        }
        
        [Test]
        public void ItCopiesTheOwningUserName()
        {
            Assert.AreEqual(gamingGroupSummary.OwningUser.UserName, viewModel.OwningUserName);
        }

        [Test]
        public void ItTransformsGamingGroupInvitationsToInvitationViewModels()
        {
            List<InvitationViewModel> invitations = new List<InvitationViewModel>();
            foreach(GamingGroupInvitation invitation in gamingGroupSummary.GamingGroupInvitations)
            {
                InvitationViewModel invitationViewModel = new InvitationViewModel();
                invitations.Add(invitationViewModel);

                invitationTransformerMock.Expect(mock => mock.Build(invitation))
                    .Repeat.Once()
                    .Return(invitationViewModel);
            }

            Assert.AreEqual(invitations.Count(), viewModel.Invitations.Count());
        }

        [Test]
        public void ItSetsThePlayers()
        {
            foreach(Player player in players)
            {
                Assert.True((from PlayerWithNemesisViewModel playerWithNemesis in viewModel.Players
                     where playerWithNemesis.PlayerId == player.Id
                     select true).First());
            }
        }

        [Test]
        public void ItSetsTheGameDefinitionSummaries()
        {
            Assert.AreSame(gameDefinitionSummaries, viewModel.GameDefinitionSummaries);
        }

        [Test]
        public void ItSetsTheRecentlyPlayedGames()
        {
            Assert.AreEqual(playedGames, viewModel.RecentGames);
        }
    }
}
