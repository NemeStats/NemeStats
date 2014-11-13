using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetGamingGroupDetailsTests
    {
        private GamingGroupRetriever gamingGroupRetriever;
        private IDataContext dataContextMock;
        private IPlayerRetriever playerRetrieverMock;
        private IGameDefinitionRetriever gameDefinitionRetrieverMock;
        private IPlayedGameRetriever playedGameRetriever;
        private ApplicationUser currentUser;
        private GamingGroup expectedGamingGroup;
        private GamingGroupInvitation expectedGamingGroupInvitation;
        private List<GameDefinitionSummary> gameDefinitionSummaries;  

        protected int gamingGroupId = 13511;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            playerRetrieverMock = MockRepository.GenerateMock<IPlayerRetriever>();
            gameDefinitionRetrieverMock = MockRepository.GenerateMock<IGameDefinitionRetriever>();
            playedGameRetriever = MockRepository.GenerateMock<IPlayedGameRetriever>();
            gamingGroupRetriever = new GamingGroupRetriever(
                dataContextMock, 
                playerRetrieverMock, 
                gameDefinitionRetrieverMock,
                playedGameRetriever);

            currentUser = new ApplicationUser() 
            { 
                Id = "application user", 
                UserName = "user name", 
                CurrentGamingGroupId = 1 
            };
            expectedGamingGroup = new GamingGroup
            {
                Id = gamingGroupId, 
                OwningUserId = currentUser.Id
            };

            dataContextMock.Expect(mock => mock.FindById<GamingGroup>(gamingGroupId))
                .Return(expectedGamingGroup);

            gameDefinitionSummaries = new List<GameDefinitionSummary>
            {
                new GameDefinitionSummary()
            };
            gameDefinitionRetrieverMock.Expect(mock => mock.GetAllGameDefinitions(gamingGroupId))
                                       .Return(gameDefinitionSummaries);

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>();
            applicationUsers.Add(currentUser);

            dataContextMock.Expect(mock => mock.GetQueryable<ApplicationUser>())
                .Return(applicationUsers.AsQueryable());

            List<GamingGroupInvitation> gamingGroupInvitations = new List<GamingGroupInvitation>();
            expectedGamingGroupInvitation = new GamingGroupInvitation() 
            { 
                GamingGroupId = expectedGamingGroup.Id, 
                RegisteredUserId = currentUser.Id 
            };
            
            gamingGroupInvitations.Add(expectedGamingGroupInvitation);
            dataContextMock.Expect(mock => mock.GetQueryable<GamingGroupInvitation>())
                .Return(gamingGroupInvitations.AsQueryable());

            dataContextMock.Expect(mock => mock.GetQueryable<ApplicationUser>())
                .Return(applicationUsers.AsQueryable());
        }

        [Test]
        public void ItReturnsTheGamingGroupSummary()
        {
            GamingGroupSummary actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.AreEqual(expectedGamingGroup.Id, actualGamingGroup.Id);
            Assert.AreEqual(expectedGamingGroup.Name, actualGamingGroup.Name);
            Assert.AreEqual(expectedGamingGroup.OwningUserId, actualGamingGroup.OwningUserId);
            Assert.AreEqual(expectedGamingGroup.DateCreated, actualGamingGroup.DateCreated);
        }

        [Test]
        public void ItReturnsTheOwningUserOnTheGameDefinition()
        {
            GamingGroupSummary actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.NotNull(actualGamingGroup.OwningUser);
        }

        [Test]
        public void ItReturnsTheGamingGroupInvitationsOnTheGamingGroup()
        {
            GamingGroupSummary actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.AreSame(expectedGamingGroupInvitation, actualGamingGroup.GamingGroupInvitations[0]);
        }
        
        [Test]
        public void ItReturnsTheRegisteredUserNameOfAnyUsersThatRegistered()
        {
            GamingGroupSummary actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            foreach(GamingGroupInvitation invitation in actualGamingGroup.GamingGroupInvitations)
            {
                if(!string.IsNullOrEmpty(invitation.RegisteredUserId))
                {
                    Assert.NotNull(invitation.RegisteredUser);
                }
            }
        }

        [Test]
        public void ItReturnsAllActivePlayersInTheGamingGroup()
        {
            List<Player> expectedPlayers = new List<Player>();
            playerRetrieverMock.Expect(mock => mock.GetAllPlayersWithNemesisInfo(gamingGroupId))
                .Return(expectedPlayers);

            GamingGroupSummary actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.AreSame(expectedPlayers, actualGamingGroup.Players);
        }

        [Test]
        public void ItReturnsAllGameDefinitionsForTheGamingGroup()
        {
            GamingGroupSummary actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.AreSame(gameDefinitionSummaries, actualGamingGroup.GameDefinitionSummaries);
        }

        [Test]
        public void ItReturnsTheSpecifiedNumberOfPlayedGamesForTheGamingGroup()
        {
            int numberOfGames = 135;
            List<PlayedGame> playedGames = new List<PlayedGame>();
            playedGameRetriever.Expect(mock => mock.GetRecentGames(numberOfGames, gamingGroupId))
                .Return(playedGames);

            GamingGroupSummary actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, numberOfGames);

            Assert.AreSame(playedGames, actualGamingGroup.PlayedGames);
        }
    }
}
