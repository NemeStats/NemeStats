;WITH Player_CTE (PlayerId, GamesWon, GamesLost, TotalGames, GameDefinitionId, GameDefinitionName)
AS
(
	SELECT PlayerId
	, SUM(CASE WHEN GameRank = 1 THEN 1 ELSE 0 END) AS GamesWon
	, SUM(CASE WHEN GameRank <> 1 THEN 1 ELSE 0 END) AS GamesLost
	, COUNT(*) AS TotalGames
	, GameDefinitionId
	, GameDefinition.Name AS GameDefinitionName
	FROM [dbo].[PlayerGameResult] INNER JOIN PlayedGame ON PlayedGame.Id = PlayerGameResult.PlayedGameId
	INNER JOIN GameDefinition ON GameDefinition.Id = PlayedGame.GameDefinitionId
	GROUP BY PlayerId, GameDefinitionId, GameDefinition.Name
),
NumberOfPlayers_CTE (PlayerId, GameDefinitionId, PlayerName, NumberOfPlayers)
AS
(
	SELECT A.PlayerId
		, GameDefinitionId
		, Player.Name
		, SUM(NumberOfPlayers) AS NumberOfPlayers
	FROM PlayedGame INNER JOIN PlayerGameResult A ON PlayedGame.Id = A.PlayedGameId
	INNER JOIN GameDefinition ON GameDefinition.Id = PlayedGame.GameDefinitionId
	INNER JOIN Player ON Player.id = A.PlayerId
	WHERE EXISTS (SELECT 1 FROM PlayerGameResult WHERE PlayerGameResult.PlayerId = A.PlayerId AND PlayerGameResult.PlayedGameId = PlayedGame.Id)
	GROUP BY A.PlayerId, Player.Name, GameDefinitionId, GameDefinition.Name
)
SELECT Player_CTE.GameDefinitionId AS GameDefinitionId
, GameDefinitionName
, Player_CTE.PlayerId
, NumberOfPlayers_CTE.PlayerName
, TotalGames
, (CAST(GamesWon AS Float) / CAST(TotalGames AS Float)) AS ActualWinPercentage
, NumberOfPlayers
, (CAST(GamesWon AS Float) / CAST(TotalGames AS Float)) - (CAST(TotalGames AS Float) / CAST(NumberOfPlayers_CTE.NumberOfPlayers AS Float)) AS Variance
, (CAST(TotalGames AS Float) / CAST(NumberOfPlayers_CTE.NumberOfPlayers AS Float)) AS ExpectedWinPercentage
FROM Player_CTE INNER JOIN NumberOfPlayers_CTE ON Player_CTE.PlayerId = NumberOfPlayers_CTE.PlayerId AND Player_CTE.GameDefinitionId = NumberOfPlayers_CTE.GameDefinitionId
WHERE TotalGames >= 5
GROUP BY Player_CTE.GameDefinitionId
, GameDefinitionName
, Player_CTE.PlayerId
, NumberOfPlayers_CTE.PlayerName
, TotalGames
,(CAST(GamesWon AS Float) / CAST(TotalGames AS Float))
,(CAST(GamesWon AS Float) / CAST(TotalGames AS Float)) - (CAST(TotalGames AS Float) / CAST(NumberOfPlayers_CTE.NumberOfPlayers AS Float))
,(CAST(TotalGames AS Float) / CAST(NumberOfPlayers_CTE.NumberOfPlayers AS Float))
,NumberOfPlayers
ORDER BY GameDefinitionId, PlayerId;