#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.PlayedGames
{
    public interface IPlayedGameSaver
    {
        PlayedGame UpdatePlayedGame(UpdatedGame updatedGame, TransactionSource transactionSource, ApplicationUser currentUser);

        void CreateApplicationLinkages(IList<ApplicationLinkage> applicationLinkages, int playedGameId, IDataContext dataContext);

        void ValidateAccessToPlayers(IEnumerable<PlayerRank> playerRanks, int gamingGroupId, ApplicationUser currentUser, IDataContext dataContext);

        List<PlayerGameResult> MakePlayerGameResults(SaveableGameBase savedGame, int? boardGameGeekGameDefinitionId, IDataContext dataContext);

        PlayedGame TransformNewlyCompletedGameIntoPlayedGame(SaveableGameBase savedGame, int gamingGroupId, string applicationUserId, List<PlayerGameResult> playerGameResults);

        void DoPostSaveStuff(TransactionSource transactionSource, ApplicationUser currentUser, int playedGameId, int gameDefinitionId,
            List<PlayerGameResult> playerGameResults, IDataContext dataContext);
    }
}
