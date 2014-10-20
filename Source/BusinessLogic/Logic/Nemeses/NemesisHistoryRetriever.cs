using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;

namespace BusinessLogic.Logic.Nemeses
{
    public class NemesisHistoryRetriever : INemesisHistoryRetriever
    {
        private readonly IDataContext dataContext;

        public NemesisHistoryRetriever(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public NemesisHistoryData GetNemesisHistory(int playerId, int numberOfPreviousNemesisToReturn)
        {
            NemesisHistoryData nemesisHistoryData = new NemesisHistoryData();

            var nemesisData = (from Nemesis nemesis in dataContext.GetQueryable<Nemesis>().Include(nem => nem.NemesisPlayer)
                                            where nemesis.MinionPlayerId == playerId
                                            select new 
                                            {
                                                Nemesis = nemesis,
                                                IsCurrentNemesis = nemesis.MinionPlayer.NemesisId == nemesis.Id
                                            })
                                            .OrderByDescending(nem => nem.Nemesis.DateCreated)
                                            //Take an additional one record in case the first one is the current Nemesis
                                            .Take(numberOfPreviousNemesisToReturn + 1)
                                            .ToList();

            if (nemesisData.Count > 0)
            {
                int startIndexOfPreviousNemeses = 0;
                if (nemesisData[0].IsCurrentNemesis)
                {
                    nemesisHistoryData.CurrentNemesis = nemesisData[0].Nemesis;
                    startIndexOfPreviousNemeses = 1;
                }

                for(int i = startIndexOfPreviousNemeses; i < nemesisData.Count; i++)
                {
                    nemesisHistoryData.PreviousNemeses.Add(nemesisData[i].Nemesis);
                }
            }

            if (nemesisHistoryData.CurrentNemesis == null)
            {
                nemesisHistoryData.CurrentNemesis = new NullNemesis();
            }

            return nemesisHistoryData;
        }
    }
}
