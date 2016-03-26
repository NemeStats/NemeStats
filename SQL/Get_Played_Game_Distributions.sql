/** These queries give the distribution of played games by number of players, BGG Weight, and BGG Average Play Time*/
USE [NerdScorekeeper]
GO

--playing time, weight, and number of players
SELECT CASE WHEN COALESCE(BoardGameGeekGameDefinition.[AverageWeight], -1) = 0 THEN -1 ELSE COALESCE(BoardGameGeekGameDefinition.[AverageWeight], -1) END AS AverageWeight
  , COALESCE(BoardGameGeekGameDefinition.[PlayingTime], -1) AS PlayingTime
  , PlayedGame.NumberOfPlayers
  , COUNT(*) AS CNT
  FROM [dbo].[PlayedGame] 
  INNER JOIN GameDefinition ON PlayedGame.GameDefinitionid = GameDefinition.Id
  LEFT JOIN BoardGameGeekGameDefinition ON BoardGameGeekGameDefinition.Id = GameDefinition.BoardGameGeekGameDefinitionId
  GROUP BY CASE WHEN COALESCE(BoardGameGeekGameDefinition.[AverageWeight], -1) = 0 THEN -1 ELSE COALESCE(BoardGameGeekGameDefinition.[AverageWeight], -1) END
  , COALESCE(BoardGameGeekGameDefinition.[PlayingTime], -1)
  , PlayedGame.NumberOfPlayers
  ORDER BY CNT DESC
GO

--weight and number of players
SELECT COALESCE(BoardGameGeekGameDefinition.[AverageWeight], -1) AS AverageWeight
  , PlayedGame.NumberOfPlayers
  , COUNT(*) AS CNT
  FROM [dbo].[PlayedGame] 
  INNER JOIN GameDefinition ON PlayedGame.GameDefinitionid = GameDefinition.Id
  LEFT JOIN BoardGameGeekGameDefinition ON BoardGameGeekGameDefinition.Id = GameDefinition.BoardGameGeekGameDefinitionId
  GROUP BY COALESCE(BoardGameGeekGameDefinition.[AverageWeight], -1)
  , PlayedGame.NumberOfPlayers
  ORDER BY CNT DESC

--playing time and number of players
SELECT COALESCE(BoardGameGeekGameDefinition.[PlayingTime], -1) AS PlayingTime
  , PlayedGame.NumberOfPlayers
  , COUNT(*) AS CNT
  FROM [dbo].[PlayedGame] 
  INNER JOIN GameDefinition ON PlayedGame.GameDefinitionid = GameDefinition.Id
  LEFT JOIN BoardGameGeekGameDefinition ON BoardGameGeekGameDefinition.Id = GameDefinition.BoardGameGeekGameDefinitionId
  GROUP BY COALESCE(BoardGameGeekGameDefinition.[PlayingTime], -1)
  , PlayedGame.NumberOfPlayers
  ORDER BY CNT DESC
GO

--number of players
SELECT PlayedGame.NumberOfPlayers
  , COUNT(*) AS CNT
  FROM [dbo].[PlayedGame] 
  INNER JOIN GameDefinition ON PlayedGame.GameDefinitionid = GameDefinition.Id
  LEFT JOIN BoardGameGeekGameDefinition ON BoardGameGeekGameDefinition.Id = GameDefinition.BoardGameGeekGameDefinitionId
  GROUP BY PlayedGame.NumberOfPlayers
  ORDER BY CNT DESC