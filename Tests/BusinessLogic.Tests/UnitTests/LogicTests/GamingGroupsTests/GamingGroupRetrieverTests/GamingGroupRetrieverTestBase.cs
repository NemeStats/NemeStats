using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupRetrieverTests
{
    public class GamingGroupRetrieverTestBase
    {
        protected GamingGroupRetriever gamingGroupRetriever;
        protected IDataContext dataContextMock;
        protected IPlayerRetriever playerRetrieverMock;
        protected IGameDefinitionRetriever gameDefinitionRetrieverMock;
        protected IPlayedGameRetriever playedGameRetriever;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
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
        }
    }
}
