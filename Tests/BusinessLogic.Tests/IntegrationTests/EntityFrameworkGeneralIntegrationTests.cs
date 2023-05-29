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
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models.Achievements;
using Shouldly;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class EntityFrameworkGeneralIntegrationTests : IntegrationTestIoCBase
    {
        private int _gamingGroupIdWithNemesisPlayers;
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
            _userWithGamingGroup = dataContext.GetQueryable<ApplicationUser>().OrderBy(x => x.Id).First(x => x.CurrentGamingGroupId > 0);

            // TODO: These tests only work if the DataAccessTests have already run -- otherwise the data may not exist
            _gamingGroupIdWithNemesisPlayers = dataContext.GetQueryable<GamingGroup>().First(gg => gg.Name == "this is test gaming group 1").Id;
        }

        [Test]
        public void TheAddOrInsertExtensionMethodSetsTheIdOnNewEntities()
        {
            var dataContext = GetInstance<IDataContext>();

            var gameDefinition = new GameDefinition
            {
                Name = "some testing game definition"
            };

            CleanupFromLastTime(dataContext, gameDefinition, _userWithGamingGroup);

            dataContext.Save(gameDefinition, _userWithGamingGroup);
            dataContext.CommitAllChanges();

            try
            {
                // TODO: update CI build to support simplified syntax for `default`
                gameDefinition.Id.ShouldNotBe(default(int));
            }
            finally
            {
                // TODO: refactor
                CleanupFromLastTime(dataContext, gameDefinition, _userWithGamingGroup);
            }
        }

        private static void CleanupFromLastTime(
        IDataContext dataContext,
        GameDefinition gameDefinition,
        ApplicationUser currentUser)
        {
            var entitiesToDelete = dataContext
                .GetQueryable<GameDefinition>().Where(game => game.Name == gameDefinition.Name).ToList();
            foreach (var entity in entitiesToDelete)
            {
                dataContext.Delete(entity, currentUser);
            }
            dataContext.CommitAllChanges();
        }

        [Test]
        public void TestIncludeMethod()
        {
            var dataContext = GetInstance<IDataContext>();

            var players = dataContext.GetQueryable<Player>()
                            .Where(player => player.Active && player.GamingGroupId == _gamingGroupIdWithNemesisPlayers)
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
        public void It_Wraps_Updates_In_A_Transaction_Implicitly()
        {
            var dataContext = new NemeStatsDataContext(new NemeStatsDbContext(), new SecuredEntityValidatorFactory());
            var firstPlayedGame = dataContext.GetQueryable<PlayedGame>().First();
            var applicationUserWhoCanSave = new ApplicationUser
            {
                CurrentGamingGroupId = firstPlayedGame.GamingGroupId
            };

            firstPlayedGame.Notes = "Integration test note: " + DateTime.UtcNow;
            dataContext.Save(firstPlayedGame, applicationUserWhoCanSave);

            var firstGameDefinition = dataContext.GetQueryable<GameDefinition>().First(x => x.GamingGroupId == applicationUserWhoCanSave.CurrentGamingGroupId);
            var originalGameName = firstGameDefinition.Name;
            int numberOfCharactersToSubstring = originalGameName.Length > 100 ? 100 : originalGameName.Length;
            firstGameDefinition.Name = (Guid.NewGuid() + firstGameDefinition.Name).Substring(0, numberOfCharactersToSubstring);
            dataContext.Save(firstGameDefinition, applicationUserWhoCanSave);

            // reset it back so that later tests can clean up properly
            firstGameDefinition.Name = originalGameName;
            dataContext.Save(firstGameDefinition, applicationUserWhoCanSave);

            dataContext.Dispose();

            #region See The Sql

            /*
            
            Opened connection at 3/24/2017 12:46:07 PM -04:00

            SELECT TOP (1) 
                [c].[Id] AS [Id], 
                [c].[GamingGroupId] AS [GamingGroupId], 
                [c].[GameDefinitionId] AS [GameDefinitionId], 
                [c].[WinnerType] AS [WinnerType], 
                [c].[NumberOfPlayers] AS [NumberOfPlayers], 
                [c].[DatePlayed] AS [DatePlayed], 
                [c].[DateCreated] AS [DateCreated], 
                [c].[DateUpdated] AS [DateUpdated], 
                [c].[CreatedByApplicationUserId] AS [CreatedByApplicationUserId], 
                [c].[Notes] AS [Notes]
                FROM [dbo].[PlayedGame] AS [c]


            -- Executing at 3/24/2017 12:46:07 PM -04:00

            -- Completed in 0 ms with result: SqlDataReader



            Closed connection at 3/24/2017 12:46:07 PM -04:00

            Opened connection at 3/24/2017 12:46:10 PM -04:00

            SELECT TOP (2) 
                [Extent1].[Id] AS [Id], 
                [Extent1].[GamingGroupId] AS [GamingGroupId], 
                [Extent1].[GameDefinitionId] AS [GameDefinitionId], 
                [Extent1].[WinnerType] AS [WinnerType], 
                [Extent1].[NumberOfPlayers] AS [NumberOfPlayers], 
                [Extent1].[DatePlayed] AS [DatePlayed], 
                [Extent1].[DateCreated] AS [DateCreated], 
                [Extent1].[DateUpdated] AS [DateUpdated], 
                [Extent1].[CreatedByApplicationUserId] AS [CreatedByApplicationUserId], 
                [Extent1].[Notes] AS [Notes]
                FROM [dbo].[PlayedGame] AS [Extent1]
                WHERE 4 = [Extent1].[Id]


            -- Executing at 3/24/2017 12:46:10 PM -04:00

            -- Completed in 0 ms with result: SqlDataReader



            Closed connection at 3/24/2017 12:46:10 PM -04:00

            Opened connection at 3/24/2017 12:46:10 PM -04:00

            Started transaction at 3/24/2017 12:46:10 PM -04:00

            UPDATE [dbo].[PlayedGame]
            SET [Notes] = @0
            WHERE ([Id] = @1)

            -- @0: 'Integration test note: 3/24/2017 4:46:09 PM' (Type = String, Size = -1)

            -- @1: '4' (Type = Int32)

            -- Executing at 3/24/2017 12:46:10 PM -04:00

            -- Completed in 5 ms with result: 1



            Committed transaction at 3/24/2017 12:46:10 PM -04:00

            Closed connection at 3/24/2017 12:46:10 PM -04:00

            Opened connection at 3/24/2017 12:46:11 PM -04:00

            SELECT TOP (1) 
                [Extent1].[Id] AS [Id], 
                [Extent1].[GamingGroupId] AS [GamingGroupId], 
                [Extent1].[Name] AS [Name], 
                [Extent1].[Description] AS [Description], 
                [Extent1].[Active] AS [Active], 
                [Extent1].[DateCreated] AS [DateCreated], 
                [Extent1].[ChampionId] AS [ChampionId], 
                [Extent1].[PreviousChampionId] AS [PreviousChampionId], 
                [Extent1].[BoardGameGeekGameDefinitionId] AS [BoardGameGeekGameDefinitionId]
                FROM [dbo].[GameDefinition] AS [Extent1]
                WHERE [Extent1].[GamingGroupId] = @p__linq__0


            -- p__linq__0: '1' (Type = Int32, IsNullable = false)

            -- Executing at 3/24/2017 12:46:11 PM -04:00

            -- Completed in 0 ms with result: SqlDataReader



            Closed connection at 3/24/2017 12:46:11 PM -04:00

            Opened connection at 3/24/2017 12:46:13 PM -04:00

            SELECT TOP (2) 
                [Extent1].[Id] AS [Id], 
                [Extent1].[GamingGroupId] AS [GamingGroupId], 
                [Extent1].[Name] AS [Name], 
                [Extent1].[Description] AS [Description], 
                [Extent1].[Active] AS [Active], 
                [Extent1].[DateCreated] AS [DateCreated], 
                [Extent1].[ChampionId] AS [ChampionId], 
                [Extent1].[PreviousChampionId] AS [PreviousChampionId], 
                [Extent1].[BoardGameGeekGameDefinitionId] AS [BoardGameGeekGameDefinitionId]
                FROM [dbo].[GameDefinition] AS [Extent1]
                WHERE 27458 = [Extent1].[Id]


            -- Executing at 3/24/2017 12:46:13 PM -04:00

            -- Completed in 1 ms with result: SqlDataReader



            Closed connection at 3/24/2017 12:46:13 PM -04:00

            Opened connection at 3/24/2017 12:46:13 PM -04:00

            Started transaction at 3/24/2017 12:46:13 PM -04:00

            UPDATE [dbo].[GameDefinition]
            SET [Name] = @0
            WHERE ([Id] = @1)

            -- @0: '01b16aaa-faf1-4c9a-9798-161bddf9dde' (Type = String, Size = 255)

            -- @1: '27458' (Type = Int32)

            -- Executing at 3/24/2017 12:46:13 PM -04:00

            -- Completed in 1 ms with result: 1



            Committed transaction at 3/24/2017 12:46:13 PM -04:00

            Closed connection at 3/24/2017 12:46:13 PM -04:00


             */

            #endregion
        }
    }
}
