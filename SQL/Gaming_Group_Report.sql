USE [NerdScorekeeper]
GO

WITH Users_CTE (GamingGroupId, NumberOfUsers)
AS 
(
  SELECT GamingGroup.[Id]
	  ,COUNT(UGG.Id) AS CountOfUsers
  FROM [dbo].[GamingGroup] 
  LEFT JOIN [dbo].[UserGamingGroup] UGG ON GamingGroup.Id = UGG.GamingGroupId
  GROUP BY GamingGroup.[Id]
),
 PlayedGames_CTE (GamingGroupId, NumberOfPlayedGames)
AS 
(
  SELECT GamingGroup.[Id]
	  ,COUNT(PlayedGame.Id) AS CountOfPlayedGames
  FROM [dbo].[GamingGroup] 
  LEFT JOIN [dbo].PlayedGame ON GamingGroup.Id = PlayedGame.GamingGroupId
  GROUP BY GamingGroup.[Id]
),
 Players_CTE (GamingGroupId, NumberOfPlayers)
AS 
(
  SELECT GamingGroup.[Id]
	  ,COUNT(Player.Id) AS CountOfPlayers
  FROM [dbo].[GamingGroup] 
  LEFT JOIN [dbo].Player ON GamingGroup.Id = Player.GamingGroupId
  GROUP BY GamingGroup.[Id]
),
 LinkedPlayers_CTE (GamingGroupId, NumberOfLinkedPlayers)
AS 
(
  SELECT GamingGroup.[Id]
	  ,COUNT(Player.Id) AS CountOfLinkedPlayers
  FROM [dbo].[GamingGroup] 
  LEFT JOIN [dbo].Player ON GamingGroup.Id = Player.GamingGroupId
  WHERE Player.ApplicationUserId IS NOT  NULL
  GROUP BY GamingGroup.[Id]
),
 Invitations_CTE (GamingGroupId, NumberOfInvitations)
AS 
(
  SELECT GamingGroup.[Id]
	  ,COUNT(GamingGroupInvitation.Id) AS NumberOfInvitations
  FROM [dbo].[GamingGroup] 
  LEFT JOIN [dbo].GamingGroupInvitation ON GamingGroup.Id = GamingGroupInvitation.GamingGroupId
  GROUP BY GamingGroup.[Id]
),
 ConsumedInvitations_CTE (GamingGroupId, NumberOfConsumedInvitations)
AS 
(
  SELECT GamingGroup.[Id]
	  ,COUNT(GamingGroupInvitation.Id) AS NumberOfInvitations
  FROM [dbo].[GamingGroup] 
  LEFT JOIN [dbo].GamingGroupInvitation ON GamingGroup.Id = GamingGroupInvitation.GamingGroupId
  WHERE GamingGroupInvitation.DateRegistered IS NOT NULL
  GROUP BY GamingGroup.[Id]
)
SELECT GamingGroup.[Id]
      ,[Name]
      ,GamingGroup.[DateCreated]
	  ,PlayedGames_CTE.NumberOfPlayedGames
	  ,Players_CTE.NumberOfPlayers
	  ,COALESCE(LinkedPlayers_CTE.NumberOfLinkedPlayers, 0) AS NumberOfLinkedPlayers
	  ,Users_CTE.NumberOfUsers
	  ,Invitations_CTE.NumberOfInvitations
	  ,COALESCE(ConsumedInvitations_CTE.NumberOfConsumedInvitations, 0) AS NumberOfConsumedInvitations
  FROM [dbo].[GamingGroup] 
  LEFT JOIN Users_CTE ON Users_CTE.GamingGroupId = GamingGroup.Id
  LEFT JOIN PlayedGames_CTE ON PlayedGames_CTE.GamingGroupId = GamingGroup.Id
  LEFT JOIN Players_CTE ON Players_CTE.GamingGroupId = GamingGroup.Id
  LEFT JOIN LinkedPlayers_CTE ON LinkedPlayers_CTE.GamingGroupId = GamingGroup.Id
  LEFT JOIN Invitations_CTE ON Invitations_CTE.GamingGroupId = GamingGroup.Id
  LEFT JOIN ConsumedInvitations_CTE ON ConsumedInvitations_CTE.GamingGroupId = GamingGroup.Id
  ORDER BY NumberOfPlayedGames DESC
GO



