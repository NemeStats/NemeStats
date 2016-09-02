using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using RollbarSharp;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.JobsTests.BoardGameGeekBatchUpdateService.LinkOrphanGamesTests
{
    [TestFixture]
    public class LinkOrphanGamesJobTests
    {
        private RhinoAutoMocker<Jobs.BoardGameGeekBatchUpdateJobService.BoardGameGeekBatchUpdateJobService> autoMocker;
        private static string okGame = "ok";

        private List<GameDefinition> OrphanGames = new List<GameDefinition>()
        {
            new GameDefinition()
            {
                Name = okGame + " (2016)",
                Id = 1,
                GamingGroupId = 1
            },
            new GameDefinition()
            {
                Name = "error (2015)",
                Id = 2,
                GamingGroupId = 1
            }
        };

        private IRollbarClient rollbarclient;

        [SetUp]
        public virtual void SetUp()
        {
            rollbarclient = MockRepository.GenerateStub<IRollbarClient>();
            autoMocker = new RhinoAutoMocker<Jobs.BoardGameGeekBatchUpdateJobService.BoardGameGeekBatchUpdateJobService>();
            autoMocker.Inject(typeof(IRollbarClient), rollbarclient);

            autoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(OrphanGames.AsQueryable());
        }

        public class When_Game_Exists_On_BoardGameGeekGameDefinition : LinkOrphanGamesJobTests
        {
            public override void SetUp()
            {
                base.SetUp();

                var bggGameDefinition = new List<BoardGameGeekGameDefinition>()
                {
                    new BoardGameGeekGameDefinition()
                    {
                        Id = 100,
                        Name = okGame
                    }
                };

                autoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>())
                .Return(bggGameDefinition.AsQueryable());

                autoMocker.Get<IBoardGameGeekApiClient>()
                .Expect(mock => mock.SearchBoardGames(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                .Return(new List<SearchBoardGameResult>());
            }

            [Test]
            public void OkGame_Is_Linked()
            {
                var result = autoMocker.ClassUnderTest.LinkOrphanGames();

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.TimeEllapsed);
                Assert.AreEqual(2, result.OrphanGames);
                Assert.AreEqual(1, result.LinkedGames);
                Assert.AreEqual(1, result.StillOrphanGames.Count);
            }
        }

        public class When_Game_Exists_On_Search_And_Is_Not_Exists_On_Our_Database : LinkOrphanGamesJobTests
        {
            public override void SetUp()
            {
                base.SetUp();

                var bggId = 100;
                autoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<BoardGameGeekGameDefinition>())
                .Return(new List<BoardGameGeekGameDefinition>().AsQueryable());

                autoMocker.Get<IBoardGameGeekApiClient>()
                .Expect(mock => mock.SearchBoardGames(Arg<string>.Is.Equal(okGame), Arg<bool>.Is.Anything))
                .Return(new List<SearchBoardGameResult>() { new SearchBoardGameResult { BoardGameId = bggId } });
                autoMocker.Get<IBoardGameGeekApiClient>()
                .Expect(mock => mock.SearchBoardGames(Arg<string>.Is.NotEqual(okGame), Arg<bool>.Is.Anything))
                .Return(new List<SearchBoardGameResult>());

                autoMocker.Get<IBoardGameGeekApiClient>()
              .Expect(mock => mock.GetGameDetails(Arg<int>.Is.Equal(bggId)))
              .Return(new GameDetails { GameId = bggId, Thumbnail = "thumbnail"});
            }

            [Test]
            public void OkGame_Is_Linked()
            {
                var result = autoMocker.ClassUnderTest.LinkOrphanGames();

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.TimeEllapsed);
                Assert.AreEqual(2, result.OrphanGames);
                Assert.AreEqual(1, result.LinkedGames);
                Assert.AreEqual(1, result.StillOrphanGames.Count);
            }
        }


    }
}
