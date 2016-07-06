using System;
using System.Collections.Generic;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekTests.BoardGameGeekGamesImporterTests
{
    [TestFixture]
    public class BoardGameGeekGamesImporterTests
    {
        private RhinoAutoMocker<BoardGameGeekGamesImporter> autoMocker;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<BoardGameGeekGamesImporter>();
            currentUser = new ApplicationUser();
        }

        [Test]
        public void ThrowException_If_ApplicationUser_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => autoMocker.ClassUnderTest.ImportBoardGameGeekGames(null));
        }

        [Test]
        public void Return_Null_If_BoardGameUserDefinition_Is_Not_Set()
        {
            autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(Arg<ApplicationUser>.Is.Anything))
                .Return(null);

            var result = autoMocker.ClassUnderTest.ImportBoardGameGeekGames(currentUser);

            Assert.IsNull(result);
        }

        [Test]
        public void Return_Null_If_The_BGG_User_Has_No_Games()
        {
            autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(Arg<ApplicationUser>.Is.Anything))
               .Return(new UserInformation() {BoardGameGeekUser = new BoardGameGeekUserInformation()});
            autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetUserGames(Arg<string>.Is.Anything))
               .Return(new List<GameDetails>());

            var result = autoMocker.ClassUnderTest.ImportBoardGameGeekGames(currentUser);

            Assert.IsNull(result);
        }

        [Test]
        public void Return_0_If_The_BGG_Games_Are_Already_On_GamingGroup()
        {
            var bggGames = new List<GameDetails>() { new GameDetails() { GameId = 1 }, new GameDetails() { GameId = 2 } };
            var ggGames = new List<GameDefinitionName> { new GameDefinitionName() { BoardGameGeekGameDefinitionId = 1 }, new GameDefinitionName() { BoardGameGeekGameDefinitionId = 2 }, new GameDefinitionName() { BoardGameGeekGameDefinitionId = 3 } };

            autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(Arg<ApplicationUser>.Is.Anything))
              .Return(new UserInformation() { BoardGameGeekUser = new BoardGameGeekUserInformation() });
            autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetUserGames(Arg<string>.Is.Anything))
               .Return(bggGames);
            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitionNames(Arg<int>.Is.Anything, Arg<string>.Is.Anything))
               .Return(ggGames);

            var result = autoMocker.ClassUnderTest.ImportBoardGameGeekGames(currentUser);

            Assert.AreEqual(0, result);
        }

        [Test]
        public void Return_Number_Of_Games_Imported_If_There_Are_BGG_Games_Not_Existing_On_GamingGroup()
        {
            var bggGames = new List<GameDetails>() { new GameDetails() { GameId = 1 }, new GameDetails() { GameId = 2 } };
            var ggGames = new List<GameDefinitionName> { new GameDefinitionName() { BoardGameGeekGameDefinitionId = 3 } };

            autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(Arg<ApplicationUser>.Is.Anything))
              .Return(new UserInformation() { BoardGameGeekUser = new BoardGameGeekUserInformation() });
            autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetUserGames(Arg<string>.Is.Anything))
               .Return(bggGames);
            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitionNames(Arg<int>.Is.Anything, Arg<string>.Is.Anything))
               .Return(ggGames);

            var result = autoMocker.ClassUnderTest.ImportBoardGameGeekGames(currentUser);

            Assert.AreEqual(2, result);
        }
    }
}


