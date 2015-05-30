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
using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.User;
using Microsoft.Ajax.Utilities;
using UI.Areas.Api.Models;
using UI.Models;
using UI.Models.GameDefinitionModels;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public class AutomapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<TopGamingGroupSummary, TopGamingGroupSummaryViewModel>(MemberList.Source);
            Mapper.CreateMap<NewGameDefinitionViewModel, GameDefinition>(MemberList.Source).ForSourceMember(x => x.ReturnUrl, opt => opt.Ignore());
            Mapper.CreateMap<VotableFeature, VotableFeatureViewModel>(MemberList.Destination);
            Mapper.CreateMap<NewUserMessage, NewUser>(MemberList.Source);
            Mapper.CreateMap<NewlyRegisteredUser, NewlyRegisteredUserMessage>(MemberList.Source);
            Mapper.CreateMap<PlayedGameSearchResult, PlayedGameSearchResultMessage>(MemberList.Destination)
                  .ForSourceMember(x => x.PlayerGameResults, opt => opt.Ignore())
                  .ForMember(x => x.DateLastUpdated, opt => opt.MapFrom(src => src.DateLastUpdated.ToString("yyyy-MM-dd")))
                  .ForMember(x => x.DatePlayed, opt => opt.MapFrom(src => src.DatePlayed.ToString("yyyy-MM-dd")));
            Mapper.CreateMap<PlayerResult, PlayerGameResultMessage>(MemberList.Destination);
        }
    }
}