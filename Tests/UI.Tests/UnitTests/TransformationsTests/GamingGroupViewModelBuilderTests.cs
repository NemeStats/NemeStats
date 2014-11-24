using System.Web.UI.WebControls;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GameDefinitionModels;
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
        private IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilderMock;
        private GamingGroupSummary gamingGroupSummary;
        private GamingGroupViewModel viewModel;
        private List<Player> players;
        private List<GameDefinitionSummary> gameDefinitionSummaries;
        private List<PlayedGame> playedGames;
        private List<GameDefinitionDetailsViewModel> gameDefinitionDetailsViewModels; 
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            invitationTransformerMock = MockRepository.GenerateMock<IGamingGroupInvitationViewModelBuilder>();
            playerWithNemesisViewModelBuilderMock = MockRepository.GenerateMock<IPlayerWithNemesisViewModelBuilder>();
            playedGameDetailsViewModelBuilderMock = MockRepository.GenerateMock<IPlayedGameDetailsViewModelBuilder>();
            gameDefinitionSummaryViewModelBuilderMock = MockRepository.GenerateMock<IGameDefinitionSummaryViewModelBuilder>();
            transformer = new GamingGroupViewModelBuilder(
                invitationTransformerMock,
                playedGameDetailsViewModelBuilderMock,
                playerWithNemesisViewModelBuilderMock,
                gameDefinitionSummaryViewModelBuilderMock);
            players = new List<Player>()
            {
                new Player(){ Id = 1 },
                new Player(){ Id = 2 }
            };
            gameDefinitionSummaries = new List<GameDefinitionSummary>
            {
                new GameDefinitionSummary{ Id = 1 },
                new GameDefinitionSummary{ Id = 2 }
            };

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

            currentUser = new ApplicationUser();

            foreach(Player player in players)
            {
                playerWithNemesisViewModelBuilderMock.Expect(mock => mock.Build(player, currentUser))
                    .Return(new PlayerWithNemesisViewModel() { PlayerId = player.Id });
            }

            foreach (GameDefinitionSummary summary in gameDefinitionSummaries)
            {
                gameDefinitionSummaryViewModelBuilderMock.Expect(mock => mock.Build(summary, currentUser))
                                                  .Return(new GameDefinitionSummaryViewModel { Id = summary.Id });
            }

            viewModel = transformer.Build(gamingGroupSummary, currentUser);
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
        public void ItBuildsTheGameDefinitionViewModels()
        {
            foreach (GameDefinitionSummary summary in gameDefinitionSummaries)
            {
                Assert.True((from GameDefinitionSummaryViewModel game in viewModel.GameDefinitionSummaries
                                 where game.Id == summary.Id
                                 select true).First());
            }
        }

        [Test]
        public void ItSetsTheRecentlyPlayedGames()
        {
            Assert.AreEqual(playedGames, viewModel.RecentGames);
        }
    }
}
