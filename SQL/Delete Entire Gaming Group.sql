--this terrifying script is supposed to clear out a gaming group entirely

BEGIN TRANSACTION

-- the main gaming group that we are clearing out
DECLARE @gamingGroupId int = 13605

--indicates whether the actual GamingGroup record itself should get blown away
DECLARE @deletGamingGroupEntirely bit = 1

--delete played games / player game results
DELETE pgr
FROM PlayerGameResult pgr INNER JOIN PlayedGame pg ON pgr.PlayedGameId = pg.Id
WHERE pg.GamingGroupId = @gamingGroupId;

DELETE FROM PlayedGame WHERE GamingGroupId = @gamingGroupId;

--delete achievements
DELETE FROM PlayerAchievement WHERE PlayerId IN (SELECT Id FROM Player WHERE GamingGroupId = @gamingGroupId);

--delete champions
ALTER TABLE GameDefinition NOCHECK CONSTRAINT ALL
UPDATE GameDefinition SET ChampionId = NULL, PreviousChampionId = null WHERE GamingGroupId = @gamingGroupId;

DELETE FROM Champion WHERE PlayerId IN (SELECT Id FROM Player WHERE GamingGroupId = @gamingGroupId);
DELETE FROM Champion WHERE GameDefinitionId IN (SELECT GameDefinition.Id FROM GameDefinition WHERE GameDefinition.GamingGroupId = @gamingGroupId );

--delete game definitions (which also can have champions)
DELETE FROM GameDefinition WHERE GamingGroupId = @gamingGroupId;
ALTER TABLE GameDefinition WITH CHECK CHECK CONSTRAINT ALL

--blank out the Nemesis and Previous Nemesis so we can more easily delete from Nemesis
UPDATE Player SET NemesisId = null, PreviousNemesisId = null WHERE GamingGroupId = @gamingGroupId;

--delete minions in this gaming group
DELETE FROM Nemesis WHERE MinionPlayerId IN (SELECT Id FROM Player WHERE GamingGroupId = @gamingGroupId);

--delete nemesis in the gaming group
DELETE FROM Nemesis WHERE NemesisPlayerId IN (SELECT Id FROM Player WHERE GamingGroupId = @gamingGroupId);

UPDATE GamingGroup SET GamingGroupChampionPlayerId = NULL WHERE Id = @gamingGroupId;

--delete players in this gaming group
DELETE FROM Player WHERE GamingGroupId = @gamingGroupId;

--if specified, delete the actual gaming group and gaming group associations as well
if @deletGamingGroupEntirely = 1
BEGIN
	UPDATE AspNetUsers SET CurrentGamingGroupId = NULL WHERE CurrentGamingGroupId = @gamingGroupId
	DELETE FROM UserGamingGroup WHERE GamingGroupId = @gamingGroupId
	DELETE FROM GamingGroupInvitation WHERE GamingGroupId = @gamingGroupId;
	DELETE FROM GamingGroup WHERE Id = @gamingGroupId
END

ROLLBACK TRANSACTION
