using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayerAchievementTests
{
    [TestFixture]
    public class GetCurrentPlayerAchievementDetailsTests
    {
        private RhinoAutoMocker<PlayerAchievementRetriever> _autoMocker;
        private ApplicationUser _currentUser;
        private AchievementId _achievementId = AchievementId.Brains;
        private PlayerAchievement _currentPlayerAchievement;
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
            _currentUser = new ApplicationUser();

            _expectedAchievement = new FakeAchievement();
            _autoMocker.Get<IAchievementRetriever>().Expect(mock => mock.GetAchievement(_achievementId)).Return(_expectedAchievement);

            _expectedPlayer = new Player
            {
                ApplicationUserId = _currentUser.Id,
                Name = "some player name",
                Id = 51
            };
            _currentPlayerAchievement = new PlayerAchievement
            {
                AchievementId = _achievementId,
                DateCreated = DateTime.UtcNow.AddDays(-15),
                LastUpdatedDate = DateTime.UtcNow,
                Player = _expectedPlayer
            };
            var playerAchievementsQueryable = new List<PlayerAchievement>
            {
                //--achievement with non-matching id
                new PlayerAchievement(),
                new PlayerAchievement
                {
                    AchievementId = _achievementId
                },
                _currentPlayerAchievement
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
            var result = _autoMocker.ClassUnderTest.GetCurrentPlayerAchievementDetails(expectedAchievement.Id, _currentUser);

            //--assert
            result.ShouldNotBeNull();
            result.AchievementId.ShouldBe(expectedAchievement.Id);
            result.AchievementDescription.ShouldBe(expectedAchievement.Description);
            result.AchievementIconClass.ShouldBe(expectedAchievement.IconClass);
            result.LevelThresholds.ShouldBe(expectedAchievement.LevelThresholds);
        }


        [Test]
        public void It_Returns_The_Number_Of_Players_With_The_Achievement()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetCurrentPlayerAchievementDetails(_achievementId, _currentUser);

            //--assert
            result.NumberOfPlayersWithThisAchievement.ShouldBe(2);
        }

        [Test]
        public void It_Doesnt_Set_The_Player_Achievement_Dates_If_The_Current_Player_Has_Not_Earned_The_Achievement()
        {
            //--arrange
            _currentUser.Id = "some different Id";

            //--act
            var result = _autoMocker.ClassUnderTest.GetCurrentPlayerAchievementDetails(_achievementId, _currentUser);

            //--assert
            result.DateCreated.ShouldBeNull();
            result.LastUpdatedDate.ShouldBeNull();
        }

        [Test]
        public void It_Sets_The_Player_Achievement_Dates_If_The_Current_Player_Has_Earned_The_Achievement()
        {
            //--arrange

            //--act
            var result = _autoMocker.ClassUnderTest.GetCurrentPlayerAchievementDetails(_achievementId, _currentUser);

            //--assert
            result.DateCreated.ShouldBe(_currentPlayerAchievement.DateCreated);
            result.LastUpdatedDate.ShouldBe(_currentPlayerAchievement.LastUpdatedDate);
        }

        [Test]
        public void It_Doesnt_Bother_Trying_To_Get_Stats_For_The_Current_User_If_The_User_Is_Unauthenticated()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.GetCurrentPlayerAchievementDetails(_achievementId, new AnonymousApplicationUser());

            //--assert
            _autoMocker.Get<IPlayerRetriever>().AssertWasNotCalled(mock => mock.GetPlayerForCurrentUser(Arg<string>.Is.Anything, Arg<int>.Is.Anything));
            _autoMocker.Get<IPlayerAchievementRetriever>().AssertWasNotCalled(mock => mock.GetCurrentPlayerAchievementDetails(Arg<AchievementId>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Sets_The_Player_Information_If_The_User_Is_Authenticated()
        {
            //--arrange
            _autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerForCurrentUser(_currentUser.Id, _currentUser.CurrentGamingGroupId))
                .Return(_expectedPlayer);

            //--act
            var result = _autoMocker.ClassUnderTest.GetCurrentPlayerAchievementDetails(_achievementId, _currentUser);

            //--assert
            result.PlayerName.ShouldBe(_expectedPlayer.Name);
            result.PlayerId.ShouldBe(_expectedPlayer.Id);
        }

        [Test]
        public void It_Sets_The_Players_Achievement_Progress_Information_If_The_User_Is_Authenticated()
        {
            //--arrange
            _autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerForCurrentUser(_currentUser.Id, _currentUser.CurrentGamingGroupId))
                .Return(_expectedPlayer);

            //--act
            var result = _autoMocker.ClassUnderTest.GetCurrentPlayerAchievementDetails(_achievementId, _currentUser);

            //--assert
            result.AchievementLevel.ShouldBe(ExpectedAchievementAwarded.LevelAwarded);
            result.PlayerProgress.ShouldBe(ExpectedAchievementAwarded.PlayerProgress);
        }

        [Test]
        public void It_Sets_The_Players_Related_Entities_If_The_User_Is_Authenticated()
        {
            //--arrange

            //--act
            _autoMocker.ClassUnderTest.GetCurrentPlayerAchievementDetails(_achievementId, new AnonymousApplicationUser());

            //--assert
            _autoMocker.ClassUnderTest.Expect(mock => mock.SetRelatedEntities(
                Arg<AchievementGroup>.Is.Same(_expectedAchievement.Group),
                Arg<PlayerAchievementDetails>.Is.Anything,
                Arg<List<int>>.Is.Same(ExpectedAchievementAwarded.RelatedEntities)));
        }
        
        [TestFixture]
        public class When_Calling_SetRelatedEntities : GetCurrentPlayerAchievementDetailsTests
        {
            private PlayerAchievementDetails _playerAchievementDetails;
            private List<int> _entityIds;
            private int _expectedGameDefinitionId = 1;
            private int _expectedGameDefinitionId2 = 2;
            private GameDefinition _expectedGameDefinition;
            private GameDefinition _expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId;
            private BoardGameGeekInfo _expectedBoardGameGeekInfo;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                _playerAchievementDetails = new PlayerAchievementDetails();

                _entityIds = new List<int> { _expectedGameDefinitionId, _expectedGameDefinitionId2 };

                _expectedGameDefinition = new GameDefinition
                {
                    Id = _expectedGameDefinitionId,
                    GamingGroupId = 4,
                    Name = "zzz some game definition name",
                    Description = "some game definition description",
                    BoardGameGeekGameDefinitionId = 53
                };
                _expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId = new GameDefinition
                {
                    Id = _expectedGameDefinitionId2,
                    GamingGroupId = 4,
                    Name = "aaa some game definition name 2",
                    Description = "some game definition description 2",
                    BoardGameGeekGameDefinitionId = null
                };
                var queryable = new List<GameDefinition>
                {
                    _expectedGameDefinition,
                    _expectedGameDefinitionWithNoBoardGameGeekGameDefinitionId,
                    new GameDefinition
                    {
                        Id = -1
                    }
                }.AsQueryable();

                _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(queryable);

                _expectedBoardGameGeekInfo = new BoardGameGeekInfo
                {
                    BoardGameGeekGameDefinitionId = _expectedGameDefinition.BoardGameGeekGameDefinitionId.Value
                };
                _autoMocker.Get<IBoardGameGeekGameDefinitionInfoRetriever>()
                    .Expect(mock => mock.GetResults(_expectedGameDefinition.BoardGameGeekGameDefinitionId.Value))
                    .Return(_expectedBoardGameGeekInfo);
            }

            [Test]
            public void It_Sets_Related_Game_Definitions_When_The_AchievementGroup_Is_Game_And_Orders_By_Game_Definition_Name()
            {
                //--arrange

                //--act
                 _autoMocker.ClassUnderTest.SetRelatedEntities(AchievementGroup.Game, _playerAchievementDetails, _entityIds);

                //--assert
                _playerAchievementDetails.RelatedGameDefinitions.ShouldNotBeNull();
                _playerAchievementDetails.RelatedGameDefinitions.Count.ShouldBe(2);

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
        }
    }
}
