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
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System.Linq;
using System.Data.Entity;
using BusinessLogic.Models.Achievements;
using Shouldly;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class EntityFrameworkGeneralIntegrationTests : IntegrationTestIoCBase
    {
        private int _playerIdWithNoPlayedGames;
        private int _somePlayedGameId;
        private ApplicationUser _userWithGamingGroup;

        [OneTimeSetUp]
        public void LocalOneTimeSetUp()
        {
            var dataContext = GetInstance<IDataContext>();
            _playerIdWithNoPlayedGames =
                dataContext.GetQueryable<Player>()
                .Where(x => !x.PlayerGameResults.Any())
                .Select(x => x.Id)
                .First();

            _somePlayedGameId = dataContext.GetQueryable<PlayedGame>().Select(x => x.Id).First();

            _userWithGamingGroup = dataContext.GetQueryable<ApplicationUser>().First(x => x.CurrentGamingGroupId > 0);
        }

        [Test]
        public void TheAddOrInsertExtensionMethodSetsTheIdOnNewEntities()
        {
            var dataContext = GetInstance<IDataContext>();

            var gamingGroup = new GamingGroup
            {
                Name = "new gaming group without an ID yet",
                OwningUserId = _userWithGamingGroup.Id
            };

            dataContext.Save(gamingGroup, _userWithGamingGroup);
            dataContext.CommitAllChanges();

            Cleanup(dataContext, gamingGroup, _userWithGamingGroup);

            gamingGroup.Id.ShouldNotBe(default(int));
        }

        [Test]
        public void TestIncludeMethod()
        {
            var dataContext = GetInstance<IDataContext>();

            var players = dataContext.GetQueryable<Player>()
                            .Where(player => player.Active && player.GamingGroupId == 1)
                            .Include(player => player.Nemesis)
                            .Include(player => player.Nemesis.NemesisPlayer)

                            .OrderBy(player => player.Name)
                            .ToList();

            var playersWithNemesisid = players.Where(player => player.NemesisId != null).ToList();

            playersWithNemesisid.Count.ShouldBeGreaterThan(0);
        }

        [Test]
        public void ItReturnsNullWhenDoingFirstOrDefaultOnAsSingleEntityThatDoesntExist()
        {
            var dataContext = GetInstance<IDataContext>();
            var result = dataContext.GetQueryable<PlayedGame>()
                .Where(player => player.Id == _playerIdWithNoPlayedGames)
                .Select(x => x.PlayerGameResults)
                .FirstOrDefault();

            result.ShouldBeNull();

            /*
            SELECT
            [Project2].[Id] AS[Id], 
            [Project2].[C1]
                AS[C1], 
            [Project2].[Id1]
                AS[Id1], 
            [Project2].[PlayedGameId]
                AS[PlayedGameId], 
            [Project2].[PlayerId]
                AS[PlayerId], 
            [Project2].[GameRank]
                AS[GameRank], 
            [Project2].[NemeStatsPointsAwarded]
                AS[NemeStatsPointsAwarded], 
            [Project2].[GameWeightBonusPoints]
                AS[GameWeightBonusPoints], 
            [Project2].[GameDurationBonusPoints]
                AS[GameDurationBonusPoints], 
            [Project2].[PointsScored]
                AS[PointsScored], 
            [Project2].[TotalPoints]
                AS[TotalPoints]
        FROM(SELECT
            [Limit1].[Id] AS[Id],
            [Extent2].[Id] AS[Id1],
            [Extent2].[PlayedGameId] AS[PlayedGameId],
            [Extent2].[PlayerId] AS[PlayerId],
            [Extent2].[GameRank] AS[GameRank],
            [Extent2].[NemeStatsPointsAwarded] AS[NemeStatsPointsAwarded],
            [Extent2].[GameWeightBonusPoints] AS[GameWeightBonusPoints],
            [Extent2].[GameDurationBonusPoints] AS[GameDurationBonusPoints],
            [Extent2].[PointsScored] AS[PointsScored],
            [Extent2].[TotalPoints] AS[TotalPoints],
            CASE WHEN ([Extent2].[Id] IS NULL) THEN CAST(NULL AS int) ELSE 1 END AS[C1]
                FROM(SELECT TOP (1)
                    [Extent1].[Id]
                AS[Id]
        FROM[dbo].[PlayedGame]
                AS[Extent1]
        WHERE[Extent1].[Id] = @p__linq__0 ) AS[Limit1]
        LEFT OUTER JOIN[dbo].[PlayerGameResult]
                AS[Extent2] ON[Limit1].[Id] = [Extent2].[PlayedGameId]
            )  AS[Project2]
            ORDER BY[Project2].[Id]
                ASC, [Project2].[C1]
                ASC
                    
        */
        }

        [Test]
        public void FirstOrDefaultReturnsNullAndDoesntBlowUpWhenAccessingARelatedEntity()
        {
            var dataContext = GetInstance<IDataContext>();

            var result = dataContext.GetQueryable<PlayedGame>()
                .Where(player => player.Id == _playerIdWithNoPlayedGames)
                .Select(x => x.PlayerGameResults.FirstOrDefault().Player)
                .FirstOrDefault();

            result.ShouldBeNull();

            /*
            SELECT 
                [Limit2].[Id] AS [Id], 
                [Limit2].[GamingGroupId] AS [GamingGroupId], 
                [Limit2].[Name] AS [Name], 
                [Limit2].[ApplicationUserId] AS [ApplicationUserId], 
                [Limit2].[Active] AS [Active], 
                [Limit2].[NemesisId] AS [NemesisId], 
                [Limit2].[PreviousNemesisId] AS [PreviousNemesisId], 
                [Limit2].[DateCreated] AS [DateCreated]
                FROM ( SELECT TOP (1) 
                    [Extent3].[Id] AS [Id], 
                    [Extent3].[GamingGroupId] AS [GamingGroupId], 
                    [Extent3].[Name] AS [Name], 
                    [Extent3].[ApplicationUserId] AS [ApplicationUserId], 
                    [Extent3].[Active] AS [Active], 
                    [Extent3].[NemesisId] AS [NemesisId], 
                    [Extent3].[PreviousNemesisId] AS [PreviousNemesisId], 
                    [Extent3].[DateCreated] AS [DateCreated]
                    FROM   (SELECT 
                        (SELECT TOP (1) 
                            [Extent2].[PlayerId] AS [PlayerId]
                            FROM [dbo].[PlayerGameResult] AS [Extent2]
                            WHERE [Extent1].[Id] = [Extent2].[PlayedGameId]) AS [C1]
                        FROM [dbo].[PlayedGame] AS [Extent1]
                        WHERE [Extent1].[Id] = @p__linq__0 ) AS [Project2]
                    LEFT OUTER JOIN [dbo].[Player] AS [Extent3] ON [Project2].[C1] = [Extent3].[Id]
                )  AS [Limit2]
                * */
        }

        [Test]
        public void ItDoesntCareAboutOtherwiseWeirdDistantRelationshipPullingWhenThereAreNoResultsToProjectInTheFirstPlace()
        {
            var dataContext = GetInstance<IDataContext>();
            int invalidPlayerId = -1;
            var result = dataContext.GetQueryable<PlayedGame>()
                .Where(player => player.Id == invalidPlayerId)
                .Select(x => new
                {
                    SomeDistantThing = x.PlayerGameResults.FirstOrDefault().Player.ChampionedGames.FirstOrDefault(),
                    x.PlayerGameResults.FirstOrDefault().Player.Id
                }).ToList();
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
            /*
            SELECT
            CASE WHEN(EXISTS(SELECT
                1 AS[C1]
                FROM[dbo].[PlayerGameResult] AS[Extent1]
                WHERE([Extent1].[PlayerId] = @p__linq__0) AND(1 = [Extent1].[GameRank])
            )) THEN cast(1 as bit) ELSE cast(0 as bit) END AS[C1]
             FROM(SELECT 1 AS X) AS[SingleRowTable1]
             */
        }

        [Test]
        public void ItDoesntBlowUpWhenAccessingAnOptionalRelationshipFieldWithoutCheckingForNull()
        {
            var dataContext = GetInstance<IDataContext>();
            var result = dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.Id == _somePlayedGameId)
                .Select(x => new AchievementRelatedPlayedGameSummary
                {
                    //--only pull records where the Player had rank -42 (i.e. none of the PlayerGameResults)
                    WinningPlayerName = x.PlayerGameResults.FirstOrDefault(y => y.GameRank == -42).Player.Name,
                    WinningPlayerId = x.PlayerGameResults.FirstOrDefault(y => y.GameRank == -42).Player.Id
                }).ToList();
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].WinningPlayerName.ShouldBeNull();
            result[0].WinningPlayerId.ShouldBeNull();

            /*
            SELECT 
                [Project2].[Id] AS [Id], 
                [Project2].[Name] AS [Name], 
                [Limit2].[PlayerId] AS [PlayerId]
                FROM   (SELECT 
                    [Filter1].[Id] AS [Id], 
                    [Extent3].[Name] AS [Name]
                    FROM    (SELECT [Extent1].[Id] AS [Id]
                        FROM [dbo].[PlayedGame] AS [Extent1]
                        WHERE [Extent1].[Id] = @p__linq__0 ) AS [Filter1]
                    OUTER APPLY  (SELECT TOP (1) 
                        [Extent2].[PlayerId] AS [PlayerId]
                        FROM [dbo].[PlayerGameResult] AS [Extent2]
                        WHERE ([Filter1].[Id] = [Extent2].[PlayedGameId]) AND (-42 = [Extent2].[GameRank]) ) AS [Limit1]
                    LEFT OUTER JOIN [dbo].[Player] AS [Extent3] ON [Limit1].[PlayerId] = [Extent3].[Id] ) AS [Project2]
                OUTER APPLY  (SELECT TOP (1) 
                    [Extent4].[PlayerId] AS [PlayerId]
                    FROM [dbo].[PlayerGameResult] AS [Extent4]
                    WHERE ([Project2].[Id] = [Extent4].[PlayedGameId]) AND (-42 = [Extent4].[GameRank]) ) AS [Limit2]
                * */
        }

        [Test]
        public void ItBlowsUpWhenAccessingAnOptionalRelationshipFieldWithoutCheckingForNullUsingPureLinq()
        {
            var playedGameQueryable = new List<PlayedGame>
            {
                new PlayedGame
                {
                    Id = _somePlayedGameId,
                    PlayerGameResults = new List<PlayerGameResult>()
                }
            }.AsQueryable();

            Assert.Throws<NullReferenceException>(() => playedGameQueryable
                .Where(playedGame => playedGame.Id == _somePlayedGameId)
                .Select(x => new AchievementRelatedPlayedGameSummary
                {
                    //--only pull records where the Player had rank -42 (i.e. none of the PlayerGameResults)
                    WinningPlayerName = x.PlayerGameResults.FirstOrDefault(y => y.GameRank == -42).Player.Name,
                    WinningPlayerId = x.PlayerGameResults.FirstOrDefault(y => y.GameRank == -42).Player.Id
                }).ToList());
        }


        [Test]
        public void GrabbingTwoSeparateFieldsInADistantRelationIsntTerrible()
        {
            var dataContext = GetInstance<IDataContext>();
            var result = dataContext.GetQueryable<PlayedGame>()
                .Where(player => player.Id == _playerIdWithNoPlayedGames)
                .Select(x => new
                    {
                        Name = x.PlayerGameResults.FirstOrDefault() != null ? x.PlayerGameResults.FirstOrDefault().Player.Name : null,
                        Id = x.PlayerGameResults.FirstOrDefault() != null ? x.PlayerGameResults.FirstOrDefault().Player.Id : 0
                })
                .FirstOrDefault();

            result.ShouldBeNull();

            /*
                SELECT 
                [Limit5].[Id] AS [Id], 
                [Limit5].[C1] AS [C1], 
                [Limit5].[C2] AS [C2]
                FROM ( SELECT TOP (1) 
                    [Project11].[Id] AS [Id], 
                    CASE WHEN ([Project11].[C1] IS NOT NULL) THEN [Project11].[Name] END AS [C1], 
                    CASE WHEN ([Project11].[C2] IS NOT NULL) THEN [Project11].[C3] ELSE 0 END AS [C2]
                    FROM ( SELECT 
                        [Project9].[Id] AS [Id], 
                        [Project9].[Name] AS [Name], 
                        [Project9].[C1] AS [C1], 
                        [Project9].[C2] AS [C2], 
                        (SELECT TOP (1) 
                            [Extent6].[PlayerId] AS [PlayerId]
                            FROM [dbo].[PlayerGameResult] AS [Extent6]
                            WHERE [Project9].[Id] = [Extent6].[PlayedGameId]) AS [C3]
                        FROM ( SELECT 
                            [Project8].[Id] AS [Id], 
                            [Project8].[Name] AS [Name], 
                            [Project8].[C1] AS [C1], 
                            [Project8].[C2] AS [C2]
                            FROM ( SELECT 
                                [Project6].[Id] AS [Id], 
                                [Project6].[Name] AS [Name], 
                                [Project6].[C1] AS [C1], 
                                (SELECT TOP (1) 
                                    [Extent5].[Id] AS [Id]
                                    FROM [dbo].[PlayerGameResult] AS [Extent5]
                                    WHERE [Project6].[Id] = [Extent5].[PlayedGameId]) AS [C2]
                                FROM ( SELECT 
                                    [Project5].[Id] AS [Id], 
                                    [Extent4].[Name] AS [Name], 
                                    [Project5].[C1] AS [C1]
                                    FROM   (SELECT 
                                        [Project3].[Id] AS [Id], 
                                        [Project3].[C1] AS [C1], 
                                        (SELECT TOP (1) 
                                            [Extent3].[PlayerId] AS [PlayerId]
                                            FROM [dbo].[PlayerGameResult] AS [Extent3]
                                            WHERE [Project3].[Id] = [Extent3].[PlayedGameId]) AS [C2]
                                        FROM ( SELECT 
                                            [Project2].[Id] AS [Id], 
                                            [Project2].[C1] AS [C1]
                                            FROM ( SELECT 
                                                [Extent1].[Id] AS [Id], 
                                                (SELECT TOP (1) 
                                                    [Extent2].[Id] AS [Id]
                                                    FROM [dbo].[PlayerGameResult] AS [Extent2]
                                                    WHERE [Extent1].[Id] = [Extent2].[PlayedGameId]) AS [C1]
                                                FROM [dbo].[PlayedGame] AS [Extent1]
                                                WHERE [Extent1].[Id] = @p__linq__0
                                            )  AS [Project2]
                                        )  AS [Project3] ) AS [Project5]
                                    LEFT OUTER JOIN [dbo].[Player] AS [Extent4] ON [Project5].[C2] = [Extent4].[Id]
                                )  AS [Project6]
                            )  AS [Project8]
                        )  AS [Project9]
                    )  AS [Project11]
                )  AS [Limit5]
                * */
        }


        [Test]
        public void It_Wraps_Everything_In_A_Transaction_Automatically()
        {
            var dataContext = GetInstance<IDataContext>();
            var firstPlayedGame = dataContext.GetQueryable<PlayedGame>().First();

            firstPlayedGame.Notes = "Integration test note: " + DateTime.UtcNow;

            //var firstGameDefinition = dataContext.GetQueryable<>()
        }


        private static void Cleanup(
        IDataContext dataContext, 
        GamingGroup gamingGroup, 
        ApplicationUser currentUser)
        {
            var gamingGroupToDelete = dataContext
                .GetQueryable<GamingGroup>().FirstOrDefault(game => game.Name == gamingGroup.Name);
            if (gamingGroupToDelete != null)
            {
                dataContext.Delete(gamingGroupToDelete, currentUser);
                dataContext.CommitAllChanges();
            }
        }
    }
}
