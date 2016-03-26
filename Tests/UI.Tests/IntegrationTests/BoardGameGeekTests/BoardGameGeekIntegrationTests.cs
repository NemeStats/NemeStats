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
using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Service;
using BusinessLogic.DataAccess;
using NUnit.Framework;
using BusinessLogic.Logic.OneTimeJobs;
using Rhino.Mocks;
using RollbarSharp;

namespace UI.Tests.IntegrationTests.BoardGameGeekTests
{
    [TestFixture, Category("Integration")]
    public class BoardGameGeekIntegrationTests
    {
        [Test, Ignore("Integration test.")]
        public void ItGetsResultsForExistingGameNames()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
                {
                    //gets a distinct list of all game names
                    //var gameNames2 = dataContext.GetQueryable<GameDefinition>()
                    //                           .GroupBy(game => game.Name)
                    //                           .Select(game => game.FirstOrDefault())
                    //                           .ToList();

                    var gameNames = new List<string>
                    {
                        "7 Wonders",
                        "Citadels",
                        "Cosmic encounter ",
                        "Coup",
                        "Dead of winter",
                        "Dominion",
                        "Five Tribes",
                        "Samurai Sword",
                        "Suburbia",
                        "Agricola",
                        "Arkham Horror",
                        "Bang!",
                        "Carcassonne",
                        "Castle Ravenloft",
                        "Race For The Galaxy",
                        "Settlers of Catan",
                        "Shadows of Brimstone",
                        "Sushi Go!",
                        "Twilight Imperium",
                        "Apples to Apples Kids",
                        "At the Gates of Loyang",
                        "Avalon",
                        "Boggle",
                        "Bohnanza",
                        "Carson City",
                        "Caverna",
                        "Chess",
                        "Cinque Terre",
                        "Clue",
                        "Coin Age",
                        "Cranium",
                        "Cranium Zigity",
                        "Cribbage",
                        "Cthulhu Gloom",
                        "Dominoes",
                        "Dungeon Fighter",
                        "Dungeon Petz",
                        "Dungeon Roll",
                        "Dutch Blitz",
                        "Eight Minute Empire",
                        "Famiglia",
                        "Fearsome Floors",
                        "Forbidden Island",
                        "Formula D",
                        "Freedom: The Underground Railroad",
                        "Galaxy Trucker",
                        "Game of thrones boardgame",
                        "Guillotine",
                        "Hive",
                        "Jaipur",
                        "Kaosball",
                        "Kemet",
                        "King of Tokyo",
                        "Legend of Drizzt",
                        "Legends of Andor",
                        "Lewis & Clark",
                        "Lords of Waterdeep",
                        "Love Letter",
                        "Machi Koro",
                        "Mage Knight Board Game",
                        "Mage Knight Dungeons",
                        "Magic the Gathering",
                        "Magic: The Gathering",
                        "Mascarade",
                        "Merchants & Marauders",
                        "Nations",
                        "Nuts",
                        "Once Upon a Time",
                        "Pairs (Calamity Variant)",
                        "Pandemic",
                        "Pathfinder ACG",
                        "Pirate Fluxx",
                        "Puerto Rico",
                        "Quantum",
                        "Quiddler",
                        "Quoridor",
                        "Race  for the galaxy",
                        "Rampage",
                        "Risk: The Lord of the Rings Trilogy Edition",
                        "Risk: The Walking Dead",
                        "RoboRally",
                        "Rummikub",
                        "Saboteur",
                        "Saboteur 2",
                        "Samurai Swords",
                        "SanGuoSha: Legends of the Three Kingdoms",
                        "Scattergories",
                        "Scotland yard",
                        "Scrabble",
                        "Settlers of America: Trails to Rails",
                        "Settlers of the Stone Age",
                        "Seven Wonders",
                        "Small World",
                        "Spartacus",
                        "Splendor",
                        "Star Realms",
                        "Star Trek Battle Force",
                        "Star Trek: Fleet Captains",
                        "Star Wars: X-Wing Miniatures Game",
                        "Survive!",
                        "Taboo",
                        "Tales of the Arabian Nights",
                        "The Little Prince",
                        "The Princes of Florence",
                        "The Resistance",
                        "Thunderstone Advance: Towers of Ruin",
                        "Ticket to Ride",
                        "Ticket to Ride Europe",
                        "Tiny Epic Kingdoms",
                        "Troyes",
                        "tzolkin",
                        "Warhammer 40K",
                        "Wits & Wagers",
                        "Wrath of Ashardalon",
                        "Zombicide"
                    };

                    var rollbarMock = MockRepository.GenerateMock<IRollbarClient>();
                    var bggSearcher = new BoardGameGeekClient(new ApiDownloaderService(), rollbarMock);

                    foreach (string gameName in gameNames)
                    {
                        var results = bggSearcher.SearchBoardGames(gameName, true);

                        if (results.Count() != 1)
                        {
                            Console.WriteLine(gameName + " has "+ results.Count() + " results.");
                        }
                    }
                }
            }
        }

        [Test, Ignore("Integration test")]
        public void ItUpdatesExistingGameDefinitions()
        {
            BoardGameGeekDataLinker dataLinker = new BoardGameGeekDataLinker();
            dataLinker.CleanUpExistingRecords();
        }
    }
}
