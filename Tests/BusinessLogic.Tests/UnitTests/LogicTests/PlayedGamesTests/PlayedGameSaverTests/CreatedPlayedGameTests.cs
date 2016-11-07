#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Events;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Points;
using BusinessLogic.Logic.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class CreatePlayedGameTests : PlayedGameSaverTestBase
    {
        [SetUp]
        public void SetUp()
        {
            autoMocker.Get<IDataContext>()
                .Stub(s => s.Save(Arg<PlayedGame>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(null)
                .WhenCalled(a => a.ReturnValue = a.Arguments.First());
        }

        [Test]
        public void It_Validates_The_User_Has_Access_To_The_Specified_Gaming_Group_Id()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();
            newlyCompletedPlayedGame.GamingGroupId = GAMING_GROUP_ID;
            currentUser.CurrentGamingGroupId = 42;

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<ISecuredEntityValidator>().AssertWasCalled(mock => mock.ValidateAccess<GamingGroup>(newlyCompletedPlayedGame.GamingGroupId.Value, currentUser));
        }

        [Test]
        public void It_Doesnt_Bother_Validating_The_Gaming_Group_If_It_Wasnt_Explicitly_Set()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<ISecuredEntityValidator>().AssertWasNotCalled(mock => mock.ValidateAccess<GamingGroup>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Doesnt_Bother_Validating_The_Gaming_Group_If_It_Matches_The_Current_Users()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();
            newlyCompletedPlayedGame.GamingGroupId = currentUser.CurrentGamingGroupId;

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<ISecuredEntityValidator>().AssertWasNotCalled(mock => mock.ValidateAccess<GamingGroup>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSavesAPlayedGameIfThereIsAGameDefinition()
        {
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                                                Arg<PlayedGame>.Matches(game => game.GameDefinitionId == gameDefinition.Id
                                                    && game.NumberOfPlayers == newlyCompletedPlayedGame.PlayerRanks.Count()
                                                    && game.DatePlayed.Date.Equals(newlyCompletedPlayedGame.DatePlayed.Date)),
                                                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void It_Sets_The_WinnerType_To_Team_Win_If_All_Players_Won()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();
            newlyCompletedPlayedGame.PlayerRanks.ForEach(x => x.GameRank = 1);

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                                                Arg<PlayedGame>.Matches(game => game.WinnerType == WinnerTypes.TeamWin),
                                                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Sets_The_WinnerType_To_Team_Loss_If_All_Players_Lost()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();
            newlyCompletedPlayedGame.PlayerRanks.ForEach(x => x.GameRank = 2);

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                                                Arg<PlayedGame>.Matches(game => game.WinnerType == WinnerTypes.TeamLoss),
                                                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Sets_The_WinnerType_To_Player_Win_If_There_Was_At_Least_One_Winner_And_One_Loser()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                                                Arg<PlayedGame>.Matches(game => game.WinnerType == WinnerTypes.PlayerWin),
                                                Arg<ApplicationUser>.Is.Anything));
        }

        private NewlyCompletedGame CreateValidNewlyCompletedGame()
        {
            List<PlayerRank> playerRanks;

            var playerOneId = 3515;
            var playerTwoId = 15151;
            var playerOneRank = 1;
            var playerTwoRank = 2;
            var newlyCompletedGame = new NewlyCompletedGame
            { GameDefinitionId = gameDefinition.Id };
            playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank
            { PlayerId = playerOneId, GameRank = playerOneRank });
            playerRanks.Add(new PlayerRank
            { PlayerId = playerTwoId, GameRank = playerTwoRank });
            newlyCompletedGame.PlayerRanks = playerRanks;
            autoMocker.Get<IPointsCalculator>()
                .Expect(mock => mock.CalculatePoints(null, null))
                .IgnoreArguments()
                .Return(new Dictionary<int, PointsScorecard>
                {
                    {playerOneId, new PointsScorecard()},
                    {playerTwoId, new PointsScorecard()}
                });
            return newlyCompletedGame;
        }

        [Test]
        public void ItSetsNemePointsForEachPlayerGameResult()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerOneGameRank = 1;
            var playerTwoGameRank = 2;
            var playerRanks = new List<PlayerRank>
            {
                new PlayerRank
                {
                    PlayerId = playerOneId,
                    GameRank = playerOneGameRank
                },
                new PlayerRank
                {
                    PlayerId = playerTwoId,
                    GameRank = playerTwoGameRank
                }
            };
            gameDefinition.BoardGameGeekGameDefinitionId = 11;
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };
            var expectedBoardGameGeekGameDefinition = new BoardGameGeekGameDefinition();
            autoMocker.Get<IDataContext>()
                      .Expect(mock => mock.FindById<BoardGameGeekGameDefinition>(gameDefinition.BoardGameGeekGameDefinitionId))
                      .Return(expectedBoardGameGeekGameDefinition);

            var playerOneScorecard = new PointsScorecard
            {
                BasePoints = 1,
                GameDurationBonusPoints = 2,
                GameWeightBonusPoints = 3
            };
            var playerTwoScorecard = new PointsScorecard
            {
                BasePoints = 4,
                GameDurationBonusPoints = 5,
                GameWeightBonusPoints = 6
            };
            var expectedPointsDictionary = new Dictionary<int, PointsScorecard>
            {
                {playerOneId, playerOneScorecard},
                {playerTwoId, playerTwoScorecard}
            };

            autoMocker.Get<IPointsCalculator>().Expect(mock => mock.CalculatePoints(playerRanks, expectedBoardGameGeekGameDefinition))
                .Return(expectedPointsDictionary);

            //--Act
            var playedGame = autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            var playerOne = playedGame.PlayerGameResults.First(gameResult => gameResult.PlayerId == playerOneId);
            Assert.That(playerOne.GameDurationBonusPoints, Is.EqualTo(playerOneScorecard.GameDurationBonusPoints));
            Assert.That(playerOne.GameWeightBonusPoints, Is.EqualTo(playerOneScorecard.GameWeightBonusPoints));
            Assert.That(playerOne.NemeStatsPointsAwarded, Is.EqualTo(playerOneScorecard.BasePoints));

            var playerTwo = playedGame.PlayerGameResults.First(gameResult => gameResult.PlayerId == playerTwoId);
            Assert.That(playerTwo.GameDurationBonusPoints, Is.EqualTo(playerTwoScorecard.GameDurationBonusPoints));
            Assert.That(playerTwo.GameWeightBonusPoints, Is.EqualTo(playerTwoScorecard.GameWeightBonusPoints));
            Assert.That(playerTwo.NemeStatsPointsAwarded, Is.EqualTo(playerTwoScorecard.BasePoints));
        }

        [Test]
        public void ItSetsTheGamingGroupIdToThatOfTheUser()
        {
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = new List<PlayerRank>()
            };

            autoMocker.ClassUnderTest.Expect(logic => logic.MakePlayerGameResults(null, null))
                .IgnoreArguments()
                .Return(new List<PlayerGameResult>());

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<PlayedGame>.Matches(game => game.GamingGroupId == currentUser.CurrentGamingGroupId),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItCreatesAnApplicationLinkageForNemeStats()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();

            var expectedPlayedGame = new PlayedGame
            {
                Id = EXPECTED_PLAYED_GAME_ID
            };
            autoMocker.ClassUnderTest.Expect(partialMock => partialMock.TransformNewlyCompletedGameIntoPlayedGame(null, 0, null, null))
                .IgnoreArguments()
                .Return(expectedPlayedGame);

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<IApplicationLinker>().AssertWasCalled(mock => mock.LinkApplication(
                EXPECTED_PLAYED_GAME_ID,
                ApplicationLinker.APPLICATION_NAME_NEMESTATS,
                EXPECTED_PLAYED_GAME_ID.ToString()));
        }

        [Test]
        public void ItCreatesAnApplicationLinkageForEachSpecifiedApplicationLinkage()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();
            var expectedApplicationLinkage1 = new ApplicationLinkage
            {
                ApplicationName = "app1",
                EntityId = "1"
            };
            var expectedApplicationLinkage2 = new ApplicationLinkage
            {
                ApplicationName = "app2",
                EntityId = "2"
            };
            newlyCompletedPlayedGame.ApplicationLinkages = new List<ApplicationLinkage>
            {
                expectedApplicationLinkage1,
                expectedApplicationLinkage2
            };

            var expectedPlayedGame = new PlayedGame
            {
                Id = EXPECTED_PLAYED_GAME_ID
            };
            autoMocker.ClassUnderTest.Expect(partialMock => partialMock.TransformNewlyCompletedGameIntoPlayedGame(null, 0, null, null))
                .IgnoreArguments()
                .Return(expectedPlayedGame);

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedPlayedGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<IApplicationLinker>().AssertWasCalled(mock => mock.LinkApplication(
                EXPECTED_PLAYED_GAME_ID,
                expectedApplicationLinkage1.ApplicationName,
                expectedApplicationLinkage1.EntityId));

            autoMocker.Get<IApplicationLinker>().AssertWasCalled(mock => mock.LinkApplication(
              EXPECTED_PLAYED_GAME_ID,
              expectedApplicationLinkage2.ApplicationName,
              expectedApplicationLinkage2.EntityId));
        }


        [Test]
        public void ItRecordsAGamePlayedEvent()
        {
            var playerRank = new PlayerRank
            {
                GameRank = 1,
                PlayerId = 1
            };
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = new List<PlayerRank>
                { playerRank }
            };
            var transactionSource = TransactionSource.RestApi;
            autoMocker.Get<IPointsCalculator>()
              .Expect(mock => mock.CalculatePoints(null, null))
              .IgnoreArguments()
              .Return(new Dictionary<int, PointsScorecard>
              {
                              {playerRank.PlayerId, new PointsScorecard()}
              });

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, transactionSource, currentUser);

            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayedGame(currentUser, transactionSource));
        }

        [Test]
        public void ItRecalculatesTheNemesisOfEveryPlayerInTheGame()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerThreeId = 3;
            var playerRanks = new List<PlayerRank>
            {
                new PlayerRank
                {
                    PlayerId = playerOneId,
                    GameRank = 1
                },
                new PlayerRank
                {
                    PlayerId = playerTwoId,
                    GameRank = 2
                },
                new PlayerRank
                {
                    PlayerId = playerThreeId,
                    GameRank = 3
                }
            };
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };
            autoMocker.Get<IPointsCalculator>()
                      .Expect(mock => mock.CalculatePoints(null, null))
                      .IgnoreArguments()
                      .Return(new Dictionary<int, PointsScorecard>
                      {
                          {playerOneId, new PointsScorecard()},
                          {playerTwoId, new PointsScorecard()},
                          {playerThreeId, new PointsScorecard()}
                      });

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            foreach (var playerRank in playerRanks)
            {
                autoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(playerRank.PlayerId, currentUser));
            }
        }

        [Test]
        public void ItRecalculatesTheChampionForTheGameButDoesntClearTheExistingChampion()
        {
            var playerRanks = new List<PlayerRank>();
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion(newlyCompletedGame.GameDefinitionId, currentUser, false));
        }

        [Test]
        public void ItChecksSecurityOnThePlayerId()
        {
            var playerRanks = new List<PlayerRank>
            {
                new PlayerRank
                {
                    PlayerId = existingPlayerWithMatchingGamingGroup.Id
                }
            };
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<Player>(existingPlayerWithMatchingGamingGroup.Id))
                .Return(existingPlayerWithMatchingGamingGroup);
            autoMocker.Get<IPointsCalculator>()
                .Expect(mock => mock.CalculatePoints(null, null))
                .IgnoreArguments()
                .Return(new Dictionary<int, PointsScorecard>
            {
                {existingPlayerWithMatchingGamingGroup.Id, new PointsScorecard()}
            });

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<ISecuredEntityValidator>().AssertWasCalled(mock => mock.ValidateAccess<Player>(
                existingPlayerWithMatchingGamingGroup.Id,
                currentUser));
        }

        [Test]
        public void ItChecksSecurityOnTheGameDefinitionId()
        {
            var playerRanks = new List<PlayerRank>();
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks
            };

            autoMocker.ClassUnderTest.CreatePlayedGame(newlyCompletedGame, TransactionSource.WebApplication, currentUser);

            autoMocker.Get<ISecuredEntityValidator>().AssertWasCalled(mock => mock.ValidateAccess<GameDefinition>(
                gameDefinition.Id,
                currentUser));
        }

        [Test]
        public void It_Send_PlayedGameCreatedEvent()
        {
            autoMocker.ClassUnderTest.CreatePlayedGame(CreateValidNewlyCompletedGame(), TransactionSource.WebApplication, currentUser);

            autoMocker.Get<IBusinessLogicEventBus>().AssertWasCalled(mock => mock.SendEvent(Arg<IBusinessLogicEvent>.Matches(m => m.GetType() == typeof(PlayedGameCreatedEvent))));
        }

        [Test]
        public void It_Checks_If_The_Entity_Has_Already_Been_Synced_From_An_External_Source()
        {
            //--arrange
            var validGame = CreateValidNewlyCompletedGame();

            //--act
            autoMocker.ClassUnderTest.CreatePlayedGame(validGame, TransactionSource.WebApplication, currentUser);

            //--assert
            autoMocker.Get<ILinkedPlayedGameValidator>()
                .AssertWasCalled(mock => mock.Validate(validGame));
        }
    }
}
