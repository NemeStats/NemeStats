using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerDeleterTests
{
    [TestFixture]
    public class DeletePlayerTests
    {
        protected IDataContext DataContextMock;
        protected PlayerDeleter PlayerDeleter;
        protected ApplicationUser CurrentUser;
        protected int PlayerId = 1;

        [SetUp]
        public virtual void SetUp()
        {
            DataContextMock = MockRepository.GenerateMock<IDataContext>();

            PlayerDeleter = new PlayerDeleter(DataContextMock);

            CurrentUser = new ApplicationUser();

        }

    }

    public class When_Player_Not_Exists : DeletePlayerTests
    {
        public override void SetUp()
        {
            base.SetUp();

            DataContextMock.Expect(m => m.GetQueryable<Player>()).Return(new List<Player>().AsQueryable());
        }

        [Test]
        public void Then_Throw_Exception()
        {
            var expectedException = new ArgumentException("Player not exists","playerId");

            var exception = Assert.Throws<ArgumentException>(() => PlayerDeleter.DeletePlayer(PlayerId, CurrentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }
    }

    public class When_Player_Has_PlayedGames : DeletePlayerTests
    {
        public override void SetUp()
        {
            base.SetUp();

            var players = new List<Player>
            {
                new Player {Id = PlayerId, PlayerGameResults = new List<PlayerGameResult> {new PlayerGameResult()}}
            };

            DataContextMock.Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
        }

        [Test]
        public void Then_Throw_Exception()
        {
            var expectedException = new Exception("You can not delete players with any played game");

            var exception = Assert.Throws<Exception>(() => PlayerDeleter.DeletePlayer(PlayerId, CurrentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }
    }

    public class When_Player_Has_No_PlayedGames : DeletePlayerTests
    {
        public override void SetUp()
        {
            base.SetUp();

            var players = new List<Player>
            {
                new Player {Id = PlayerId, PlayerGameResults = new List<PlayerGameResult> ()}
            };

            DataContextMock.Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
        }

        [Test]
        public void Then_Deletes_The_Player()
        {
            PlayerDeleter.DeletePlayer(1, CurrentUser);

            DataContextMock.AssertWasCalled(mock => mock.DeleteById<Player>(PlayerId, CurrentUser));
        }
    }
}
