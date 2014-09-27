using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.PlayedGames
{
    public class PlayedGameDeleter : IPlayedGameDeleter
    {
        private IDataContext dataContext;
        private INemesisRecalculator nemesisRecalculator;

        public PlayedGameDeleter(IDataContext dataContext, INemesisRecalculator nemesisRecalculatorMock)
        {
            this.dataContext = dataContext;
            this.nemesisRecalculator = nemesisRecalculatorMock;
        }

        public void DeletePlayedGame(int playedGameId, ApplicationUser currentUser)
        {
            List<int> playerIds = (from playerResult in dataContext.GetQueryable<PlayerGameResult>()
                                   where playerResult.PlayedGameId == playedGameId
                                   select playerResult.PlayerId).ToList();
            dataContext.DeleteById<PlayedGame>(playedGameId, currentUser);
            dataContext.CommitAllChanges();
                     
            foreach(int playerId in playerIds)
            {
                nemesisRecalculator.RecalculateNemesis(playerId, currentUser);
            }
        }
    }
}
