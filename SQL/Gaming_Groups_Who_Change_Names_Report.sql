USE [NerdScorekeeper]
GO


SELECT COUNT(*) AS CNT,

	  CASE WHEN NAME LIKE '%''s Gaming Group' THEN 0 ELSE 1 END AS ChangedName
  FROM [dbo].[GamingGroup]
  WHERE Id IN (SELECT GamingGroupId FROM PlayedGame GROUP BY GamingGroupID HAVING COUNT(*) > 2)
  AND Id IN (SELECT GamingGroupId FROM PlayedGame GROUP BY GamingGroupID HAVING DATEDIFF(DAY, MIN(DatePlayed), MAX(DatePlayed)) >= 14)
  GROUP BY CASE WHEN NAME LIKE '%''s Gaming Group' THEN 0 ELSE 1 END
GO


