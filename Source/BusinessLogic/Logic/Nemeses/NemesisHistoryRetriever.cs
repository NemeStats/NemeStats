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
using System.Data.Entity;
using System.Linq;
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

        public List<NemesisChange> GetRecentNemesisChanges(int numberOfRecentNemeses)
        {
            return (from nemesisChange in dataContext.GetQueryable<Nemesis>().GroupBy(n => n.MinionPlayerId)
                                                     .Select(n => n.OrderByDescending(p => p.DateCreated)
                                                                   .FirstOrDefault())
                    select new NemesisChange
                    {
                        LossPercentageVersusNemesis = nemesisChange.LossPercentage,
                        NemesisPlayerId = nemesisChange.NemesisPlayerId,
                        NemesisPlayerName = nemesisChange.NemesisPlayer.Name,
                        MinionPlayerName = nemesisChange.MinionPlayer.Name,
                        MinionPlayerId = nemesisChange.MinionPlayerId,
                        DateCreated = nemesisChange.DateCreated
                    }).OrderByDescending(n => n.DateCreated)
                    .Take(numberOfRecentNemeses)
                    .ToList();
        }
    }
}
