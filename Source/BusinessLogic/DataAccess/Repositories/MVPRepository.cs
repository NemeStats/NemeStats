using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BusinessLogic.Models;
using BusinessLogic.Models.MVPData;

namespace BusinessLogic.DataAccess.Repositories
{
    public class MVPRepository : IMVPRepository
    {
        private readonly IDataContext _dataContext;

        public MVPRepository(IDataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public MVPData GetMVPData(int gameDefinitionId)
        {
            var gameDefinition = _dataContext.FindById<GameDefinition>(gameDefinitionId);

            if (gameDefinition == null)
            {
                return null;
            }

            var top2 = _dataContext.GetQueryable<PlayerGameResult>()
                .Include(pgr => pgr.PlayedGame)
                .Where(pgr => pgr.PlayedGame.GameDefinitionId == gameDefinitionId && pgr.PointsScored != null)
                .OrderByDescending(pgr => pgr.PointsScored)
                .ThenBy(pgr => pgr.PlayedGame.DatePlayed)
                .Select(pgr => new MVPData()
                {
                    Id = pgr.Id,
                    PointsScored = pgr.PointsScored.Value,
                    PlayerId = pgr.PlayerId,
                    DatePlayed = pgr.PlayedGame.DatePlayed
                })
                .Take(2)
                .ToList();

            //No one has scored 
            if (!top2.Any())
            {
                return null;
            }

            //Only one player has scored
            if (top2.Count == 1)
            {
                return top2.First();
            }

            //There is a tie
            if (top2[0].PointsScored == top2[1].PointsScored)
            {
                if (gameDefinition.MVP != null)
                {
                    //One of the players tied is the current MVP
                    var currentMVPIsPresent = top2.FirstOrDefault(r => r.PlayerId == gameDefinition.MVP.PlayerId);
                    if (currentMVPIsPresent != null)
                    {
                        return currentMVPIsPresent;
                    }
                }

            }

            return top2.First();


        }

    }


}