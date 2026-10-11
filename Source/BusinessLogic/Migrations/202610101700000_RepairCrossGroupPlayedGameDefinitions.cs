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
namespace BusinessLogic.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed partial class RepairCrossGroupPlayedGameDefinitions : DbMigration
    {
        public override void Up()
        {
            Sql(@"
IF EXISTS
(
    SELECT 1
    FROM dbo.PlayedGame AS playedGame
    INNER JOIN dbo.GameDefinition AS sourceDefinition
        ON sourceDefinition.Id = playedGame.GameDefinitionId
    WHERE playedGame.GamingGroupId <> sourceDefinition.GamingGroupId
)
BEGIN
IF OBJECT_ID(N'dbo.NemeStatsPlayedGameDefinitionRepair', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.NemeStatsPlayedGameDefinitionRepair
    (
        PlayedGameId int NOT NULL PRIMARY KEY,
        PreviousGameDefinitionId int NOT NULL,
        TargetGamingGroupId int NOT NULL,
        RepairedGameDefinitionId int NULL,
        CreatedGameDefinitionId int NULL,
        CreatedGameDefinitionDateCreated datetime NULL,
        RepairStatus nvarchar(32) NOT NULL
    );
END;

CREATE TABLE #DefinitionRepairPlan
(
    SourceGameDefinitionId int NOT NULL,
    TargetGamingGroupId int NOT NULL,
    ExistingCandidateGameDefinitionId int NULL,
    CandidateCount int NOT NULL,
    NameCollisionCount int NOT NULL,
    SourceCollisionCount int NOT NULL DEFAULT 0,
    CreatedGameDefinitionId int NULL,
    PRIMARY KEY (SourceGameDefinitionId, TargetGamingGroupId)
);

;WITH MismatchedDefinitions AS
(
    SELECT DISTINCT
        playedGame.GameDefinitionId AS SourceGameDefinitionId,
        playedGame.GamingGroupId AS TargetGamingGroupId
    FROM dbo.PlayedGame AS playedGame
    INNER JOIN dbo.GameDefinition AS sourceDefinition
        ON sourceDefinition.Id = playedGame.GameDefinitionId
    WHERE playedGame.GamingGroupId <> sourceDefinition.GamingGroupId
)
INSERT INTO #DefinitionRepairPlan
(
    SourceGameDefinitionId,
    TargetGamingGroupId,
    ExistingCandidateGameDefinitionId,
    CandidateCount,
    NameCollisionCount
)
SELECT
    mismatch.SourceGameDefinitionId,
    mismatch.TargetGamingGroupId,
    candidateMatch.CandidateGameDefinitionId,
    candidateMatch.CandidateCount,
    nameMatch.NameCollisionCount
FROM MismatchedDefinitions AS mismatch
INNER JOIN dbo.GameDefinition AS sourceDefinition
    ON sourceDefinition.Id = mismatch.SourceGameDefinitionId
CROSS APPLY
(
    SELECT
        MIN(candidate.Id) AS CandidateGameDefinitionId,
        COUNT(candidate.Id) AS CandidateCount
    FROM dbo.GameDefinition AS candidate
    WHERE candidate.GamingGroupId = mismatch.TargetGamingGroupId
        AND
        (
            (
                sourceDefinition.BoardGameGeekGameDefinitionId IS NOT NULL
                AND candidate.BoardGameGeekGameDefinitionId = sourceDefinition.BoardGameGeekGameDefinitionId
            )
            OR
            (
                sourceDefinition.BoardGameGeekGameDefinitionId IS NULL
                AND candidate.BoardGameGeekGameDefinitionId IS NULL
                AND candidate.Name = sourceDefinition.Name
            )
        )
) AS candidateMatch
CROSS APPLY
(
    SELECT COUNT(*) AS NameCollisionCount
    FROM dbo.GameDefinition AS sameName
    WHERE sameName.GamingGroupId = mismatch.TargetGamingGroupId
        AND sameName.Name = sourceDefinition.Name
) AS nameMatch;

UPDATE repairPlan
SET SourceCollisionCount =
(
    SELECT COUNT(*)
    FROM #DefinitionRepairPlan AS siblingPlan
    INNER JOIN dbo.GameDefinition AS siblingDefinition
        ON siblingDefinition.Id = siblingPlan.SourceGameDefinitionId
    WHERE siblingPlan.TargetGamingGroupId = repairPlan.TargetGamingGroupId
        AND siblingPlan.SourceGameDefinitionId <> repairPlan.SourceGameDefinitionId
        AND siblingPlan.CandidateCount = 0
        AND siblingPlan.NameCollisionCount = 0
        AND siblingDefinition.Name = sourceDefinition.Name
)
FROM #DefinitionRepairPlan AS repairPlan
INNER JOIN dbo.GameDefinition AS sourceDefinition
    ON sourceDefinition.Id = repairPlan.SourceGameDefinitionId;

INSERT INTO dbo.GameDefinition
(
    Name,
    Description,
    GamingGroupId,
    Active,
    DateCreated,
    BoardGameGeekGameDefinitionId
)
SELECT
    sourceDefinition.Name,
    sourceDefinition.Description,
    repairPlan.TargetGamingGroupId,
    sourceDefinition.Active,
    SYSUTCDATETIME(),
    sourceDefinition.BoardGameGeekGameDefinitionId
FROM #DefinitionRepairPlan AS repairPlan
INNER JOIN dbo.GameDefinition AS sourceDefinition
    ON sourceDefinition.Id = repairPlan.SourceGameDefinitionId
WHERE repairPlan.CandidateCount = 0
    AND repairPlan.NameCollisionCount = 0
    AND repairPlan.SourceCollisionCount = 0;

UPDATE repairPlan
SET CreatedGameDefinitionId = targetDefinition.Id
FROM #DefinitionRepairPlan AS repairPlan
INNER JOIN dbo.GameDefinition AS sourceDefinition
    ON sourceDefinition.Id = repairPlan.SourceGameDefinitionId
INNER JOIN dbo.GameDefinition AS targetDefinition
    ON targetDefinition.GamingGroupId = repairPlan.TargetGamingGroupId
        AND targetDefinition.Name = sourceDefinition.Name
WHERE repairPlan.CandidateCount = 0
    AND repairPlan.NameCollisionCount = 0
    AND repairPlan.SourceCollisionCount = 0;

;WITH RepairResults AS
(
    SELECT
        playedGame.Id AS PlayedGameId,
        playedGame.GameDefinitionId AS PreviousGameDefinitionId,
        playedGame.GamingGroupId AS TargetGamingGroupId,
        CASE
            WHEN repairPlan.CandidateCount = 1 THEN repairPlan.ExistingCandidateGameDefinitionId
            WHEN repairPlan.CandidateCount = 0
                AND repairPlan.NameCollisionCount = 0
                AND repairPlan.SourceCollisionCount = 0
                THEN repairPlan.CreatedGameDefinitionId
            ELSE NULL
        END AS RepairedGameDefinitionId,
        repairPlan.CreatedGameDefinitionId,
        createdDefinition.DateCreated AS CreatedGameDefinitionDateCreated,
        CASE
            WHEN repairPlan.CandidateCount = 1 THEN N'MatchedExisting'
            WHEN repairPlan.CandidateCount > 1 THEN N'AmbiguousEquivalent'
            WHEN repairPlan.NameCollisionCount > 0 THEN N'NameConflict'
            WHEN repairPlan.SourceCollisionCount > 0 THEN N'AmbiguousSourceName'
            WHEN repairPlan.CreatedGameDefinitionId IS NOT NULL THEN N'CreatedAndRepaired'
            ELSE N'NoEquivalentDefinition'
        END AS RepairStatus
    FROM dbo.PlayedGame AS playedGame
    INNER JOIN dbo.GameDefinition AS sourceDefinition
        ON sourceDefinition.Id = playedGame.GameDefinitionId
    INNER JOIN #DefinitionRepairPlan AS repairPlan
        ON repairPlan.SourceGameDefinitionId = sourceDefinition.Id
            AND repairPlan.TargetGamingGroupId = playedGame.GamingGroupId
    LEFT JOIN dbo.GameDefinition AS createdDefinition
        ON createdDefinition.Id = repairPlan.CreatedGameDefinitionId
    WHERE playedGame.GamingGroupId <> sourceDefinition.GamingGroupId
)
INSERT INTO dbo.NemeStatsPlayedGameDefinitionRepair
(
    PlayedGameId,
    PreviousGameDefinitionId,
    TargetGamingGroupId,
    RepairedGameDefinitionId,
    CreatedGameDefinitionId,
    CreatedGameDefinitionDateCreated,
    RepairStatus
)
SELECT
    PlayedGameId,
    PreviousGameDefinitionId,
    TargetGamingGroupId,
    RepairedGameDefinitionId,
    CreatedGameDefinitionId,
    CreatedGameDefinitionDateCreated,
    RepairStatus
FROM RepairResults
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.NemeStatsPlayedGameDefinitionRepair AS existingRepair
    WHERE existingRepair.PlayedGameId = RepairResults.PlayedGameId
);

UPDATE playedGame
SET GameDefinitionId = repair.RepairedGameDefinitionId
FROM dbo.PlayedGame AS playedGame
INNER JOIN dbo.NemeStatsPlayedGameDefinitionRepair AS repair
    ON repair.PlayedGameId = playedGame.Id
WHERE repair.RepairedGameDefinitionId IS NOT NULL
    AND playedGame.GameDefinitionId = repair.PreviousGameDefinitionId;
END;
");
        }

        public override void Down()
        {
            Sql(@"
IF OBJECT_ID(N'dbo.NemeStatsPlayedGameDefinitionRepair', N'U') IS NULL
    RETURN;

IF EXISTS
(
    SELECT 1
    FROM dbo.NemeStatsPlayedGameDefinitionRepair AS repair
    INNER JOIN dbo.PlayedGame AS playedGame
        ON playedGame.Id = repair.PlayedGameId
    WHERE repair.RepairedGameDefinitionId IS NOT NULL
        AND playedGame.GameDefinitionId <> repair.RepairedGameDefinitionId
)
    THROW 51000, 'Cannot revert played-game definition repair after a repaired row has changed.', 1;

IF EXISTS
(
    SELECT 1
    FROM
    (
        SELECT DISTINCT CreatedGameDefinitionId
        FROM dbo.NemeStatsPlayedGameDefinitionRepair
        WHERE CreatedGameDefinitionId IS NOT NULL
    ) AS createdDefinition
    INNER JOIN dbo.PlayedGame AS playedGame
        ON playedGame.GameDefinitionId = createdDefinition.CreatedGameDefinitionId
    LEFT JOIN dbo.NemeStatsPlayedGameDefinitionRepair AS repair
        ON repair.PlayedGameId = playedGame.Id
            AND repair.RepairedGameDefinitionId = createdDefinition.CreatedGameDefinitionId
    WHERE repair.PlayedGameId IS NULL
)
    THROW 51001, 'Cannot revert a created game definition that is used by new played games.', 1;

IF EXISTS
(
    SELECT 1
    FROM
    (
        SELECT DISTINCT
            CreatedGameDefinitionId,
            CreatedGameDefinitionDateCreated,
            PreviousGameDefinitionId,
            TargetGamingGroupId
        FROM dbo.NemeStatsPlayedGameDefinitionRepair
        WHERE CreatedGameDefinitionId IS NOT NULL
    ) AS createdDefinition
    INNER JOIN dbo.GameDefinition AS newDefinition
        ON newDefinition.Id = createdDefinition.CreatedGameDefinitionId
    INNER JOIN dbo.GameDefinition AS sourceDefinition
        ON sourceDefinition.Id = createdDefinition.PreviousGameDefinitionId
    WHERE newDefinition.Name <> sourceDefinition.Name
        OR newDefinition.GamingGroupId <> createdDefinition.TargetGamingGroupId
        OR newDefinition.Active <> sourceDefinition.Active
        OR newDefinition.DateCreated <> createdDefinition.CreatedGameDefinitionDateCreated
        OR newDefinition.ChampionId IS NOT NULL
        OR newDefinition.PreviousChampionId IS NOT NULL
        OR newDefinition.Description <> sourceDefinition.Description
        OR (newDefinition.Description IS NULL AND sourceDefinition.Description IS NOT NULL)
        OR (newDefinition.Description IS NOT NULL AND sourceDefinition.Description IS NULL)
        OR newDefinition.BoardGameGeekGameDefinitionId <> sourceDefinition.BoardGameGeekGameDefinitionId
        OR (newDefinition.BoardGameGeekGameDefinitionId IS NULL AND sourceDefinition.BoardGameGeekGameDefinitionId IS NOT NULL)
        OR (newDefinition.BoardGameGeekGameDefinitionId IS NOT NULL AND sourceDefinition.BoardGameGeekGameDefinitionId IS NULL)
)
    THROW 51002, 'Cannot revert a created game definition that has been edited.', 1;

UPDATE playedGame
SET GameDefinitionId = repair.PreviousGameDefinitionId
FROM dbo.PlayedGame AS playedGame
INNER JOIN dbo.NemeStatsPlayedGameDefinitionRepair AS repair
    ON repair.PlayedGameId = playedGame.Id
WHERE repair.RepairedGameDefinitionId IS NOT NULL;

DELETE newDefinition
FROM dbo.GameDefinition AS newDefinition
INNER JOIN
(
    SELECT DISTINCT CreatedGameDefinitionId
    FROM dbo.NemeStatsPlayedGameDefinitionRepair
    WHERE CreatedGameDefinitionId IS NOT NULL
) AS createdDefinition
    ON createdDefinition.CreatedGameDefinitionId = newDefinition.Id
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.PlayedGame AS playedGame
    WHERE playedGame.GameDefinitionId = newDefinition.Id
);

DROP TABLE dbo.NemeStatsPlayedGameDefinitionRepair;
");
        }
    }
}
