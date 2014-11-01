using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Logic.Nemeses;
using UI.Models.Nemeses;

namespace UI.Transformations
{
    public class NemesisChangeViewModelBuilder : INemesisChangeViewModelBuilder
    {
        public List<NemesisChangeViewModel> Build(List<NemesisChange> nemesisChanges)
        {
            return nemesisChanges.Select(nemesisChange => new NemesisChangeViewModel
            {
                NemesisPlayerId = nemesisChange.NemesisPlayerId,
                NemesisPlayerName = nemesisChange.NemesisPlayerName,
                PlayerId = nemesisChange.MinionPlayerId,
                PlayerName = nemesisChange.MinionPlayerName,
                LossPercentageVersusNemesis = nemesisChange.LossPercentageVersusNemesis
            }).ToList();
        }
    }
}