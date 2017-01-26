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

using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.ChamionRepositoryTests
{
    [TestFixture]
    public class GetChampionTests : IntegrationTestBase
    {
        private IGameDefinitionRetriever _gameDefinitionRetriever;
        private GameDefinition _gameDefinition;
        private GameDefinition _championlessGameDefinition;
        private int _championPlayerIdForGameDefinition;
        private int _otherChampionPlayerIdForGameDefinition;


        [OneTimeSetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();
            _gameDefinitionRetriever = GetInstance<GameDefinitionRetriever>();

            _gameDefinition = _gameDefinitionRetriever.GetGameDefinitionDetails(testGameDefinitionWithOtherGamingGroupId.Id,
                0);
            _championlessGameDefinition = _gameDefinitionRetriever.GetGameDefinitionDetails(gameDefinitionWithNoChampion.Id, 0);

            // Player ID 1 has a winning percentage high enough to be considered the champion
            _championPlayerIdForGameDefinition = testPlayer7WithOtherGamingGroupId.Id;

            // Player ID 9 has a higher winning percentage than player 7, but is not active
            _otherChampionPlayerIdForGameDefinition = testPlayer9UndefeatedWith5Games.Id;
        }

        [Test]
        public void ItGetsThePlayerWithTheHighestWinPercentage()
        {
            // Player 7 won 75% of the GameDefinition's total games played
            Assert.That(_gameDefinition.Champion.PlayerId, Is.EqualTo(_championPlayerIdForGameDefinition));
        }

        [Test]
        public void AChampionMustBeActive()
        {
            // Player 8 won 100% of the GameDefinition's total games played, but is inactive
            Assert.That(_otherChampionPlayerIdForGameDefinition, Is.Not.EqualTo(_gameDefinition.Champion.PlayerId));
        }

        [Test]
        public void ItReturnsANullChampionIfThereIsntAChampion()
        {
            // The game definition has recorded games, but no player has played three games
            Assert.That(_championlessGameDefinition.Champion, Is.InstanceOf<NullChampion>());
        }

        [Test]
        public void ItReturnsANullChampionIfThereIsATie()
        {
            // missing integration test but leaving this as documentation of the intent
        }

        [Test]
        public void ItSetsTheWinPercentageForTheChampion()
        {
            // The champion won 83% of all games played
            Assert.That(_gameDefinition.Champion.WinPercentage, Is.EqualTo(83));
        }

        [Test]
        public void ItSetsTheNumberOfGamesForTheChampion()
        {
            // The champion played 6 games
            Assert.That(_gameDefinition.Champion.NumberOfGames, Is.EqualTo(6));
        }

        [Test]
        public void ItSetsTheNumberOfWinsForTheChampion()
        {
            // The champion won 5 games
            Assert.That(_gameDefinition.Champion.NumberOfWins, Is.EqualTo(5));
        }
    }
}
