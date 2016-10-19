using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
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

    public class When_Player_Has_Achievements : DeletePlayerTests
    {
        public override void SetUp()
        {
            base.SetUp();

            var players = new List<Player>
            {
                new Player {Id = PlayerId, PlayerGameResults = new List<PlayerGameResult> ()}
            };

            var achievements = new List<PlayerAchievement>
            {
                new PlayerAchievement { PlayerId = PlayerId, AchievementId = AchievementId.Champion, AchievementLevel = AchievementLevel.Bronze }
            };

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<PlayerAchievement>()).Return(achievements.AsQueryable());
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

            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<PlayerAchievement>(0, CurrentUser));
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<Player>(PlayerId, CurrentUser));
        }
    }

    public class When_Player_Is_Champion : DeletePlayerTests
    {
        public override void SetUp()
        {
            base.SetUp();
            var championGames = new List<GameDefinition>
            {
                new GameDefinition { Id = 1, ChampionId = PlayerId }
            };

            var players = new List<Player>
            {
                new Player {Id = PlayerId, PlayerGameResults = new List<PlayerGameResult> () }
            };

            var champions = new List<Champion>
            {
                new Champion { Id = 1, GameDefinitionId = 1, PlayerId = PlayerId }
            };

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<GameDefinition>()).Return(championGames.AsQueryable());
            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Champion>()).Return(champions.AsQueryable());
        }

        [Test]
        public void Then_Deletes_The_Champion()
        {
            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<PlayerAchievement>())
                .Return(new List<PlayerAchievement>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(new List<GameDefinition>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Champion>())
                .Return(new List<Champion>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(new List<Nemesis>().AsQueryable());

            AutoMocker.ClassUnderTest.DeletePlayer(1, CurrentUser);

            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<Champion>(1, CurrentUser));
        }
    }

    public class When_Deleted_Player_Is_Champion : DeletePlayerTests
    {
        public override void SetUp()
        {
            base.SetUp();
            var championGames = new List<GameDefinition>
            {
                new GameDefinition { Id = 1, ChampionId = 1 }
            };

            var playedGames = new List<PlayerGameResult>
            {
                new PlayerGameResult { Id = 1, PlayerId = 2 }
            };

            var players = new List<Player>
            {
                new Player {Id = 1, PlayerGameResults = new List<PlayerGameResult> () },
                new Player {Id = 2, PlayerGameResults = new List<PlayerGameResult> () },
            };

            var champions = new List<Champion>
            {
                new Champion { Id = 1, GameDefinitionId = 1, PlayerId = 1 }
            };

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<GameDefinition>()).Return(championGames.AsQueryable());
            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Champion>()).Return(champions.AsQueryable());
        }

        [Test]
        public void Then_Recalculates_New_Champion()
        {
            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<PlayerAchievement>())
                .Return(new List<PlayerAchievement>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(new List<GameDefinition>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Champion>())
                .Return(new List<Champion>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(new List<Nemesis>().AsQueryable());

            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<Champion>(1, CurrentUser));

            var champion = AutoMocker.Get<IDataContext>().GetQueryable<Champion>().ToList();
        }
    }

    public class When_Deleted_Player_Is_Nemesis : DeletePlayerTests
    {
        public override void SetUp()
        {
            base.SetUp();

            var playedGames = new List<PlayerGameResult>
            {
                new PlayerGameResult { Id = 1, PlayerId = 2 }
            };

            var players = new List<Player>
            {
                new Player {Id = 1, PlayerGameResults = new List<PlayerGameResult> (), NemesisId = 2 },
                new Player {Id = 2, PlayerGameResults = new List<PlayerGameResult> () }
            };

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
        }

        [Test]
        public void Then_Recalculates_New_Nemesis()
        {
            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<PlayerAchievement>())
                .Return(new List<PlayerAchievement>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(new List<GameDefinition>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Champion>())
                .Return(new List<Champion>().AsQueryable());

            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(new List<Nemesis>().AsQueryable());

            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            //TODO: AutoMocker.ClassUnderTest.AssertWasCalled(mock => mock.DeletePlayerNemesesRecords(1, CurrentUser));
        }
    }

    //TODO BRIAN need tests for at least the following
    //* it deletes achievements DONE
    //* it deletes champions DONE
    //* it recalculates champions once per game definition - MOVED TO PlayedGameDeleter
    //* it only recalculates champions once (not more!) for a given game definition - MOVED TO PlayedGameDeleter
    //* it clears out the previous nemesis of any player who has this player as a previous nemesis
    //* it clears out the current nemesis of any player who currently has this player as a nemesis
    //* it recalculates the nemesis once (not more!) each player that had their current nemesis cleared
}
