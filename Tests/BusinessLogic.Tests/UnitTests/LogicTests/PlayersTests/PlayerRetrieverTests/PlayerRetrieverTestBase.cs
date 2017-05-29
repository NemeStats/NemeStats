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
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Utility;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerRetrieverTests
{
    public class PlayerRetrieverTestBase
    {
        internal RhinoAutoMocker<PlayerRetriever> autoMocker;
        internal IQueryable<Player> playerQueryable;
        internal int gamingGroupId = 558585;
        internal int totalPoints = 265;
        internal List<PlayerGameResult> playerGameResultsForFirstPlayer;
        internal List<Champion> playerChampionshipsForFirstPlayer;
        internal GameDefinition gameDefinition;

        [SetUp]
        public virtual void BaseSetUp()
        {
            autoMocker = new RhinoAutoMocker<PlayerRetriever>();
            autoMocker.PartialMockTheClassUnderTest();
            const int CHAMPION_ID = 55;

            gameDefinition = new GameDefinition
            {
                Name = "game name",
                ChampionId = CHAMPION_ID
            };

            var playerId = 1;
            var champion = new Champion
            {
                Id = CHAMPION_ID,
                GameDefinition = gameDefinition,
                PlayerId = playerId
            };

            playerGameResultsForFirstPlayer = new List<PlayerGameResult>()
            {
                new PlayerGameResult
                {
                    GameRank = 2,
                    NemeStatsPointsAwarded = 10,
                    PlayedGame = new PlayedGame
                    {
                        DatePlayed = new BasicDateRangeFilter().FromDate
                    }
                },
                new PlayerGameResult
                {
                    GameRank = 1,
                    NemeStatsPointsAwarded = 20,
                    PlayedGame = new PlayedGame
                    {
                        DatePlayed = new BasicDateRangeFilter().ToDate
                    }
                }};

            playerChampionshipsForFirstPlayer = new List<Champion>
            {
                champion
            };
            List<Player> players = new List<Player>()
            {
                new Player
                {
                    GamingGroupId = gamingGroupId,
                    Name = "2",
                    PlayerGameResults = new List<PlayerGameResult>(), ChampionedGames = new List<Champion>(),
                    Nemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    },
                    PreviousNemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    }
                },
                new Player(){ GamingGroupId = gamingGroupId, Name = "3", PlayerGameResults = new List<PlayerGameResult>(), ChampionedGames = new List<Champion>(),
                    Nemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    },
                    PreviousNemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    } },
                new Player(){ GamingGroupId = -1, Name = "not in gaming group", PlayerGameResults = new List<PlayerGameResult>(), ChampionedGames = new List<Champion>(),
                    Nemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    },
                    PreviousNemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    } },
                new Player()
                {
                    Id = playerId,
                    GamingGroupId = gamingGroupId, 
                    Name = "1", 
                    PlayerGameResults = playerGameResultsForFirstPlayer,
                    ChampionedGames = new List<Champion>
                    {
                        champion
                    },
                    Nemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    },
                    PreviousNemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    }
                },
                new Player()
                {
                    //player that will be last because she's inactive
                    GamingGroupId = gamingGroupId, Name = "0", PlayerGameResults = new List<PlayerGameResult>(), ChampionedGames = new List<Champion>(), Active = false ,
                    Nemesis = new Nemesis()
                    {
                        NemesisPlayerId = 2,
                        NemesisPlayer = new Player() { Id = 93995 }
                    },
                    PreviousNemesis = new Nemesis
                    {
                        NemesisPlayer = new Player()
                    }
                }
            };
            players[3].NemesisId = 1;
            players[3].Nemesis = new Nemesis()
            {
                NemesisPlayerId = 2,
                NemesisPlayer = new Player() { Id = 93995 }
            };
            playerQueryable = players.AsQueryable<Player>();

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                .Return(playerQueryable);

            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(
                new List<GameDefinition>
                { gameDefinition }.AsQueryable());

            var playerAchievement =
                new PlayerAchievement
                {
                    DateCreated = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    PlayerId = playerId,
                    AchievementLevel = AchievementLevel.Bronze,
                    AchievementId = AchievementId.BusyBee
                };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<PlayerAchievement>())
                .Return(new List<PlayerAchievement> { playerAchievement }.AsQueryable());
        }
    }
}
