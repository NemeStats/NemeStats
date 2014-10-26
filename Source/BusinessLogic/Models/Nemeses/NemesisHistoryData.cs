using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models.Nemeses
{
    public class NemesisHistoryData
    {
        public NemesisHistoryData()
        {
            PreviousNemeses = new List<Nemesis>();
        }

        public Nemesis CurrentNemesis { get; set; }
        public List<Nemesis> PreviousNemeses { get; set; }
    }
}
