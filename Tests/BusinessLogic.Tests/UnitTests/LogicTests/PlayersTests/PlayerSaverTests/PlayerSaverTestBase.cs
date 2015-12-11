using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerSaverTests
{
    public class PlayerSaverTestBase
    {
        protected RhinoAutoMocker<PlayerSaver> autoMocker;
        protected ApplicationUser currentUser;
        protected List<Player> players;
        protected Player playerThatAlreadyExists;
        protected int idOfPlayerThatAlreadyExists;

        [SetUp]
        public void SetUpBase()
        {
            autoMocker = new RhinoAutoMocker<PlayerSaver>();

            currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = 12
            };

            playerThatAlreadyExists = new Player
            {
                Name = "the new player name"
            };
            idOfPlayerThatAlreadyExists = 9;
            players = new List<Player>
            {
                new Player
                {
                    Id = idOfPlayerThatAlreadyExists,
                    Name = this.playerThatAlreadyExists.Name,
                    GamingGroupId = currentUser.CurrentGamingGroupId
                },
                new Player
                {
                    Id = 2
                }
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                .Repeat.Once()
                .Return(players.AsQueryable());
        }
    }
}
