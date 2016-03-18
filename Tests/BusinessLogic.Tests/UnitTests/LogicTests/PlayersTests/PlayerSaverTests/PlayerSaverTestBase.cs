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
        protected RhinoAutoMocker<PlayerSaver> _autoMocker;
        protected ApplicationUser _currentUser;
        protected List<Player> _players;
        protected Player _playerThatAlreadyExists;
        protected int _idOfPlayerThatAlreadyExists;

        [SetUp]
        public void SetUpBase()
        {
            _autoMocker = new RhinoAutoMocker<PlayerSaver>();

            _currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = 12
            };

            _playerThatAlreadyExists = new Player
            {
                Name = "the new player name"
            };
            _idOfPlayerThatAlreadyExists = 9;
            _players = new List<Player>
            {
                new Player
                {
                    Id = _idOfPlayerThatAlreadyExists,
                    Name = this._playerThatAlreadyExists.Name,
                    GamingGroupId = _currentUser.CurrentGamingGroupId
                },
                new Player
                {
                    Id = 2
                }
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                .Repeat.Once()
                .Return(_players.AsQueryable());
        }
    }
}
