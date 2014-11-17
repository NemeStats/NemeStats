using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Champions;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface IChampionRepository
    {
        ChampionData GetChampionData(int gameDefinitionId);
    }
}
