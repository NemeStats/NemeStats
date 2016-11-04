using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
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
        private int _existingPlayerGameResultId1 = 10;
        private int _existingPlayerGameResultId2 = 11;
        private int _existingPlayedGameApplicationLinkage1 = 20;
        private int _existingPlayedGameApplicationLinkage2 = 20;

        private void SetupExpectationsForExistingPlayedGame()
        {
            _existingGamingGroupId = currentUser.CurrentGamingGroupId;

            _existingPlayedGame = new PlayedGame
            {
                Id = 1,
                GamingGroupId = _existingGamingGroupId,
                GameDefinition = new GameDefinition
                {
                    Id = _existingGameDefinitionId
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
                        Id = _existingPlayedGameApplicationLinkage1
                    },
                    new PlayedGameApplicationLinkage
                    {
                        Id = _existingPlayedGameApplicationLinkage2
                    }
                }
            };

            var playedGameQueryable = new List<PlayedGame>
            {
                _existingPlayedGame
            }.AsQueryable();

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(playedGameQueryable);

            autoMocker.ClassUnderTest.Expect(partialMock => partialMock.ValidateAccessToPlayers(
                Arg<List<PlayerRank>>.Is.Anything,
                Arg<int>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Throws_An_EntityDoesNotExistException_If_The_Specified_Played_Game_Id_Does_Not_Match_An_Existing_Game()
        {
            //--arrange
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayedGame>()).Return(new List<PlayedGame>().AsQueryable());

            var playerRanks = new List<PlayerRank>();
            var updatedGame = new UpdatedGame
            {
                GameDefinitionId = gameDefinition.Id,
                PlayerRanks = playerRanks,
                PlayedGameId = -1
            };
            var expectedException = new EntityDoesNotExistException(typeof(PlayedGame), updatedGame.PlayedGameId);

            //--act
            var actualException = Assert.Throws<EntityDoesNotExistException>(
                () => autoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, currentUser));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
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

            autoMocker.ClassUnderTest.Expect(
                    partialMock =>
                        partialMock.ValidateAccessToGameDefinition(Arg<int>.Is.Anything,
                            Arg<ApplicationUser>.Is.Anything))
                .Return(new GameDefinition());

            //--act
            autoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, currentUser);

            //--assert
            autoMocker.ClassUnderTest.AssertWasCalled(partialMock => partialMock.ValidateAccessToGameDefinition(updatedGame.GameDefinitionId, currentUser));
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

            autoMocker.ClassUnderTest.Expect(
                    partialMock =>
                        partialMock.ValidateAccessToGameDefinition(Arg<int>.Is.Anything,
                            Arg<ApplicationUser>.Is.Anything))
                .Return(new GameDefinition());

            //--act
            autoMocker.ClassUnderTest.UpdatePlayedGame(updatedGame, TransactionSource.RestApi, currentUser);

            //--assert
            autoMocker.ClassUnderTest.AssertWasCalled(partialMock => partialMock.ValidateAccessToPlayers(updatedGame.PlayerRanks, _existingGamingGroupId, currentUser));
        }

    }
}
