using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Nemeses;
namespace BusinessLogic.DataAccess.Repositories
{
    public interface IPlayerRepository
    {
        NemesisData GetNemesisData(int playerId);
    }
}
