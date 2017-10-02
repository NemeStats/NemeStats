using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    [TestFixture]
    public class GetPlayersToCreateTests
    {
        private string _currentUserId = "some id";
        private int _gamingGroupId = 1;
        private Player _playerForCurrentUser;
        private Player _expectedRecentPlayer1;
        private Player _expectedRecentPlayer2;
        private Player _expectedRecentPlayer3;
        private Player _expectedRecentPlayer4;
        private Player _expectedRecentPlayer5;
        private Player _expectedOtherPlayer1;
        private Player _expectedOtherPlayer2;


        private AutoMocker<PlayerRetriever> _autoMocker;
            
        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<PlayerRetriever>();
            _playerForCurrentUser = new Player
            {
                ApplicationUserId = _currentUserId,
                GamingGroupId = _gamingGroupId,
                Id = 2,
                PlayerGameResults = new List<PlayerGameResult>()
            };
            _expectedRecentPlayer1 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Id = 10,
                PlayerGameResults = new List<PlayerGameResult>
                {
                    //--add a played game that was in the past to make sure that the other played game
                    // that was today gets precedence
                    new PlayerGameResult
                    {
                        PlayedGame = new PlayedGame
                        {
                            DatePlayed = DateTime.UtcNow.Date.AddMonths(-1)
                        }
                    },
                    new PlayerGameResult
                    {
                        PlayedGame = new PlayedGame
                        {
                            DatePlayed = DateTime.UtcNow.Date
                        }
                    }
                }
            };
            _expectedRecentPlayer2 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Id = 20,
                PlayerGameResults = new List<PlayerGameResult>
                {
                    new PlayerGameResult
                    {
                        PlayedGame = new PlayedGame
                        {
                            DatePlayed = DateTime.UtcNow.Date.AddDays(-2)
                        }
                    }
                }
            };
            _expectedRecentPlayer3 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Id = 30,
                PlayerGameResults = new List<PlayerGameResult>
                {
                    new PlayerGameResult
                    {
                        PlayedGame = new PlayedGame
                        {
                            DatePlayed = DateTime.UtcNow.Date.AddDays(-3)
                        }
                    }
                }
            };
            _expectedRecentPlayer4 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Id = 40,
                //--pick a name that will sort higher than expectedRecentPlayer5
                Name = "a",
                PlayerGameResults = new List<PlayerGameResult>
                {
                    new PlayerGameResult
                    {
                        PlayedGame = new PlayedGame
                        {
                            DatePlayed = DateTime.UtcNow.Date.AddDays(-4)
                        }
                    }
                }
            };
            _expectedRecentPlayer5 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Id = 40,
                Name = "b",
                PlayerGameResults = new List<PlayerGameResult>
                {
                    new PlayerGameResult
                    {
                        PlayedGame = new PlayedGame
                        {
                            DatePlayed = DateTime.UtcNow.Date.AddDays(-4)
                        }
                    }
                }
            };
            _expectedOtherPlayer1 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Id = 100,
                Name = "Arthur",
                PlayerGameResults = new List<PlayerGameResult>()
            };
            _expectedOtherPlayer2 = new Player
            {
                GamingGroupId = _gamingGroupId,
                Id = 200,
                Name = "Beatrice",
                //--set a date on a played game to prove that it doesn't sort on this for other players
                PlayerGameResults = new List<PlayerGameResult>
                {
                    new PlayerGameResult
                    {
                        PlayedGame = new PlayedGame
                        {
                            DatePlayed = DateTime.UtcNow.Date.AddYears(-1)
                        }
                    }
                }
            };
            var queryable = new List<Player>
            {
                //--add the players to the queryable in a semi-random order since they should get sorted anyway
                _playerForCurrentUser,
                _expectedOtherPlayer2,
                _expectedOtherPlayer1,
                _expectedRecentPlayer2,
                _expectedRecentPlayer3,
                _expectedRecentPlayer4,
                _expectedRecentPlayer5,
                _expectedRecentPlayer1,
                //--add a Player with a different gaming group id to make sure it doesn't show up
                new Player
                {
                    GamingGroupId = -999,
                    Name = "A",
                    PlayerGameResults = new List<PlayerGameResult>()
                    {
                        new PlayerGameResult
                        {
                            PlayedGame = new PlayedGame
                            {
                                DatePlayed = DateTime.MaxValue
                            }
                        }
                    }
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>()).Return(queryable);
        }

        [Test]
        public void It_Sets_The_UserPlayer_To_The_Current_Users_Player_If_There_Is_One()
        {
            //--arrange

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetPlayersToCreate(_currentUserId, _gamingGroupId);

            //--assert
            actualResult.UserPlayer.ShouldNotBeNull();
            actualResult.UserPlayer.PlayerId.ShouldBe(_playerForCurrentUser.Id);
        }

        [Test]
        public void It_Sets_The_UserPlayer_To_Null_If_There_Is_Not_One()
        {
            //--arrange

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetPlayersToCreate("invalid user", _gamingGroupId);

            //--assert
            actualResult.UserPlayer.ShouldBeNull();
        }

        [Test]
        public void It_Sets_The_RecentPlayers_To_Include_The_Last_5_Players_To_Play_A_Game_With_Name_As_The_Tiebreaker()
        {
            //--arrange

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetPlayersToCreate(_currentUserId, _gamingGroupId);

            //--assert
            actualResult.RecentPlayers.ShouldNotBeNull();
            actualResult.RecentPlayers.Count.ShouldBe(5);
            actualResult.RecentPlayers[0].PlayerId.ShouldBe(_expectedRecentPlayer1.Id);
            actualResult.RecentPlayers[1].PlayerId.ShouldBe(_expectedRecentPlayer2.Id);
            actualResult.RecentPlayers[2].PlayerId.ShouldBe(_expectedRecentPlayer3.Id);
            actualResult.RecentPlayers[3].PlayerId.ShouldBe(_expectedRecentPlayer4.Id);
            actualResult.RecentPlayers[4].PlayerId.ShouldBe(_expectedRecentPlayer5.Id);
        }

        [Test]
        public void It_Sets_The_OtherPlayers_To_Include_Everything_Other_Than_The_Recent_Players_And_Current_Users_Player_Ordered_By_Name_Only()
        {
            //--arrange

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetPlayersToCreate(_currentUserId, _gamingGroupId);

            //--assert
            actualResult.OtherPlayers.ShouldNotBeNull();
            actualResult.OtherPlayers.Count.ShouldBe(2);
            actualResult.OtherPlayers[0].PlayerId.ShouldBe(_expectedOtherPlayer1.Id);
            actualResult.OtherPlayers[1].PlayerId.ShouldBe(_expectedOtherPlayer2.Id);
        }

        [Test]
        public void It_Returns_Null_And_Two_Empty_Lists_On_The_PlayersToCreateModel_If_There_Is_No_Data()
        {
            //--arrange

            //--act
            var actualResult = _autoMocker.ClassUnderTest.GetPlayersToCreate(_currentUserId, -1);

            //--assert
            actualResult.ShouldNotBeNull();
            actualResult.UserPlayer.ShouldBeNull();
            actualResult.RecentPlayers.ShouldNotBeNull();
            actualResult.RecentPlayers.Count.ShouldBe(0);
            actualResult.OtherPlayers.ShouldNotBeNull();
            actualResult.OtherPlayers.Count.ShouldBe(0);
        }
    }
}
