/* Get number of user registrations and gaming groups created by week */
SELECT AspNetUsers.DateCreated AS UserRegisteredDate
	  ,AspNetUsers.Id AS UserId
	  ,AspNetUsers.UserName AS UserName
	  ,GamingGroup.Id AS GamingGroupId
	  ,CASE WHEN EXISTS (SELECT 1 FROM GamingGroup WHERE GamingGroup.OwningUserId = AspNetUsers.Id) THEN 1 ELSE 0 END AS GamingGroupCreated
	  ,CASE WHEN EXISTS (SELECT 1 FROM PlayedGame WHERE PlayedGame.GamingGroupId = GamingGroup.Id) THEN 1 ELSE 0 END AS PlayedGameCreated
	  ,CASE WHEN EXISTS (SELECT 1 FROM PlayedGame 
							WHERE PlayedGame.GamingGroupId = GamingGroup.Id 
								AND PlayedGame.DatePlayed > DATEADD(day, 7, GamingGroup.DateCreated)) THEN 1 ELSE 0 END AS ActiveAfterOneWeek
	  ,CASE WHEN EXISTS (SELECT 1 FROM PlayedGame 
							WHERE PlayedGame.GamingGroupId = GamingGroup.Id 
								AND PlayedGame.DatePlayed > DATEADD(month, 1, GamingGroup.DateCreated)) THEN 1 ELSE 0 END AS ActiveAfterOneMonth
	  ,CASE WHEN EXISTS (SELECT 1 FROM PlayedGame 
							WHERE PlayedGame.GamingGroupId = GamingGroup.Id 
								AND PlayedGame.DatePlayed > DATEADD(month, 3, GamingGroup.DateCreated)) THEN 1 ELSE 0 END AS ActiveAfterThreeMonths
  FROM [NerdScorekeeper].[dbo].[AspNetUsers]
  LEFT JOIN GamingGroup on GamingGroup.OwningUserId = AspNetUsers.Id
  LEFT JOIN PlayedGame ON PlayedGame.GamingGroupId = GamingGroup.Id
  ORDER BY UserRegisteredDate;
  