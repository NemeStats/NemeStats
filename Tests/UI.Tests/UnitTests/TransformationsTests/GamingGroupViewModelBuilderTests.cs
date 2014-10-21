using BusinessLogic.Models;
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
        private GamingGroup gamingGroup;
        private GamingGroupViewModel viewModel;
        private List<Player> players;
        private List<GameDefinition> gameDefinitions;
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
            gameDefinitions = new List<GameDefinition>();
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
            gamingGroup = new GamingGroup()
            {
                Id = 1,
                Name = "gaming group",
                OwningUserId = owningUser.Id,
                OwningUser = owningUser,
                GamingGroupInvitations = new List<GamingGroupInvitation>() { invitation },
                Players = players,
                GameDefinitions = gameDefinitions,
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

            viewModel = transformer.Build(gamingGroup, null);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(gamingGroup.Id, viewModel.Id);
        }

        [Test]
        public void ItCopiesTheOwningUserId()
        {
            Assert.AreEqual(gamingGroup.OwningUserId, viewModel.OwningUserId);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(gamingGroup.Name, viewModel.Name);
        }
        
        [Test]
        public void ItCopiesTheOwningUserName()
        {
            Assert.AreEqual(gamingGroup.OwningUser.UserName, viewModel.OwningUserName);
        }

        [Test]
        public void ItTransformsGamingGroupInvitationsToInvitationViewModels()
        {
            List<InvitationViewModel> invitations = new List<InvitationViewModel>();
            foreach(GamingGroupInvitation invitation in gamingGroup.GamingGroupInvitations)
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
        public void ItSetsTheGameDefinitions()
        {
            Assert.AreSame(gameDefinitions, viewModel.GameDefinitions);
        }

        [Test]
        public void ItSetsTheRecentlyPlayedGames()
        {
            Assert.AreEqual(playedGames, viewModel.RecentGames);
        }
    }
}
