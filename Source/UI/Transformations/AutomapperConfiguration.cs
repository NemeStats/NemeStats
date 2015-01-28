using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using UI.Models.GameDefinitionModels;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public class AutomapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<TopGamingGroupSummary, TopGamingGroupSummaryViewModel>(MemberList.Source);
            Mapper.CreateMap<NewGameDefinitionViewModel, GameDefinition>(MemberList.Source);
        }
    }
}