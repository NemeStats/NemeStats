using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLogic.Models;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerEditViewModelBuilder : IPlayerEditViewModelBuilder
    {
        public PlayerEditViewModel Build(Player player)
        {
            return new PlayerEditViewModel
            {
                Active = player.Active,
                Id = player.Id,
                GamingGroupId = player.GamingGroupId,
                Name = player.Name
            };
        }
    }
}