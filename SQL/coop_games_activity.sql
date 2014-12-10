USE [NerdScorekeeper]
GO

SELECT DISTINCT GameRank, gameDefinition.Name, Playedgame.Id
  FROM [dbo].[PlayerGameResult] INNER JOIN PlayedGame on PlayerGameResult.PlayedGameId = PlayedGame.Id
  INNER JOIN GameDefinition ON GameDefinition.Id = PlayedGame.gameDefinitionId
  WHERE gameDefinition.Name LIKE 'Dead Of%' OR GameDefinition.Name LIKE 'Pand%'
GO


