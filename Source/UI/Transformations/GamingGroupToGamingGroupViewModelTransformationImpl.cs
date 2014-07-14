using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public class GamingGroupToGamingGroupViewModelTransformationImpl : GamingGroupToGamingGroupViewModelTransformation
    {
        public GamingGroupViewModel Build(GamingGroup gamingGroup)
        {
            GamingGroupViewModel viewModel = new GamingGroupViewModel()
            {
                Id = gamingGroup.Id,
                OwningUserId = gamingGroup.OwningUserId,
                Name = gamingGroup.Name,
                OwningUserName = gamingGroup.OwningUser.UserName
            };

            return viewModel;
        }
    }
}