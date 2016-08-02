--this terrifying script is supposed to clear out a gaming group entirely

BEGIN TRANSACTION
 
-- the main gaming group that we are clearing out
DECLARE @gamingGroupId int = 8247

--indicates whether the acutal GamingGroup record itself should get blown away
DECLARE @deletGamingGroupEntirely bit = 0
--if deleting the entire gaming group, may need to provide a fallback gaming group so we can 
--set the currentGamingGroupId of the user to this before deleting the gaming group
DECLARE @fallbackGamingGroupId int = 8246

--delete played games / player game results
DELETE pgr
FROM PlayerGameResult pgr INNER JOIN PlayedGame pg ON pgr.PlayedGameId = pg.Id
WHERE pg.GamingGroupId = @gamingGroupId;

DELETE FROM PlayedGame WHERE GamingGroupId = @gamingGroupId;

--delete achievements
DELETE FROM PlayerAchievement WHERE PlayerId IN (SELECT Id FROM Player WHERE GamingGroupId = @gamingGroupId);

--delete champions
ALTER TABLE GameDefinition NOCHECK CONSTRAINT ALL
DELETE FROM Champion WHERE PlayerId IN (SELECT Id FROM Player WHERE GamingGroupId = @gamingGroupId);

--delete game definitions (which also can have champions)
DELETE FROM GameDefinition WHERE GamingGroupId = @gamingGroupId;
ALTER TABLE GameDefinition WITH CHECK CHECK CONSTRAINT ALL

--delete minions in this gaming group
DELETE FROM Nemesis WHERE MinionPlayerId IN (SELECT Id FROM Player WHERE GamingGroupId = @gamingGroupId);

--delete nemesis in the gaming group
DELETE FROM Nemesis WHERE NemesisPlayerId IN (SELECT Id FROM Player WHERE GamingGroupId = @gamingGroupId);

--delete players in this gaming group
DELETE FROM Player WHERE GamingGroupId = @gamingGroupId;

--if specified, delete the actual gaming group and gaming group associations as well
if @deletGamingGroupEntirely = 1
BEGIN
	UPDATE AspNetUsers SET CurrentGamingGroupId = @fallbackGamingGroupId WHERE CurrentGamingGroupId = @gamingGroupId
	DELETE FROM UserGamingGroup WHERE GamingGroupId = @gamingGroupId
	DELETE FROM GamingGroupInvitation WHERE GamingGroupId = @gamingGroupId;
	DELETE FROM GamingGroup WHERE Id = @gamingGroupId
END

COMMIT TRANSACTION
