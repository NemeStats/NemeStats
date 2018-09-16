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
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Points;
using BusinessLogic.Logic.Security;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.CreatePlayedGameComponentTests
{
    [TestFixture]
    public class ExecuteTests
    {
        private RhinoAutoMocker<CreatePlayedGameComponent> _autoMocker;

        private int GAMING_GROUP_ID = 1;
        private GameDefinition _expectedGameDefinition;
        private PlayedGame _expectedPlayedGame;
        private ApplicationUser _currentUser;
        private IDataContext _dataContext;

        [SetUp]
        public void SetUp()
        {
            _currentUser = new ApplicationUser
            {
                CurrentGamingGroupId = GAMING_GROUP_ID
            };

            _expectedGameDefinition = new GameDefinition
            {
                Name = "game definition name",
                GamingGroupId = GAMING_GROUP_ID,
                Id = 9598
            };

            _expectedPlayedGame = new PlayedGame
            {
                Id = 2,
                GameDefinitionId = _expectedGameDefinition.Id,
                NumberOfPlayers = 3,
                GamingGroupId = GAMING_GROUP_ID
            };

            _autoMocker = new RhinoAutoMocker<CreatePlayedGameComponent>();
            _dataContext = MockRepository.GenerateMock<IDataContext>();

            _autoMocker.Get<ISecuredEntityValidator>().Expect(mock => mock.RetrieveAndValidateAccess<GameDefinition>(
                    Arg<int>.Is.Anything,
                    Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedGameDefinition);

            _autoMocker.Get<IPlayedGameSaver>().Expect(mock => mock.TransformNewlyCompletedGameIntoPlayedGame(null, 0, null, null))
                .IgnoreArguments()
                .Return(_expectedPlayedGame);

            _dataContext
                .Expect(s => s.Save(Arg<PlayedGame>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedPlayedGame);
        }

        [Test]
        public void It_Throws_A_NoValidGamingGroupException_If_The_User_Has_No_Gaming_Group_And_There_Is_None_Specified_On_The_Request()
        {
            //--arrange
            var expectedException = new NoValidGamingGroupException(_currentUser.Id);
            _currentUser.CurrentGamingGroupId = null;
            var request = CreateValidNewlyCompletedGame();
            request.GamingGroupId = null;

            //--act
            var actualException =
                Assert.Throws<NoValidGamingGroupException>(
                    () => _autoMocker.ClassUnderTest.Execute(request, _currentUser, _dataContext));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void It_Saves_The_Played_Game()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();

            //--act
            _autoMocker.ClassUnderTest.Execute(newlyCompletedPlayedGame, _currentUser, _dataContext);

            //--assert
            _dataContext.AssertWasCalled(mock => mock.Save(Arg<PlayedGame>.Is.Same(_expectedPlayedGame), Arg<ApplicationUser>.Is.Same(_currentUser)));
        }

        private NewlyCompletedGame CreateValidNewlyCompletedGame()
        {
            List<PlayerRank> playerRanks;

            var playerOneId = 3515;
            var playerTwoId = 15151;
            var playerOneRank = 1;
            var playerTwoRank = 2;
            var newlyCompletedGame = new NewlyCompletedGame
            { GameDefinitionId = _expectedGameDefinition.Id };
            playerRanks = new List<PlayerRank>();
            playerRanks.Add(new PlayerRank
            { PlayerId = playerOneId, GameRank = playerOneRank });
            playerRanks.Add(new PlayerRank
            { PlayerId = playerTwoId, GameRank = playerTwoRank });
            newlyCompletedGame.PlayerRanks = playerRanks;
            _autoMocker.Get<IPointsCalculator>()
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
        public void ItSetsTheGamingGroupIdToThatOfTheUser()
        {
            var newlyCompletedPlayedGame = new NewlyCompletedGame
            {
                GameDefinitionId = _expectedGameDefinition.Id,
                PlayerRanks = new List<PlayerRank>()
            };

            _autoMocker.Get<IPlayedGameSaver>().Expect(logic => logic.MakePlayerGameResults(null, null, null))
                .IgnoreArguments()
                .Return(new List<PlayerGameResult>());

            _autoMocker.ClassUnderTest.Execute(newlyCompletedPlayedGame, _currentUser, _dataContext);

            _dataContext.AssertWasCalled(mock => mock.Save(
                Arg<PlayedGame>.Matches(game => game.GamingGroupId == _currentUser.CurrentGamingGroupId.Value),
                Arg<ApplicationUser>.Is.Same(_currentUser)));
        }

        [Test]
        public void ItCreatesApplicationLinkages()
        {
            //--arrange
            var newlyCompletedPlayedGame = CreateValidNewlyCompletedGame();
            var expectedApplicationLinkage1 = new ApplicationLinkage
            {
                ApplicationName = "app1",
                EntityId = "1"
            };
            newlyCompletedPlayedGame.ApplicationLinkages = new List<ApplicationLinkage>
            {
                expectedApplicationLinkage1
            };

            //--act
            _autoMocker.ClassUnderTest.Execute(newlyCompletedPlayedGame, _currentUser, _dataContext);

            //--assert
            _autoMocker.Get<IPlayedGameSaver>().AssertWasCalled(mock => mock.CreateApplicationLinkages(
                newlyCompletedPlayedGame.ApplicationLinkages,
                _expectedPlayedGame.Id,
                _dataContext));
        }

        [Test]
        public void ItValidatesAccessToPlayers()
        {
            var playerRanks = new List<PlayerRank>();

            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = _expectedGameDefinition.Id,
                PlayerRanks = playerRanks,
                GamingGroupId = 42
            };
            
            _autoMocker.ClassUnderTest.Execute(newlyCompletedGame, _currentUser, _dataContext);

            _autoMocker.Get<IPlayedGameSaver>().AssertWasCalled(mock => mock.ValidateAccessToPlayers(
                newlyCompletedGame.PlayerRanks, 
                newlyCompletedGame.GamingGroupId.Value,
                _currentUser,
                _dataContext));
        }

        [Test]
        public void ItChecksSecurityOnTheGameDefinitionId()
        {
            var playerRanks = new List<PlayerRank>();
            var newlyCompletedGame = new NewlyCompletedGame
            {
                GameDefinitionId = _expectedGameDefinition.Id,
                PlayerRanks = playerRanks
            };

            _autoMocker.ClassUnderTest.Execute(newlyCompletedGame, _currentUser, _dataContext);

            _autoMocker.Get<ISecuredEntityValidator>().AssertWasCalled(mock => mock.RetrieveAndValidateAccess<GameDefinition>(
                _expectedGameDefinition.Id,
                _currentUser));
        }

        [Test]
        public void It_Checks_If_The_Entity_Has_Already_Been_Synced_From_An_External_Source()
        {
            //--arrange
            var validGame = CreateValidNewlyCompletedGame();

            //--act
            _autoMocker.ClassUnderTest.Execute(validGame, _currentUser, _dataContext);

            //--assert
            _autoMocker.Get<ILinkedPlayedGameValidator>()
                .AssertWasCalled(mock => mock.Validate(validGame));
        }
    }
}
