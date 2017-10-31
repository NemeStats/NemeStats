﻿using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayerAchievementTests
{
    [TestFixture]
    public class GetPlayerAchievementTests
    {
        private RhinoAutoMocker<PlayerAchievementRetriever> _autoMocker;
        private AchievementId _achievementId = AchievementId.Brains;
        private PlayerAchievement _playerAchievement;
        private Player _expectedPlayer;
        private IAchievement _expectedAchievement;

        internal static readonly AchievementAwarded ExpectedAchievementAwarded = new AchievementAwarded
        {
            LevelAwarded = AchievementLevel.Silver,
            PlayerProgress = 42,
            RelatedEntities = new List<int> { 1 }
        };

        [SetUp]
        public virtual void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<PlayerAchievementRetriever>();
            _autoMocker.PartialMockTheClassUnderTest();

            _expectedAchievement = new FakeAchievement();
            _autoMocker.Get<IAchievementRetriever>().Expect(mock => mock.GetAchievement(_achievementId)).Return(_expectedAchievement);

            _expectedPlayer = new Player
            {
                Name = "some player name",
                Id = 51
            };
            _playerAchievement = new PlayerAchievement
            {
                AchievementId = _achievementId,
                DateCreated = DateTime.UtcNow.AddDays(-15),
                LastUpdatedDate = DateTime.UtcNow,
                PlayerId = _expectedPlayer.Id
            };
            var playerAchievementsQueryable = new List<PlayerAchievement>
            {
                //--achievement with non-matching id
                new PlayerAchievement(),
                new PlayerAchievement
                {
                    AchievementId = _achievementId
                },
                _playerAchievement
            }.AsQueryable();

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>()).Return(playerAchievementsQueryable);
        }

        private class FakeAchievement : IAchievement
        {
            public AchievementId Id { get; } = AchievementId.Brains;
            public AchievementGroup Group { get; } = AchievementGroup.NotApplicable;
            public string Name { get; } = "fake name";
            public string DescriptionFormat { get; } = "{0}";
            public string Description { get; } = "fake description";
            public string IconClass { get; } = "fake icon class";
            public Dictionary<AchievementLevel, int> LevelThresholds { get; } = new Dictionary<AchievementLevel, int>();
            public AchievementAwarded IsAwardedForThisPlayer(int playerId)
            {
                return ExpectedAchievementAwarded;
            }
        }

        [Test]
        public void It_Returns_Basic_Achievement_Data()
        {
            //--arrange
            var expectedAchievement = new FakeAchievement();

            //--act
            var query = new PlayerAchievementQuery(_achievementId, _expectedPlayer.Id);
            var result = _autoMocker.ClassUnderTest.GetPlayerAchievement(query);

            //--assert
            result.ShouldNotBeNull();
            result.AchievementId.ShouldBe(expectedAchievement.Id);
            result.Description.ShouldBe(expectedAchievement.Description);
            result.IconClass.ShouldBe(expectedAchievement.IconClass);
            result.LevelThresholds.ShouldBe(expectedAchievement.LevelThresholds);
        }


        [Test]
        public void It_Returns_The_Number_Of_Players_With_The_Achievement()
        {
            //--arrange

            //--act
            var query = new PlayerAchievementQuery(_achievementId, _expectedPlayer.Id);
            var result = _autoMocker.ClassUnderTest.GetPlayerAchievement(query);

            //--assert
            result.NumberOfPlayersWithThisAchievement.ShouldBe(2);
        }

        [Test]
        public void It_Doesnt_Set_The_Player_Achievement_Dates_If_The_Specified_Player_Has_Not_Earned_The_Achievement()
        {
            //--arrange

            //--act
            var query = new PlayerAchievementQuery(_achievementId, -1);
            var result = _autoMocker.ClassUnderTest.GetPlayerAchievement(query);

            //--assert
            result.DateCreated.ShouldBeNull();
            result.LastUpdatedDate.ShouldBeNull();
        }

        [Test]
        public void It_Sets_The_Player_Achievement_Dates_If_The_Specified_Player_Has_Earned_The_Achievement()
        {
            //--arrange

            //--act
            var query = new PlayerAchievementQuery(_achievementId, _expectedPlayer.Id);
            var result = _autoMocker.ClassUnderTest.GetPlayerAchievement(query);

            //--assert
            result.DateCreated.ShouldBe(_playerAchievement.DateCreated);
            result.LastUpdatedDate.ShouldBe(_playerAchievement.LastUpdatedDate);
        }

        [Test]
        public void It_Sets_The_Player_Information()
        {
            //--arrange
            _autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(_expectedPlayer.Id))
                .Return(_expectedPlayer);

            //--act
            var query = new PlayerAchievementQuery(_achievementId, _expectedPlayer.Id);
            var result = _autoMocker.ClassUnderTest.GetPlayerAchievement(query);

            //--assert
            result.PlayerName.ShouldBe(_expectedPlayer.Name);
            result.PlayerId.ShouldBe(_expectedPlayer.Id);
        }

        [Test]
        public void It_Uses_The_Player_Information_For_The_Users_Current_Gaming_Group_Related_Player_If_ApplicationUserId_Is_Passed_Instead_Of_Player_Id()
        {
            //--arrange
            var currentUser = new ApplicationUser
            {
                Id = "some user id",
                CurrentGamingGroupId = 718
            };
            var expectedPlayer = new Player
            {
                Id = 14,
                Name = "player name"
            };
            _autoMocker.Get<IPlayerRetriever>()
                .Expect(mock => mock.GetPlayerForCurrentUser(currentUser.Id, currentUser.CurrentGamingGroupId.Value))
                .Return(expectedPlayer);

            //--act
            var query = new PlayerAchievementQuery(_achievementId, currentUser.Id, currentUser.CurrentGamingGroupId.Value);
            var result = _autoMocker.ClassUnderTest.GetPlayerAchievement(query);

            //--assert
            result.PlayerName.ShouldBe(expectedPlayer.Name);
            result.PlayerId.ShouldBe(expectedPlayer.Id);
        }


        [Test]
        public void It_Sets_The_Players_Achievement_Progress_Information()
        {
            //--arrange
            _autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(_expectedPlayer.Id))
                .Return(_expectedPlayer);

            //--act
            var query = new PlayerAchievementQuery(_achievementId, _expectedPlayer.Id);
            var result = _autoMocker.ClassUnderTest.GetPlayerAchievement(query);

            //--assert
            result.AchievementLevel.ShouldBe(ExpectedAchievementAwarded.LevelAwarded);
            result.PlayerProgress.ShouldBe(ExpectedAchievementAwarded.PlayerProgress);
        }

        [Test]
        public void It_Sets_The_Players_Related_Entities()
        {
            //--arrange

            //--act
            var query = new PlayerAchievementQuery(_achievementId, _expectedPlayer.Id);
            var result = _autoMocker.ClassUnderTest.GetPlayerAchievement(query);

            //--assert
            _autoMocker.ClassUnderTest.Expect(mock => mock.SetRelatedEntities(
                Arg<AchievementGroup>.Is.Same(_expectedAchievement.Group),
                Arg<PlayerAchievementDetails>.Is.Anything,
                Arg<List<int>>.Is.Same(ExpectedAchievementAwarded.RelatedEntities)));
        }

        [TestFixture]
        public class When_Calling_SetRelatedEntities : GetPlayerAchievementTests
        {
            private PlayerAchievementDetails _playerAchievementDetails;
            private List<int> _entityIds;
            private int _expectedEntityId1 = 1;
            private int _expectedEntityId2 = 2;
            private GameDefinition _expectedGameDefinition;
            private GameDefinition _expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId;
            private BoardGameGeekInfo _expectedBoardGameGeekInfo;
            private PlayedGame _expectedPlayedGame1;
            private PlayedGame _expectedPlayedGame2;
            private Player _expectedPlayer1;
            private Player _expectedPlayer2;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                _playerAchievementDetails = new PlayerAchievementDetails();

                _entityIds = new List<int> { _expectedEntityId1, _expectedEntityId2 };

                SetUpGameDefinitionStuff();

                SetUpPlayedGameStuff();

                _expectedPlayer1 = new Player
                {
                    Id = _expectedEntityId1,
                    Name = "aaa player 1 name",
                    GamingGroupId = 1000,
                    GamingGroup = new GamingGroup
                    {
                        Name = "Gaming group name 1"
                    }
                };

                _expectedPlayer2 = new Player
                {
                    Id = _expectedEntityId2,
                    Name = "bbb player 2 name",
                    GamingGroupId = 1001,
                    GamingGroup = new GamingGroup
                    {
                        Name = "Gaming group name 2"
                    }
                };

                var playerQueryable = new List<Player>
                {
                    _expectedPlayer2,
                    _expectedPlayer1,
                    new Player()
                }.AsQueryable();
                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(playerQueryable);
            }

            private void SetUpPlayedGameStuff()
            {
                _expectedBoardGameGeekInfo = new BoardGameGeekInfo
                {
                    BoardGameGeekGameDefinitionId = _expectedGameDefinition.BoardGameGeekGameDefinitionId.Value
                };
                _autoMocker.Get<IBoardGameGeekGameDefinitionInfoRetriever>()
                    .Expect(mock => mock.GetResults(_expectedGameDefinition.BoardGameGeekGameDefinitionId.Value))
                    .Return(_expectedBoardGameGeekInfo);

                _expectedPlayedGame1 = new PlayedGame
                {
                    Id = _expectedEntityId1,
                    GameDefinition = new GameDefinition
                    {
                        Id = 101,
                        BoardGameGeekGameDefinitionId = 1,
                        Name = "game 1 name",
                        BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition
                        {
                            Thumbnail = "some thumbnail"
                        }
                    },
                    //--have to set PlayerGameResults with a winner so that LINQ doesn't blow up -- even though EntityFramework can handle this just fine
                    PlayerGameResults = new List<PlayerGameResult>
                    {
                        new PlayerGameResult
                        {
                            GameRank = 1,
                            PlayerId = 51,
                            Player = new Player
                            {
                                Name = "some player name"

                            }
                        }
                    },
                    WinnerType = WinnerTypes.PlayerWin,
                    DatePlayed = DateTime.UtcNow
                };
                _expectedPlayedGame2 = new PlayedGame
                {
                    Id = _expectedEntityId2,
                    GameDefinition = new GameDefinition
                    {
                        Id = 102,
                        Name = "game 2 name",
                        //--have to do this because entity framework can handle the null relationship, but pure LINQ cannot :(
                        BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition()
                    },
                    PlayerGameResults = new List<PlayerGameResult>
                    {
                        new PlayerGameResult
                        {
                            GameRank = 1,
                            PlayerId = 51,
                            Player = new Player
                            {
                                Name = "some player name"

                            }
                        }
                    },
                    WinnerType = WinnerTypes.TeamLoss,
                    DatePlayed = DateTime.UtcNow.AddDays(-10)
                };

                var playedGameQueryable = new List<PlayedGame>
                {
                    //--add this one first to make sure that the query orders correctly
                    _expectedPlayedGame2,
                    _expectedPlayedGame1,
                    new PlayedGame
                    {
                        PlayerGameResults = new List<PlayerGameResult>
                        {
                            new PlayerGameResult
                            {
                                GameRank = 1,
                                PlayerId = 51,
                                Player = new Player
                                {
                                    Name = "some player name"
                                    
                                }
                            }
                        }
                    }
                }.AsQueryable();
                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGameQueryable);
            }

            private void SetUpGameDefinitionStuff()
            {
                _expectedGameDefinition = new GameDefinition
                {
                    Id = _expectedEntityId1,
                    GamingGroupId = 4,
                    Name = "zzz some game definition name",
                    Description = "some game definition description",
                    BoardGameGeekGameDefinitionId = 53,
                    BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition()
                };
                _expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId = new GameDefinition
                {
                    Id = _expectedEntityId2,
                    GamingGroupId = 4,
                    Name = "aaa some game definition name 2",
                    Description = "some game definition description 2",
                    BoardGameGeekGameDefinitionId = null,
                    //--have to do this because entity framework can handle the null relationship, but pure LINQ cannot :(
                    BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition()
                };
                var gameDefinitionQueryable = new List<GameDefinition>
                {
                    //--add this one first to make sure that the query orders correctly
                    _expectedGameDefinition,
                    _expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId,
                    new GameDefinition
                    {
                        Id = -1,
                        //--have to do this because entity framework can handle the null relationship, but pure LINQ cannot :(
                        BoardGameGeekGameDefinition = new BoardGameGeekGameDefinition()
                    }
                }.AsQueryable();

                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(gameDefinitionQueryable);
            }

            [Test]
            public void It_Sets_Related_Game_Definitions_When_The_AchievementGroup_Is_Game_And_Orders_By_Game_Definition_Name()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.SetRelatedEntities(AchievementGroup.Game, _playerAchievementDetails, _entityIds);

                //--assert
                var firstGameDefinitionDetails = _playerAchievementDetails.RelatedGameDefinitions[0];
                firstGameDefinitionDetails.Id.ShouldBe(_expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId.Id);
                firstGameDefinitionDetails.GamingGroupId.ShouldBe(_expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId.GamingGroupId);
                firstGameDefinitionDetails.Name.ShouldBe(_expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId.Name);
                firstGameDefinitionDetails.BoardGameGeekInfo.ShouldBeNull();

                var secondGameDefinitionDetails = _playerAchievementDetails.RelatedGameDefinitions[1];
                secondGameDefinitionDetails.Id.ShouldBe(_expectedGameDefinition.Id);
                secondGameDefinitionDetails.GamingGroupId.ShouldBe(_expectedGameDefinition.GamingGroupId);
                secondGameDefinitionDetails.Name.ShouldBe(_expectedGameDefinition.Name);
                secondGameDefinitionDetails.BoardGameGeekInfo.ShouldBeSameAs(_expectedBoardGameGeekInfo);
            }

            [Test]
            public void It_Sets_Related_Played_Games_When_The_AchievementGroup_Is_PlayedGame_And_Orders_By_DatePlayed_Descending()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.SetRelatedEntities(AchievementGroup.PlayedGame, _playerAchievementDetails, _entityIds);

                //--assert
                _playerAchievementDetails.RelatedPlayedGames.ShouldNotBeNull();
                _playerAchievementDetails.RelatedPlayedGames.Count.ShouldBe(2);

                var firstResult = _playerAchievementDetails.RelatedPlayedGames[0];
                firstResult.BoardGameGeekGameDefinitionId.ShouldBe(_expectedPlayedGame1.GameDefinition.BoardGameGeekGameDefinitionId);
                firstResult.DatePlayed.ShouldBe(_expectedPlayedGame1.DatePlayed);
                firstResult.GameDefinitionName.ShouldBe(_expectedPlayedGame1.GameDefinition.Name);
                firstResult.GameDefinitionId.ShouldBe(_expectedPlayedGame1.GameDefinitionId);
                firstResult.PlayedGameId.ShouldBe(_expectedPlayedGame1.Id);
                firstResult.ThumbnailImageUrl.ShouldBe(_expectedPlayedGame1.GameDefinition.BoardGameGeekGameDefinition.Thumbnail);
                firstResult.WinnerType.ShouldBe(_expectedPlayedGame1.WinnerType);

                var secondResult = _playerAchievementDetails.RelatedPlayedGames[1];
                secondResult.BoardGameGeekGameDefinitionId.ShouldBe(_expectedPlayedGame2.GameDefinition.BoardGameGeekGameDefinitionId);
                secondResult.DatePlayed.ShouldBe(_expectedPlayedGame2.DatePlayed);
                secondResult.GameDefinitionName.ShouldBe(_expectedPlayedGame2.GameDefinition.Name);
                secondResult.GameDefinitionId.ShouldBe(_expectedPlayedGame2.GameDefinitionId);
                secondResult.PlayedGameId.ShouldBe(_expectedPlayedGame2.Id);
                secondResult.ThumbnailImageUrl.ShouldBeNull();
                secondResult.WinnerType.ShouldBe(_expectedPlayedGame2.WinnerType);
            }

            public void It_Sets_Related_Players_When_The_AchievementGroup_Is_Player_And_Orders_By_Player_Name()
            {
                //--arrange

                //--act
                _autoMocker.ClassUnderTest.SetRelatedEntities(AchievementGroup.Player, _playerAchievementDetails, _entityIds);

                //--assert
                _playerAchievementDetails.RelatedPlayers.ShouldNotBeNull();
                _playerAchievementDetails.RelatedPlayers.Count.ShouldBe(2);

                var firstResult = _playerAchievementDetails.RelatedPlayers[0];
                firstResult.GamingGroupId.ShouldBe(_expectedPlayer1.GamingGroupId);
                firstResult.GamingGroupName.ShouldBe(_expectedPlayer1.GamingGroup.Name);
                firstResult.PlayerId.ShouldBe(_expectedPlayer1.Id);
                firstResult.PlayerName.ShouldBe(_expectedPlayer1.Name);

                var secondResult = _playerAchievementDetails.RelatedPlayers[1];
                secondResult.GamingGroupId.ShouldBe(_expectedPlayer2.GamingGroupId);
                secondResult.GamingGroupName.ShouldBe(_expectedPlayer2.GamingGroup.Name);
                secondResult.PlayerId.ShouldBe(_expectedPlayer2.Id);
                secondResult.PlayerName.ShouldBe(_expectedPlayer2.Name);
            }
        }
    }
}
