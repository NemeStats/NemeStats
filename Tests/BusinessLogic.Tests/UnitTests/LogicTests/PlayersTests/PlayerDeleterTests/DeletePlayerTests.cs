using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
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

        protected void SetupDefaultExpectations(
            bool setupPlayer = true,
            bool setupAchievements = true,
            bool setupChampions = true,
            bool setupNemeses = true)
        {
            if (setupPlayer)
            {
                var players = new List<Player>
                {
                    new Player {Id = PlayerId, PlayerGameResults = new List<PlayerGameResult> ()}
                };

                AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
            }

            if (setupAchievements)
            {
                AutoMocker.Get<IDataContext>()
                    .Expect(mock => mock.GetQueryable<PlayerAchievement>())
                    .Return(new List<PlayerAchievement>().AsQueryable());
            }

            if (setupChampions)
            {
                AutoMocker.Get<IDataContext>()
                    .Expect(mock => mock.GetQueryable<Champion>())
                    .Return(new List<Champion>().AsQueryable());
            }

            if (setupNemeses)
            {
                AutoMocker.Get<IDataContext>()
                    .Expect(mock => mock.GetQueryable<Nemesis>())
                    .Return(new List<Nemesis>().AsQueryable());
            }
        }
    }

    public class When_Player_Not_Exists : DeletePlayerTests
    {
        [Test]
        public void Then_Throw_Exception()
        {
            SetupDefaultExpectations(setupPlayer: false);
            var players = new List<Player>
            {
                new Player
                {
                    Id = -1
                }
            };
            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());

            var expectedException = new ArgumentException("Player not exists", "playerId");

            var exception = Assert.Throws<ArgumentException>(() => AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }
    }

    public class When_Player_Has_PlayedGames : DeletePlayerTests
    {
        [Test]
        public void Then_Throw_Exception()
        {
            SetupDefaultExpectations(setupPlayer: false);
            var players = new List<Player>
            {
                new Player
                {
                    Id = PlayerId,
                    PlayerGameResults = new List<PlayerGameResult>
                    {
                        new PlayerGameResult
                        {
                            PlayerId = PlayerId
                        }
                    }
                }
            };

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());

            var expectedException = new Exception("You can not delete players with any played game");

            var exception = Assert.Throws<Exception>(() => AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }
    }

    public class When_Player_Has_No_PlayedGames : DeletePlayerTests
    {
        [Test]
        public void Then_Deletes_The_Player()
        {
            SetupDefaultExpectations();

            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<Player>(PlayerId, CurrentUser));
        }
    }

    public class When_Player_Has_Achievements : DeletePlayerTests
    {
        [Test]
        public void Then_Deletes_The_Achievements_For_That_Player()
        {
            SetupDefaultExpectations(setupAchievements: false);

            var players = new List<Player>
            {
                new Player {Id = PlayerId, PlayerGameResults = new List<PlayerGameResult> ()}
            };

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());

            var achievements = new List<PlayerAchievement>
            {
                new PlayerAchievement
                {
                    Id = 1,
                    PlayerId = PlayerId
                },
                new PlayerAchievement
                {
                    Id = 2,
                    PlayerId = PlayerId
                },
                new PlayerAchievement
                {
                    Id = 3,
                    PlayerId = -1
                }
            };
            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<PlayerAchievement>()).Return(achievements.AsQueryable());

            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<PlayerAchievement>(achievements[0].Id, CurrentUser));
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<PlayerAchievement>(achievements[1].Id, CurrentUser));

            AutoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.DeleteById<PlayerAchievement>(achievements[2].Id, CurrentUser));
        }
    }

    public class When_Deleted_Player_Is_Champion : DeletePlayerTests
    {
        private int _championId = 50;
        private int _championId2 = 51;
        private int _gameDefinitionId = 200;
        private int _gameDefinitionId2 = 201;

        public override void SetUp()
        {
            base.SetUp();
            SetupDefaultExpectations(setupChampions: false);

            var champions = new List<Champion>
            {
                new Champion
                {
                    PlayerId = PlayerId,
                    Id = _championId
                },
                new Champion
                {
                    PlayerId = PlayerId,
                    Id = _championId2
                },
                new Champion
                {
                    PlayerId = -1
                }
            };
            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>()).Return(champions.AsQueryable());

            var gameDefinitions = new List<GameDefinition>
            {
                new GameDefinition
                {
                    ChampionId = _championId,
                    Id = _gameDefinitionId
                },
                new GameDefinition
                {
                    Id = _gameDefinitionId2,
                    ChampionId = _championId2
                }
            };
            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(gameDefinitions.AsQueryable());
        }

        [Test]
        public void Then_Clears_Out_The_Champion_For_Each_Championed_Game()
        {
            //--arrange

            //--act
            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            //--assert
            AutoMocker.Get<IDataContext>().AssertWasCalled(
                mock => mock.Save(Arg<GameDefinition>.Matches(x => x.Id == _gameDefinitionId && x.ChampionId == null), Arg<ApplicationUser>.Is.Same(CurrentUser)));
            AutoMocker.Get<IDataContext>().AssertWasCalled(
                mock => mock.Save(Arg<GameDefinition>.Matches(x => x.Id == _gameDefinitionId2 && x.ChampionId == null), Arg<ApplicationUser>.Is.Same(CurrentUser)));
        }

        [Test]
        public void Then_Recalculates_The_Champion_For_Each_Game_Championed()
        {
            //--act
            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            //--assert
            AutoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion(_gameDefinitionId, CurrentUser));
            AutoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion(_gameDefinitionId2, CurrentUser));
        }
    }

    public class When_Deleted_Player_Is_Nemesis : DeletePlayerTests
    {
        private int _nemesisId1 = 1;
        private int _nemesisId2 = 2;
        private int _nemesisId3 = 3;
        private int _nemesisId4 = 4;

        private int _playerIdWithPreviousNemesis = 10;
        private int _playerId2WithPreviousNemesis = 11;
        private int _playerIdWithCurrentNemesis = 20;
        private int _playerId2WithCurrentNemesis = 21;

        public override void SetUp()
        {
            base.SetUp();
            SetupDefaultExpectations(setupPlayer: false, setupNemeses: false);

            var nemeses = new List<Nemesis>
            {
                new Nemesis
                {
                    NemesisPlayerId = PlayerId,
                    Id = _nemesisId1
                },
                new Nemesis
                {
                    NemesisPlayerId = PlayerId,
                    Id = _nemesisId2
                },
                new Nemesis
                {
                    NemesisPlayerId = PlayerId,
                    Id = _nemesisId3
                },
                new Nemesis
                {
                    NemesisPlayerId = PlayerId,
                    Id = _nemesisId4
                }
            };

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Nemesis>()).Return(nemeses.AsQueryable());

            var players = new List<Player>
            {
                new Player
                {
                    Id = PlayerId,
                    PlayerGameResults = new List<PlayerGameResult> ()
                },
                new Player
                {
                    Id = _playerIdWithCurrentNemesis,
                    NemesisId = _nemesisId1
                },
                new Player
                {
                    Id = _playerId2WithCurrentNemesis,
                    NemesisId = _nemesisId2
                },
                new Player
                {
                    Id = _playerIdWithPreviousNemesis,
                    PreviousNemesisId = _nemesisId3
                },
                new Player
                {
                    Id = _playerId2WithPreviousNemesis,
                    PreviousNemesisId = _nemesisId4
                }
            };

            AutoMocker.Get<IDataContext>().Expect(m => m.GetQueryable<Player>()).Return(players.AsQueryable());
        }

        [Test]
        public void Then_Clears_Out_The_Previous_Nemesis_Of_All_Players_Who_Had_This_Previous_Nemesis()
        {
            //--arrange

            //--act
            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            //--assert
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(p => p.Id == _playerIdWithPreviousNemesis && p.PreviousNemesisId == null), Arg<ApplicationUser>.Is.Same(CurrentUser)));
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(p => p.Id == _playerId2WithPreviousNemesis && p.PreviousNemesisId == null), Arg<ApplicationUser>.Is.Same(CurrentUser)));
        }

        [Test]
        public void Then_Clears_Out_The_Current_Nemesis_Of_All_Players_Who_Had_This_Current_Nemesis()
        {
            //--arrange

            //--act
            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            //--assert
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(p => p.Id == _playerIdWithCurrentNemesis && p.NemesisId == null), Arg<ApplicationUser>.Is.Same(CurrentUser)));
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(p => p.Id == _playerId2WithCurrentNemesis && p.NemesisId == null), Arg<ApplicationUser>.Is.Same(CurrentUser)));
        }

        [Test]
        public void Then_Recalculates_The_Nemesis_For_The_Players_Who_Had_This_Current_Nemesis_Only()
        {
            //--arrange

            //--act
            AutoMocker.ClassUnderTest.DeletePlayer(PlayerId, CurrentUser);

            //--assert
            AutoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(_playerIdWithCurrentNemesis, CurrentUser));
            AutoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(_playerId2WithCurrentNemesis, CurrentUser));

            AutoMocker.Get<INemesisRecalculator>().AssertWasNotCalled(mock => mock.RecalculateNemesis(_playerIdWithPreviousNemesis, CurrentUser));
            AutoMocker.Get<INemesisRecalculator>().AssertWasNotCalled(mock => mock.RecalculateNemesis(_playerId2WithPreviousNemesis, CurrentUser));
        }
    }
}
