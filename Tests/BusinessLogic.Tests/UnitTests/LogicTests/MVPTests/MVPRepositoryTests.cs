using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.MVPModels;
using NSubstitute;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.MVPTests
{
    [TestFixture]
    public class MVPRepositoryTests
    {
        protected MVPRepository MVPRepository;
        protected IDataContext DataContextMock = MockRepository.GenerateMock<IDataContext>();
        protected GameDefinition GameDefinition = new GameDefinition();
        protected List<PlayerGameResult> PlayerGameResults = new List<PlayerGameResult>();

        protected int GameDefinitionId = 1;
        protected PlayedGame PlayedGame = new PlayedGame() { GameDefinitionId = 1 };
        protected int MVPPlayerId = 100;
        protected int MVPPlayerGameResultId = 1000;

        protected MVPData Result;

        [SetUp]
        public virtual void SetUp()
        {
            DataContextMock.Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(PlayerGameResults.AsQueryable());
            MVPRepository = MockRepository.GeneratePartialMock<MVPRepository>(DataContextMock);

            Result = MVPRepository.GetMVPData(GameDefinitionId);
        }



        public class When_GameDefinition_Not_Exists : MVPRepositoryTests
        {
            [Test]
            public void Then_Return_Null()
            {
                Result.ShouldBeNull();
            }
        }

        public class When_GameDefinition_Exists : MVPRepositoryTests
        {
            [SetUp]
            public override void SetUp()
            {
                DataContextMock.Expect(mock => mock.FindById<GameDefinition>(Arg<int>.Is.Anything)).Return(GameDefinition);
                base.SetUp();
            }

            public class When_No_Player_Has_Scored : When_GameDefinition_Exists
            {
                [Test]
                public void Then_Return_Null()
                {
                    Result.ShouldBeNull();
                }

            }

            public class When_Only_One_Player_Has_Scored : When_GameDefinition_Exists
            {
                [SetUp]
                public override void SetUp()
                {
                    PlayerGameResults.Add(new PlayerGameResult() { Id = MVPPlayerGameResultId, PlayerId = MVPPlayerId, PointsScored = 1, PlayedGame = PlayedGame });
                    PlayerGameResults.Add(new PlayerGameResult() { PlayedGame = PlayedGame });

                    base.SetUp();
                }

                [Test]
                public void Then_Return_MVP()
                {
                    Result.PlayedGameResultId.ShouldBe(MVPPlayerGameResultId);
                    Result.PlayerId.ShouldBe(MVPPlayerId);
                    Result.PointsScored.ShouldBe(1);
                }

            }

            public class When_One_Player_Has_More_Score_Than_Others : When_GameDefinition_Exists
            {
                [SetUp]
                public override void SetUp()
                {
                    PlayerGameResults.Add(new PlayerGameResult() { Id = MVPPlayerGameResultId, PlayerId = MVPPlayerId, PointsScored = 100, PlayedGame = PlayedGame });
                    PlayerGameResults.Add(new PlayerGameResult() { PlayerId = 2, PointsScored = 99, PlayedGame = PlayedGame });
                    PlayerGameResults.Add(new PlayerGameResult() { PlayerId = 3, PointsScored = 80, PlayedGame = PlayedGame });

                    base.SetUp();
                }

                [Test]
                public void Then_Return_MVP()
                {
                    Result.PlayedGameResultId.ShouldBe(MVPPlayerGameResultId);
                    Result.PlayerId.ShouldBe(MVPPlayerId);
                    Result.PointsScored.ShouldBe(100);
                }

            }

            public class When_There_Is_A_Tie : When_GameDefinition_Exists
            {
                protected PlayerGameResult MVPPlayerGameResult;
                protected PlayerGameResult TiedPlayerGameResult;
                [SetUp]
                public override void SetUp()
                {
                    PlayedGame.DatePlayed = DateTime.Now;
                    
                    MVPPlayerGameResult = new PlayerGameResult()
                    {
                        Id = MVPPlayerGameResultId,
                        PlayerId = MVPPlayerId,
                        PointsScored = 100,
                        PlayedGame = PlayedGame
                    };

                    TiedPlayerGameResult = new PlayerGameResult()
                    {
                        Id = MVPPlayerGameResultId,
                        PlayerId = MVPPlayerId+1,
                        PointsScored = 100,
                        PlayedGame = new PlayedGame()
                        {
                            DatePlayed =  PlayedGame.DatePlayed.AddDays(1)
                        }
                    };

                    PlayerGameResults.Add(MVPPlayerGameResult);
                    PlayerGameResults.Add(TiedPlayerGameResult);
                    

                    base.SetUp();
                }

                public class When_One_Tied_Players_Is_The_Current_MPV : When_There_Is_A_Tie
                {
                    [SetUp]
                    public override void SetUp()
                    {
                        GameDefinition.MVP = new MVP {PlayerId = MVPPlayerId};

                        base.SetUp();
                    }

                    [Test]
                    public void Then_Return_Current_MVP()
                    {
                        Result.PlayerId.ShouldBe(MVPPlayerId);
                    }
                }

                public class When_Tied_Players_Is_Not_The_Current_MPV : When_There_Is_A_Tie
                {
                    [SetUp]
                    public override void SetUp()
                    {
                        GameDefinition.MVP = new MVP { PlayerId = 500 };

                        base.SetUp();
                    }

                    [Test]
                    public void Then_Return_Recent_Oldes_Player_Tied()
                    {
                        Result.PlayerId.ShouldBe(MVPPlayerId);
                    }
                }
            }
        }
    }
}
