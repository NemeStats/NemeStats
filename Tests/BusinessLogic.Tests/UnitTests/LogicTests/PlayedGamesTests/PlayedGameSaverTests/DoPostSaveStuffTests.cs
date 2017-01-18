using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class DoPostSaveStuffTests : PlayedGameSaverTestBase
    {
        [Test]
        public void ItRecalculatesTheNemesisOfEveryPlayerInTheGame()
        {
            var playerOneId = 1;
            var playerTwoId = 2;
            var playerThreeId = 3;
            var playerGameResults = new List<PlayerGameResult>
            {
                new PlayerGameResult
                {
                    PlayerId = playerOneId,
                    GameRank = 1
                },
                new PlayerGameResult
                {
                    PlayerId = playerTwoId,
                    GameRank = 2
                },
                new PlayerGameResult
                {
                    PlayerId = playerThreeId,
                    GameRank = 3
                }
            };
            int playedGameId = 999;

            AutoMocker.ClassUnderTest.DoPostSaveStuff(TransactionSource.WebApplication, CurrentUser, playedGameId, GameDefinition.Id, playerGameResults, AutoMocker.Get<IDataContext>());

            foreach (var playerGameResult in playerGameResults)
            {
                AutoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(playerGameResult.PlayerId, CurrentUser, AutoMocker.Get<IDataContext>()));
            }
        }

        [Test]
        public void ItRecalculatesTheChampionForTheGameButDoesntClearTheExistingChampion()
        {
            int playedGameId = 44;

            AutoMocker.ClassUnderTest.DoPostSaveStuff(TransactionSource.WebApplication, CurrentUser, playedGameId, GameDefinition.Id, new List<PlayerGameResult>(), AutoMocker.Get<IDataContext>());

            AutoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion(GameDefinition.Id, CurrentUser, AutoMocker.Get<IDataContext>(), false));
        }

    }
}
