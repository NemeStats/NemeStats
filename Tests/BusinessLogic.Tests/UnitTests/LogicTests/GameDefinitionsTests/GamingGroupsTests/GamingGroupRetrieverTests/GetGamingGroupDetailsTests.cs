using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    [TestFixture]
    public class GetGamingGroupDetailsTests
    {
        protected GamingGroupRetriever gamingGroupRetriever;
        protected IDataContext dataContextMock;
        protected IPlayerRetriever playerRetrieverMock;
        protected IGameDefinitionRetriever gameDefinitionRetrieverMock;
        protected IPlayedGameRetriever playedGameRetriever;
        protected ApplicationUser currentUser;
        protected GamingGroup expectedGamingGroup;
        protected GamingGroupInvitation expectedGamingGroupInvitation;

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
            expectedGamingGroup = new GamingGroup() { Id = gamingGroupId, OwningUserId = currentUser.Id };

            dataContextMock.Expect(mock => mock.FindById<GamingGroup>(gamingGroupId))
                .Return(expectedGamingGroup);

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
        public void ItReturnsTheGamingGroup()
        {
            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.AreSame(expectedGamingGroup, actualGamingGroup);
        }

        [Test]
        public void ItReturnsTheOwningUserOnTheGameDefinition()
        {
            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.NotNull(actualGamingGroup.OwningUser);
        }

        [Test]
        public void ItReturnsTheGamingGroupInvitationsOnTheGamingGroup()
        {
            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.AreSame(expectedGamingGroup.GamingGroupInvitations[0], actualGamingGroup.GamingGroupInvitations[0]);
        }
        
        [Test]
        public void ItReturnsTheRegisteredUserNameOfAnyUsersThatRegistered()
        {
            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

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

            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.AreSame(expectedPlayers, actualGamingGroup.Players);
        }

        [Test]
        public void ItReturnsAllGameDefinitionsForTheGamingGroup()
        {
            List<GameDefinition> expectedGameDefinitions = new List<GameDefinition>();
            gameDefinitionRetrieverMock.Expect(mock => mock.GetAllGameDefinitions(gamingGroupId))
                .Return(expectedGameDefinitions);

            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, 0);

            Assert.AreSame(expectedGameDefinitions, actualGamingGroup.GameDefinitions);
        }

        [Test]
        public void ItReturnsTheSpecifiedNumberOfPlayedGamesForTheGamingGroup()
        {
            int numberOfGames = 135;
            List<PlayedGame> playedGames = new List<PlayedGame>();
            playedGameRetriever.Expect(mock => mock.GetRecentGames(numberOfGames, gamingGroupId))
                .Return(playedGames);

            GamingGroup actualGamingGroup = gamingGroupRetriever.GetGamingGroupDetails(gamingGroupId, numberOfGames);

            Assert.AreSame(playedGames, actualGamingGroup.PlayedGames);
        }
    }
}
