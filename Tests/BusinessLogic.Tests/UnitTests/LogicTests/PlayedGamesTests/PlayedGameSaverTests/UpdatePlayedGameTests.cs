using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Events;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Events.Interfaces;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class UpdatePlayedGameTests : PlayedGameSaverTestBase
    {
        private PlayedGame _existingPlayedGame;
        private int _existingPlayedGameId = 1;
        private int _existingGamingGroupId;
        private int _existingGameDefinitionId = 2;
        private int _existingBoardGameGeekGameDefinitionId = 3;
        private int _existingPlayerGameResultId1 = 10;
        private int _existingPlayerGameResultId2 = 11;
        private int _existingPlayedGameApplicationLinkageId1 = 20;
        private int _existingPlayedGameApplicationLinkageId2 = 20;
        private List<PlayerGameResult> _expectedNewPlayerGameResults;
        private PlayedGame _expectedTransformedPlayedGame;
        private PlayedGame _expectedSavedPlayedGame;

        private void SetupExpectationsForExistingPlayedGame()
        {
            _existingGamingGroupId = CurrentUser.CurrentGamingGroupId;

            _existingPlayedGame = new PlayedGame
            {
                Id = 1,
                GamingGroupId = _existingGamingGroupId,
                GameDefinitionId = _existingGameDefinitionId,
                GameDefinition = new GameDefinition
                {
                    BoardGameGeekGameDefinitionId = _existingBoardGameGeekGameDefinitionId
                },
                PlayerGameResults = new List<PlayerGameResult>
                {
                    new PlayerGameResult
                    {
                        Id = _existingPlayerGameResultId1
                    },
                    new PlayerGameResult
                    {
                        Id = _existingPlayerGameResultId2
                    }
                },
                ApplicationLinkages = new List<PlayedGameApplicationLinkage>
                {
                    new PlayedGameApplicationLinkage
                    {
                        Id = _existingPlayedGameApplicationLinkageId1
                    },
                    new PlayedGameApplicationLinkage
                    {
                        Id = _existingPlayedGameApplicationLinkageId2
                    }
                }
            };

            var playedGameQueryable = new List<PlayedGame>
            {
                _existingPlayedGame
            }.AsQueryable();

            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGameQueryable);

            AutoMocker.Get<ISecuredEntityValidator>().Expect(mock => mock.RetrieveAndValidateAccess<GamingGroup>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(new GamingGroup());
            AutoMocker.Get<ISecuredEntityValidator>().Expect(mock => mock.RetrieveAndValidateAccess<PlayedGame>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(new PlayedGame());
            AutoMocker.Get<ISecuredEntityValidator>().Expect(mock => mock.RetrieveAndValidateAccess<GameDefinition>(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(new GameDefinition());

            AutoMocker.ClassUnderTest.Expect(partialMock => partialMock.ValidateAccessToPlayers(
                Arg<List<PlayerRank>>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything, 
                Arg<IDataContext>.Is.Anything));

            _expectedNewPlayerGameResults = new List<PlayerGameResult>
            {
                new PlayerGameResult { PlayerId = 10 },
                new PlayerGameResult { PlayerId = 11 },
                new PlayerGameResult { PlayerId = 12 }
            };

            AutoMocker.ClassUnderTest.Expect(
                    partialMock =>
                            partialMock.MakePlayerGameResults(Arg<SaveableGameBase>.Is.Anything, Arg<int>.Is.Anything, Arg<IDataContext>.Is.Anything))
                .Return(_expectedNewPlayerGameResults);

            _expectedTransformedPlayedGame = new PlayedGame
            {
                Id = _existingPlayedGameId,
                GameDefinitionId = _existingGameDefinitionId
            };
            AutoMocker.ClassUnderTest.Expect(
                    partialMock =>
                        partialMock.TransformNewlyCompletedGameIntoPlayedGame(Arg<SaveableGameBase>.Is.Anything, Arg<int>.Is.Anything,
                            Arg<string>.Is.Anything, Arg<List<PlayerGameResult>>.Is.Anything))
                .Return(_expectedTransformedPlayedGame);

            _expectedSavedPlayedGame = new PlayedGame();
            AutoMocker.Get<IDataContext>()
                .Expect(mock => mock.Save(Arg<PlayedGame>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedSavedPlayedGame);
        }

        [Test]
        public void It_Throws_An_EntityDoesNotExistException_If_The_Specified_Played_Game_Id_Does_Not_Match_An_Existing_Game()
        {
            //--arrange
            AutoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(new List<PlayedGame>().AsQueryable());

            var playerRanks = new List<PlayerRank>();
            var updatedGame = new UpdatedGame
            {
                GameDefinitionId = GameDefinition.Id,
                PlayerRanks = playerRanks,
                PlayedGameId = -1
            };
            var expectedException = new EntityDoesNotExistException(typeof(PlayedGame), updatedGame.PlayedGameId);

            //--act
            var actualException = Assert.Throws<EntityDoesNotExistException>(
                () => AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void It_Validates_Access_To_The_Played_Game()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            int differentGameDefinitionId = _existingGameDefinitionId + 1;

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = differentGameDefinitionId
            };

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser);

            //--assert
            AutoMocker.Get<ISecuredEntityValidator>()
                .AssertWasCalled(mock => mock.RetrieveAndValidateAccess<PlayedGame>(_existingPlayedGame.Id, CurrentUser));
        }

        [Test]
        public void It_Validates_Access_To_The_Gaming_Group()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            int differentGameDefinitionId = _existingGameDefinitionId + 1;

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = differentGameDefinitionId,
                GamingGroupId = 42
            };

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser);

            //--assert
            AutoMocker.Get<ISecuredEntityValidator>().AssertWasCalled(mock => mock.RetrieveAndValidateAccess<GamingGroup>(updatedGame.GamingGroupId.Value, CurrentUser));
        }

        [Test]
        public void It_Validates_Access_To_The_New_Game_Definition()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            int differentGameDefinitionId = _existingGameDefinitionId + 1;

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = differentGameDefinitionId
            };

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser);

            //--assert
            AutoMocker.Get<ISecuredEntityValidator>().AssertWasCalled(mock => mock.RetrieveAndValidateAccess<GameDefinition>(updatedGame.GameDefinitionId, CurrentUser));
        }

        public void It_Validates_Access_To_The_Players_In_The_Game()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            int differentGameDefinitionId = _existingGameDefinitionId + 1;

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = differentGameDefinitionId
            };

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser);

            //--assert
            AutoMocker.ClassUnderTest.AssertWasCalled(partialMock 
                => partialMock.ValidateAccessToPlayers(updatedGame.PlayerRanks, _existingGamingGroupId, CurrentUser, AutoMocker.Get<IDataContext>()));
        }

        [Test]
        public void It_Clears_Out_Existing_Player_Game_Results()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            int differentGameDefinitionId = _existingGameDefinitionId + 1;

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = differentGameDefinitionId
            };

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser);

            //--assert
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<PlayerGameResult>(_existingPlayerGameResultId1, CurrentUser));
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<PlayerGameResult>(_existingPlayerGameResultId2, CurrentUser));
        }

        [Test]
        public void It_Clears_Out_Existing_Application_Linkages()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            int differentGameDefinitionId = _existingGameDefinitionId + 1;

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = differentGameDefinitionId
            };

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser);

            //--assert
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<PlayedGameApplicationLinkage>(_existingPlayedGameApplicationLinkageId1, CurrentUser));
            AutoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.DeleteById<PlayedGameApplicationLinkage>(_existingPlayedGameApplicationLinkageId2, CurrentUser));
        }

        [Test]
        public void It_Reconstructs_The_Played_Game_Then_Updates_It()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            int differentGameDefinitionId = _existingGameDefinitionId + 1;

            var playerRank1 = new PlayerRank
            {
                PlayerId = 100,
                GameRank = 1
            };
            var playerRank2 = new PlayerRank
            {
                PlayerId = 101,
                GameRank = 1
            };
            var playerRank3 = new PlayerRank
            {
                PlayerId = 100,
                GameRank = 1
            };

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = differentGameDefinitionId,
                PlayerRanks = new List<PlayerRank>
                {
                    playerRank1,
                    playerRank2,
                    playerRank3
                },
                DatePlayed = DateTime.UtcNow
            };

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser);

            //--assert
            AutoMocker.ClassUnderTest.AssertWasCalled(
                    partialMock =>
                            partialMock.MakePlayerGameResults(updatedGame, _existingBoardGameGeekGameDefinitionId, AutoMocker.Get<IDataContext>()));

            AutoMocker.ClassUnderTest.AssertWasCalled(
                partialMock =>
                    partialMock.TransformNewlyCompletedGameIntoPlayedGame(updatedGame, _existingGamingGroupId,
                        CurrentUser.Id, _expectedNewPlayerGameResults));

            var arguments =
                AutoMocker.Get<IDataContext>()
                    .GetArgumentsForCallsMadeOn(
                        x => x.Save(Arg<PlayedGame>.Is.Anything, Arg<ApplicationUser>.Is.Anything));

            arguments.ShouldNotBeNull();
            arguments.Count.ShouldBe(1);
            var actualPlayedGame = arguments[0][0] as PlayedGame;
            actualPlayedGame.ShouldNotBeNull();
            actualPlayedGame.DateUpdated.Date.ShouldBe(DateTime.UtcNow.Date);
            actualPlayedGame.Id.ShouldBe(updatedGame.PlayedGameId);
        }

        [Test]
        public void It_Creates_The_Played_Game_Application_Linkages()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = _existingGameDefinitionId,
                ApplicationLinkages = new List<ApplicationLinkage>
                {
                    new ApplicationLinkage()
                }
            };
            var transactionSource = TransactionSource.RestApi;

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, transactionSource, CurrentUser);

            //--assert
            AutoMocker.ClassUnderTest.AssertWasCalled(mock => mock.CreateApplicationLinkages(updatedGame.ApplicationLinkages, _existingPlayedGameId, AutoMocker.Get<IDataContext>()));
        }

        [Test]
        public void It_Does_All_Of_The_Post_Save_Stuff()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = _existingGameDefinitionId
            };
            var transactionSource = TransactionSource.RestApi;

            //--act
            AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, transactionSource, CurrentUser);

            //--assert
            var args = AutoMocker.Get<IBusinessLogicEventSender>().GetArgumentsForCallsMadeOn(
                mock => mock.SendEvent(
                Arg<IBusinessLogicEvent>.Is.Anything));

            var businessLogicEvent = args.AssertFirstCallIsType<IBusinessLogicEvent>();

            businessLogicEvent.ShouldNotBeNull();
            businessLogicEvent.ShouldBeOfType<PlayedGameCreatedEvent>();
            var playedGameCreatedEvent = (PlayedGameCreatedEvent)businessLogicEvent;

            playedGameCreatedEvent.TransactionSource.ShouldBe(transactionSource);

            playedGameCreatedEvent.TriggerEntityId.ShouldBe(_existingPlayedGameId);

            playedGameCreatedEvent.GameDefinitionId.ShouldBe(_existingGameDefinitionId);

            var expectedListOfPlayerIds = _expectedNewPlayerGameResults.Select(x => x.PlayerId).ToList();
            playedGameCreatedEvent.ParticipatingPlayerIds.ShouldBe(expectedListOfPlayerIds);
        }

        [Test]
        public void It_Returns_The_Saved_Played_Game()
        {
            //--arrange
            SetupExpectationsForExistingPlayedGame();

            var updatedGame = new UpdatedGame
            {
                PlayedGameId = _existingPlayedGameId,
                GameDefinitionId = _existingGameDefinitionId
            };

            //--act
            var result = AutoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, CurrentUser);

            //--assert
            result.ShouldBeSameAs(_expectedSavedPlayedGame);
        }
    }
}
