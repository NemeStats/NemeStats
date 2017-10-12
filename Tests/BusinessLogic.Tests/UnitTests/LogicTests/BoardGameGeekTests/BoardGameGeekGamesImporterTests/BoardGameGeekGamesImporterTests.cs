using System;
using System.Collections.Generic;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekTests.BoardGameGeekGamesImporterTests
{
    [TestFixture]
    public class BoardGameGeekGamesImporterTests
    {
        private RhinoAutoMocker<BoardGameGeekGamesImporter> _autoMocker;
        private ApplicationUser _currentUser;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<BoardGameGeekGamesImporter>();
            _currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = 1
            };
        }

        [Test]
        public void It_Throws_An_ArgumentNullException_If_ApplicationUser_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => _autoMocker.ClassUnderTest.ImportBoardGameGeekGames(null));
        }

        [Test]
        public void It_Throws_A_UserHasNoGamingGroupException_If_ApplicationUser_Current_Gaming_Group_Is_Null()
        {
            _currentUser.Id = "some user id";
            var expectedException = new UserHasNoGamingGroupException(_currentUser.Id);

            var actualException = Assert.Throws<UserHasNoGamingGroupException>(() => _autoMocker.ClassUnderTest.ImportBoardGameGeekGames(_currentUser));
            actualException.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void Return_Null_If_BoardGameUserDefinition_Is_Not_Set()
        {
            _autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(Arg<ApplicationUser>.Is.Anything))
                .Return(null);

            var result = _autoMocker.ClassUnderTest.ImportBoardGameGeekGames(_currentUser);

            Assert.IsNull(result);
        }

        [Test]
        public void Return_Null_If_The_BGG_User_Has_No_Games()
        {
            _autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(Arg<ApplicationUser>.Is.Anything))
               .Return(new UserInformation() {BoardGameGeekUser = new BoardGameGeekUserInformation()});
            _autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetUserGames(Arg<string>.Is.Anything))
               .Return(new List<GameDetails>());

            var result = _autoMocker.ClassUnderTest.ImportBoardGameGeekGames(_currentUser);

            Assert.IsNull(result);
        }

        [Test]
        public void Return_0_If_The_BGG_Games_Are_Already_On_GamingGroup()
        {
            var bggGames = new List<GameDetails>() { new GameDetails() { GameId = 1 }, new GameDetails() { GameId = 2 } };
            var ggGames = new List<GameDefinitionName> { new GameDefinitionName() { BoardGameGeekGameDefinitionId = 1 }, new GameDefinitionName() { BoardGameGeekGameDefinitionId = 2 }, new GameDefinitionName() { BoardGameGeekGameDefinitionId = 3 } };

            _autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(Arg<ApplicationUser>.Is.Anything))
              .Return(new UserInformation() { BoardGameGeekUser = new BoardGameGeekUserInformation() });
            _autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetUserGames(Arg<string>.Is.Anything))
               .Return(bggGames);
            _autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitionNames(Arg<int>.Is.Anything, Arg<string>.Is.Anything))
               .Return(ggGames);

            var result = _autoMocker.ClassUnderTest.ImportBoardGameGeekGames(_currentUser);

            Assert.AreEqual(0, result);
        }

        [Test]
        public void Return_Number_Of_Games_Imported_If_There_Are_BGG_Games_Not_Existing_On_GamingGroup()
        {
            var bggGames = new List<GameDetails>() { new GameDetails() { GameId = 1 }, new GameDetails() { GameId = 2 } };
            var ggGames = new List<GameDefinitionName> { new GameDefinitionName() { BoardGameGeekGameDefinitionId = 3 } };

            _autoMocker.Get<IUserRetriever>().Expect(mock => mock.RetrieveUserInformation(Arg<ApplicationUser>.Is.Anything))
              .Return(new UserInformation() { BoardGameGeekUser = new BoardGameGeekUserInformation() });
            _autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetUserGames(Arg<string>.Is.Anything))
               .Return(bggGames);
            _autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitionNames(Arg<int>.Is.Anything, Arg<string>.Is.Anything))
               .Return(ggGames);
            _autoMocker.Get<ICreateGameDefinitionComponent>()
                .Expect(mock => mock.Execute(Arg<CreateGameDefinitionRequest>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(new GameDefinition());

            var result = _autoMocker.ClassUnderTest.ImportBoardGameGeekGames(_currentUser);

            Assert.AreEqual(2, result);
        }
    }
}


