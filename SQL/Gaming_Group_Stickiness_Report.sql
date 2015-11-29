USE NerdScorekeeper;

WITH ActiveAfterOneWeek (GamingGroupId)
AS 
(
  SELECT GamingGroup.[Id]
  FROM [dbo].[GamingGroup]
  WHERE EXISTS (SELECT 1 FROM PlayedGame WHERE PlayedGame.GamingGroupid = GamingGroup.id 
	AND PlayedGame.DateCreated > DATEADD(day, 7, GamingGroup.DateCreated))
),
ActiveAfterOneMonth (GamingGroupId)
AS 
(
  SELECT GamingGroup.[Id]
  FROM [dbo].[GamingGroup]
  WHERE EXISTS (SELECT 1 FROM PlayedGame WHERE PlayedGame.GamingGroupid = GamingGroup.id 
	AND PlayedGame.DateCreated > DATEADD(month, 1, GamingGroup.DateCreated))
),
ActiveAfterThreeMonths (GamingGroupId)
AS 
(
  SELECT GamingGroup.[Id]
  FROM [dbo].[GamingGroup]
  WHERE EXISTS (SELECT 1 FROM PlayedGame WHERE PlayedGame.GamingGroupid = GamingGroup.id 
	AND PlayedGame.DateCreated > DATEADD(month, 3, GamingGroup.DateCreated))
),
CountOfGameDefinitions (NumberOfGameDefinitions, GamingGroupId)
AS 
(
	SELECT COUNT(*) AS NumberOfGameDefinitions, GameDefinition.GamingGroupId FROM GameDefinition GROUP BY GameDefinition.GamingGroupId
),
CountOfPlayers (NumberOfPlayers, GamingGroupId)
AS 
(
	SELECT COUNT(*) AS NumberOfPlayers, Player.GamingGroupId FROM Player GROUP BY Player.GamingGroupId
)
 SELECT GamingGroup.id, GamingGroup.Name, GamingGroup.DateCreated, AspnetUsers.Email, 
 DATEDIFF(day, GamingGroup.DateCreated, GETDATE()) AS GamingGroupAge,
 MAX(PlayedGame.DateCreated) AS DateLastGamePlayed,
 COALESCE(DATEDIFF(day, MAX(PlayedGame.DateCreated), GETDATE()), 0) AS DaysSinceLastGameRecorded,
 COUNT(PlayedGame.Id) AS TotalGamesPlayed,
 COALESCE(CountOfGameDefinitions.NumberOfGameDefinitions, 0) AS TotalGameDefinitionsCreated,
 COALESCE(CountOfPlayers.NumberOfPlayers, 0) AS TotalPlayersCreated,
 COALESCE(DATEDIFF(day, GamingGroup.DateCreated, MAX(PlayedGame.DateCreated)), 0) AS NumberOfDaysGamingGroupWasActive,
 CASE WHEN EXISTS (SELECT 1 FROM ActiveAfterOneWeek WHERE GamingGroupId = GamingGroup.Id) THEN 1 ELSE 0 END AS ActiveAfter1Week,
 CASE WHEN EXISTS (SELECT 1 FROM ActiveAfterOneMonth WHERE GamingGroupId = GamingGroup.Id) THEN 1 ELSE 0 END AS ActiveAfter1Month,
 CASE WHEN EXISTS (SELECT 1 FROM ActiveAfterThreeMonths WHERE GamingGroupId = GamingGroup.Id) THEN 1 ELSE 0 END AS ActiveAfter3Months,
 CASE WHEN (
	(
		--days since last active is less than 30 days + 20% of the time the group was active
		COALESCE(DATEDIFF(day, MAX(PlayedGame.DateCreated), GETDATE()), 0) < 
		(DATEDIFF(day, GamingGroup.DateCreated, GETDATE()) * .2) + 30)
		AND COUNT(PlayedGame.Id) > 0
	) THEN 1 ELSE 0 END AS SeeminglyActive
 FROM GamingGroup 
 LEFT JOIN AspnetUsers ON GamingGroup.OwningUserId = AspNetUsers.Id
 LEFT JOIN PlayedGame ON PlayedGame.GamingGroupid = GamingGroup.Id
 LEFT JOIN CountOfGameDefinitions ON CountOfGameDefinitions.GamingGroupId = GamingGroup.Id
 LEFT JOIN CountOfPlayers ON CountOfPlayers.GamingGroupId = GamingGroup.Id
 GROUP BY GamingGroup.id, GamingGroup.Name, GamingGroup.DateCreated, AspnetUsers.Email,
 CountOfGameDefinitions.NumberOfGameDefinitions, CountOfPlayers.NumberOfPlayers
 ORDER BY ActiveAfter1Week DESC, ActiveAfter1Month DESC, ActiveAfter3Months DESC
