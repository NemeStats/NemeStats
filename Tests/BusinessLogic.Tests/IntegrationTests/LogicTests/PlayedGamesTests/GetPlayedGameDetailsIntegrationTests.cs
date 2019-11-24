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
using System;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGamesTests
{
    [TestFixture]
    public class GetPlayedGameDetailsIntegrationTests : IntegrationTestBase
    {
        private PlayedGameRetriever _playedGameRetriever;

        [SetUp]
        public void SetUp()
        {
            _playedGameRetriever = GetInstance<PlayedGameRetriever>();
        }
        private PlayedGame GetTestSubjectPlayedGame()
        {
            return _playedGameRetriever.GetPlayedGameDetails(testPlayedGames[0].Id);
        }

        [Test]
        public void ItRetrievesThePlayedGame()
        {
            var playedGame = GetTestSubjectPlayedGame();
            Assert.NotNull(playedGame);
        }

        [Test]
        public void ItFetchesTheGameResults()
        {
            var playedGame = GetTestSubjectPlayedGame();
            Assert.GreaterOrEqual(testPlayedGames[0].PlayerGameResults.Count, playedGame.PlayerGameResults.Count);
        }

        [Test]
        public void ItFetchesTheGameDefinition()
        {
            var playedGame = GetTestSubjectPlayedGame();
            Assert.NotNull(playedGame.GameDefinition);
        }

        [Test]
        public void ItFetchesThePlayers()
        {
            var playedGame = GetTestSubjectPlayedGame();
            Assert.NotNull(playedGame.PlayerGameResults[0].Player);
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfTheIdIsInvalid()
        {
            var invalidId = -1;
            var expectedException = new EntityDoesNotExistException<PlayedGame>(invalidId);

            Exception actualException = Assert.Throws<EntityDoesNotExistException<PlayedGame>>(() => _playedGameRetriever.GetPlayedGameDetails(invalidId));

            Assert.That(expectedException.Message, Is.EqualTo(actualException.Message));
        }
    }
}
