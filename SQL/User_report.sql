USE [NerdScorekeeper]
GO

SELECT Users.[Id]
      ,[Email]
      ,[EmailConfirmed]
      ,[UserName]
      ,[DateCreated]
	  ,COUNT(*) AS NumberOfGamingGroups
  FROM [dbo].[AspNetUsers] Users 
  LEFT JOIN [dbo].[UserGamingGroup] UGG1 ON Users.Id = UGG1.ApplicationUserId
  GROUP BY Users.[Id]
      ,[Email]
      ,[EmailConfirmed]
      ,[UserName]
      ,[DateCreated]
  ORDER BY DateCreated DESC
GO


