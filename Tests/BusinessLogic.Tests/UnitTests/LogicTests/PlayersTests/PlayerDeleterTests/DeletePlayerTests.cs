using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerDeleterTests
{
    [TestFixture]
    public class DeletePlayerTests
    {
        protected RhinoAutoMocker<PlayerDeleter> AutoMocker;
        protected ApplicationUser CurrentUser;
        protected int PlayerId = 1;

        [SetUp]
        public virtual void SetUp()
        {
            AutoMocker = new RhinoAutoMocker<PlayerDeleter>();

            CurrentUser = new ApplicationUser();
        }
    }

    public class When_Player_Not_Exists : DeletePlayerTests
    {
        public override void SetUp()
        {
            base.SetUp();

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(new List<Player>().AsQueryable());
        }

        [Test]
        public void Then_Throw_Exception()
        {
            var expectedException = new ArgumentException("Player not exists","playerId");

            var exception = Assert.Throws<ArgumentException>(() => AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser));

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

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
        }

        [Test]
        public void Then_Throw_Exception()
        {
            var expectedException = new Exception("You can not delete players with any played game");

            var exception = Assert.Throws<Exception>(() => AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser));

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

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
        }

        [Test]
        public void Then_Deletes_The_Player()
        {
            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<PlayerAchievement>())
                .Return(new List<PlayerAchievement>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Champion>())
                .Return(new List<Champion>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(new List<Nemesis>().AsQueryable());

            AutoMocker.ClassUnderTest.DeletePlayer(1, CurrentUser);

            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<Player>(PlayerId, CurrentUser));
        }
    }

    //TODO BRIAN need tests for at least the following
    //* it deletes achievements
    //* it deletes champions
    //* it recalculates champions once per game definition
    //* it only recalculates champions once (not more!) for a given game definition
    //* it clears out the previous nemesis of any player who has this player as a previous nemesis
    //* it clears out the current nemesis of any player who currently has this player as a nemesis
    //* it recalculates the nemesis once (not more!) each player that had their current nemesis cleared
}
