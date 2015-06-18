
WITH PlayerSummary_CTE (BoardGameGeekObjectId, PlayerId, NumberofPlayers, NumberOfGamesPlayedWithThisManyPlayers, TotalVariation)
AS
(
	SELECT GameDefinition.BoardGameGeekObjectId, 
	PlayerGameResult.PlayerId, 
	PlayedGame.NumberofPlayers,
	COUNT(*) AS NumberOfGamesPlayedWithThisManyPlayers,
	SUM(((CAST(PlayerGameResult.GameRank AS Float) - (CAST(PlayedGame.NumberOfPlayers AS Float) / 2.0)))) AS TotalVariation
	FROM GameDefinition 
	INNER JOIN PlayedGame ON GameDefinition.Id = PlayedGame.GameDefinitionid
	INNER JOIN PlayerGameResult on PlayedGame.Id = PlayerGameResult.PlayedGameId
	GROUP BY GameDefinition.BoardGameGeekObjectId, 
	PlayedGame.NumberofPlayers, 
	PlayerGameResult.PlayerId
	HAVING COUNT(*) > 1
)
SELECT BoardGameGeekObjectId
,AVG(ABS(((TotalVariation / NumberOfGamesPlayedWithThisManyPlayers) / CAST(NumberOfPlayers AS Float) / 2.0))) AS AverageVariation
FROM PlayerSummary_CTE
GROUP BY BoardGameGeekObjectId
ORDER BY AverageVariation DESC;

SELECT * 
FROM GameDefinition
WHERE BoardGameGeekObjectid IN (34219, 147716)