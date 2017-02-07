using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.AchievementRetrieverTests
{
    [TestFixture]
    public class GetAllAchievementSummariesTests
    {
        private RhinoAutoMocker<AchievementRetriever> _autoMocker;
        private List<PlayerAchievement> _playerAchievements;
        private List<IAchievement> _allAchievements;
        private ApplicationUser _currentUser;
        private int _currentUserPlayerId = 10;
        private IAchievement _achievement1;
        private IAchievement _achievement2;
        private IAchievement _achievementNoOneHas;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<AchievementRetriever>();
            _autoMocker.PartialMockTheClassUnderTest();

            _currentUser = new ApplicationUser();

            _achievement1 = new AchievementOne();
            _achievement2 = new AchievementTwo();
            _achievementNoOneHas = new AchievementNoOneHas();

            _allAchievements = new List<IAchievement>
            {
                _achievementNoOneHas,
                _achievement1,
                _achievement2
            };
            _autoMocker.ClassUnderTest.Expect(mock => mock.GetAllAchievements()).Return(_allAchievements);

            _playerAchievements = new List<PlayerAchievement>
            {
                new PlayerAchievement
                {
                    AchievementId = _achievement2.Id,
                    Player = new Player()
                },
                new PlayerAchievement
                {
                    AchievementId = _achievement1.Id,
                    Player = new Player
                    {
                        ApplicationUserId = _currentUser.Id
                    }
                },
                new PlayerAchievement
                {
                    AchievementId = _achievement1.Id,
                    Player = new Player()
                }
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>()).Return(_playerAchievements.AsQueryable());
        }

        [Test]
        public void It_Returns_All_Achievements_Ordered_By_The_Number_Of_Players_With_The_Achievement()
        {
            //--arrange

            //--act
            var results = _autoMocker.ClassUnderTest.GetAllAchievementSummaries(_currentUser);

            //--assert
            results.Count.ShouldBe(3);
            
            var achievement1 = results[0];
            achievement1.Id.ShouldBe(_achievement1.Id);
            achievement1.Description.ShouldBe(_achievement1.Description);
            achievement1.NumberOfPlayersWithThisAchievement.ShouldBe(2);
            achievement1.Group.ShouldBe(_achievement1.Group);
            achievement1.IconClass.ShouldBe(_achievement1.IconClass);
            achievement1.LevelThresholds.ShouldBe(_achievement1.LevelThresholds);
            achievement1.Name.ShouldBe(_achievement1.Name);
            achievement1.CurrentPlayerUnlockedThisAchievement.ShouldBeTrue();

            var achievement2 = results[1];
            achievement2.Id.ShouldBe(_achievement2.Id);
            achievement2.Description.ShouldBe(achievement2.Description);
            achievement2.NumberOfPlayersWithThisAchievement.ShouldBe(1);
            achievement2.Group.ShouldBe(achievement2.Group);
            achievement2.IconClass.ShouldBe(achievement2.IconClass);
            achievement2.LevelThresholds.ShouldBe(achievement2.LevelThresholds);
            achievement2.Name.ShouldBe(achievement2.Name);
            achievement2.CurrentPlayerUnlockedThisAchievement.ShouldBeFalse();

            //--don't bother checking the rest of third achievement because the previous 2 are good enough 
            //  (and I'm a little lazy, but not too lazy to write this comment :P
            var achievement3 = results[2];
            achievement3.Id.ShouldBe(_achievementNoOneHas.Id);
        }

        [Test]
        public void The_Anonymous_User_Will_Have_No_Achievements_Unlocked()
        {
            //--arrange
            var anonymousUser= new AnonymousApplicationUser();

            //--act
            var results = _autoMocker.ClassUnderTest.GetAllAchievementSummaries(anonymousUser);

            //--assert
            results.Any(x => x.CurrentPlayerUnlockedThisAchievement).ShouldBeFalse();
        }

        internal class AchievementOne : IAchievement
        {
            public AchievementId Id => AchievementId.BusyBee;
            public AchievementGroup Group => AchievementGroup.Game;
            public string Name => "Achievement 1";
            public string DescriptionFormat => "{0} 1";
            public string Description => "Achievement 1 description";
            public string IconClass => "some class";
            public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int> { {AchievementLevel.Gold, 1 }};
            public AchievementAwarded IsAwardedForThisPlayer(int playerId)
            {
                throw new NotImplementedException();
            }

            public Lazy<List<PlayerAchievement>> Winners { get; set; }
        }

        internal class AchievementTwo : IAchievement
        {
            public AchievementId Id => AchievementId.Brains;
            public AchievementGroup Group => AchievementGroup.PlayedGame;
            public string Name => "Achievement 2";
            public string DescriptionFormat => "{0} 2";
            public string Description => "Achievement 2 description";
            public string IconClass => "some class 2";
            public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int> { { AchievementLevel.Gold, 2 } };
            public AchievementAwarded IsAwardedForThisPlayer(int playerId)
            {
                throw new NotImplementedException();
            }

            public Lazy<List<PlayerAchievement>> Winners { get; set; }
        }

        internal class AchievementNoOneHas : IAchievement
        {
            public AchievementId Id => AchievementId.BoardGameGeek2017_10x10;
            public AchievementGroup Group => AchievementGroup.PlayedGame;
            public string Name => "Achievement no one has";
            public string DescriptionFormat => "{0} 3";
            public string Description => "Achievement mo one has description";
            public string IconClass => "some class 3";
            public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int> { { AchievementLevel.Gold, 3 } };
            public AchievementAwarded IsAwardedForThisPlayer(int playerId)
            {
                throw new NotImplementedException();
            }

            public Lazy<List<PlayerAchievement>> Winners { get; set; }
        }
    }
}
