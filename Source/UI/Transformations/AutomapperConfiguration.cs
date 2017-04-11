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

using System;
using System.Linq;
using System.Globalization;
using AutoMapper;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.Points;
using BusinessLogic.Models.User;
using UI.Areas.Api.Models;
using UI.Models;
using UI.Models.Achievements;
using UI.Models.GameDefinitionModels;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Models.Points;
using UI.Models.UniversalGameModels;

namespace UI.Transformations
{
    public class AutomapperConfiguration
    {
        public static void Configure()
        {
            Mapper.CreateMap<ApplicationLinkageMessage, ApplicationLinkage>(MemberList.Destination);
            Mapper.CreateMap<ApplicationLinkage, ApplicationLinkageMessage>(MemberList.Destination);
            Mapper.CreateMap<TopGamingGroupSummary, TopGamingGroupSummaryViewModel>(MemberList.Source);
            Mapper.CreateMap<VotableFeature, VotableFeatureViewModel>(MemberList.Destination);
            Mapper.CreateMap<NewUserMessage, NewUser>(MemberList.Destination)
                .ForMember(x => x.GamingGroupInvitationId, opt => opt.Ignore())
                .ForMember(x => x.Source, opt => opt.Ignore());
            Mapper.CreateMap<NewlyRegisteredUser, NewlyRegisteredUserMessage>(MemberList.Source);
            Mapper.CreateMap<PlayedGameApplicationLinkage, ApplicationLinkage>(MemberList.Destination);
            Mapper.CreateMap<PlayedGameSearchResult, PlayedGameSearchResultMessage>(MemberList.Destination)
                  .ForSourceMember(x => x.PlayerGameResults, opt => opt.Ignore())
                  .ForMember(x => x.DateLastUpdated, opt => opt.MapFrom(src => src.DateLastUpdated.ToString("yyyy-MM-dd")))
                  .ForMember(x => x.DatePlayed, opt => opt.MapFrom(src => src.DatePlayed.ToString("yyyy-MM-dd")));
            Mapper.CreateMap<PlayerResult, PlayerGameResultMessage>(MemberList.Destination)
                  .ForMember(x => x.TotalNemeStatsPointsAwarded, opt => opt.MapFrom(src => src.TotalPoints));
            Mapper.CreateMap<PlayerGameSummary, PlayerGameSummaryViewModel>(MemberList.Source);
            Mapper.CreateMap<PlayerInfoForUser, PlayerInfoForUserMessage>(MemberList.Destination);
            Mapper.CreateMap<GamingGroupInfoForUser, GamingGroupInfoForUserMessage>(MemberList.Destination);
            Mapper.CreateMap<UserInformation, UserInformationMessage>(MemberList.Destination);
            Mapper.CreateMap<PlayerWinRecord, PlayerSummaryViewModel>(MemberList.Destination)
                  .ForMember(x => x.SpecialBadgeTypes, opt => opt.MapFrom(src => src.MapSpecialBadges()))
                  .ForMember(x => x.PlayerName, opt => opt.MapFrom(src => PlayerNameBuilder.BuildPlayerName(src.PlayerName, src.PlayerActive)));
            Mapper.CreateMap<PlayerWinRecord, GameDefinitionPlayerSummaryViewModel>(MemberList.Destination)
                  .ForMember(x => x.SpecialBadgeTypes, opt => opt.MapFrom(src => src.MapSpecialBadges()))
                  .ForMember(x => x.PlayerName, opt => opt.MapFrom(src => PlayerNameBuilder.BuildPlayerName(src.PlayerName, src.PlayerActive)));
            Mapper.CreateMap<GameDefinitionTotal, GameDefinitionTotalMessage>(MemberList.Destination);
            Mapper.CreateMap<GameDefinitionTotals, GameDefinitionTotalsMessage>(MemberList.Destination);
            Mapper.CreateMap<GameDefinition, GameDefinitionEditViewModel>(MemberList.Destination)
                                  .ForMember(x => x.GameDefinitionId, opt => opt.MapFrom(src => src.Id));
            Mapper.CreateMap<GameDefinitionEditViewModel, GameDefinitionUpdateRequest>(MemberList.Destination);
            Mapper.CreateMap<CreateGameDefinitionViewModel, CreateGameDefinitionRequest>(MemberList.Destination)
                  //for now, GamingGroupId is optional and only passed from the API
                  .ForMember(x => x.GamingGroupId, opt => opt.Ignore());
            Mapper.CreateMap<PlayerStatistics, PlayerStatisticsMessage>(MemberList.Destination)
                  .ForMember(x => x.BaseNemePoints, opt => opt.MapFrom(src => src.NemePointsSummary.BaseNemePoints))
                  .ForMember(x => x.GameDurationBonusNemePoints, opt => opt.MapFrom(src => src.NemePointsSummary.GameDurationBonusNemePoints))
                  .ForMember(x => x.WeightBonusNemePoints, opt => opt.MapFrom(src => src.NemePointsSummary.WeightBonusNemePoints))
                  .ForMember(x => x.TotalPoints, opt => opt.MapFrom(src => src.NemePointsSummary.TotalPoints));
            Mapper.CreateMap<PlayerQuickStats, PlayerQuickStatsViewModel>(MemberList.Destination);
            Mapper.CreateMap<NemePointsSummary, NemePointsSummaryViewModel>(MemberList.Destination)
                  .ConstructUsing(x => new NemePointsSummaryViewModel(x.BaseNemePoints, x.GameDurationBonusNemePoints, x.WeightBonusNemePoints));
            Mapper.CreateMap<TrendingGame, TrendingGameViewModel>(MemberList.Destination);
            Mapper.CreateMap<BoardGameGeekGameDefinition, BoardGameGeekGameDefinitionViewModel>()
                .ForMember(m => m.BoardGameGeekUri,opt => opt.MapFrom(src => BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(src.Id)))
                .ForMember(m => m.Categories,opt => opt.MapFrom(src => src.Categories.Select(c=>c.CategoryName)))
                .ForMember(m => m.Mechanics, opt => opt.MapFrom(src => src.Mechanics.Select(c => c.MechanicName)))
                .ForMember(m => m.WeightDescription, opt => opt.Ignore());
            Mapper.CreateMap<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>(MemberList.Destination);

            Mapper.CreateMap<PlayedGameMessage, NewlyCompletedGame>(MemberList.Destination)
                .ForMember(m => m.TransactionSource, opt => opt.Ignore())
                .ForMember(m => m.DatePlayed, opt => opt.ResolveUsing(x =>
                {
                    var datePlayed = DateTime.UtcNow;

                    if (!string.IsNullOrWhiteSpace(x.DatePlayed))
                    {
                        datePlayed = DateTime.ParseExact(x.DatePlayed, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    }

                    return datePlayed;
                }));

            Mapper.CreateMap<UpdatedPlayedGameMessage, UpdatedGame>(MemberList.Destination)
                .ForMember(m => m.DatePlayed, opt => opt.ResolveUsing(x =>
                {
                    var datePlayed = DateTime.UtcNow;

                    if (!string.IsNullOrWhiteSpace(x.DatePlayed))
                    {
                        datePlayed = DateTime.ParseExact(x.DatePlayed, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    }

                    return datePlayed;
                }));

            Mapper.CreateMap<PlayedGameFilterMessage, PlayedGameFilter>(MemberList.Source);

            Mapper.CreateMap<BoardGameGeekInfo, BoardGameGeekInfoViewModel>()
                 .ForMember(m => m.HideLinkToGlobalStats, opt => opt.Ignore())
                 .ForMember(m => m.BoardGameGeekUri,
                    opt => opt.MapFrom(src => BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(src.BoardGameGeekGameDefinitionId)))
                .ForMember(m => m.BoardGameGeekAverageWeightDescription,
                    opt => opt.MapFrom(src => new WeightTierCalculator().GetWeightTier(src.BoardGameGeekAverageWeight).ToString()))
                .ForMember(m => m.BoardGameGeekWeightPercent,
                    opt => opt.MapFrom(src => src.BoardGameGeekAverageWeight.HasValue ? ((src.BoardGameGeekAverageWeight.Value * 100) / BoardGameGeekGameDefinitionViewModel.MaxBggWeight).ToString(CultureInfo.InvariantCulture).Replace(",", ".") : "0"))
                    .ForMember(m => m.AveragePlayTime,
                    opt =>
                        opt.MapFrom(
                            src =>
                                !src.MaxPlayTime.HasValue
                                    ? src.MinPlayTime
                                    : (src.MinPlayTime.HasValue ? (src.MaxPlayTime.Value + src.MinPlayTime.Value) / 2 : src.MaxPlayTime)));

            Mapper.CreateMap<UniversalGameStats, UniversalGameStatsViewModel>()
                .ForMember(m => m.AveragePlayersPerGame,
                    opt => opt.MapFrom(src => $"{(src.AveragePlayersPerGame ?? 0):0.#}"));

            Mapper.CreateMap<BoardGameGeekGameSummary, UniversalGameDetailsViewModel>(MemberList.Destination)
                .ForMember(m => m.GamingGroupGameDefinitionSummary, opt => opt.Ignore());

            Mapper.CreateMap<ChampionData, ChampionDataModel>(MemberList.Destination);

            Mapper.CreateMap<AchievementRelatedPlayerSummary, PlayerListSummaryViewModel>();

            Mapper.CreateMap<PlayerAchievementWinner, PlayerAchievementWinnerViewModel>();

            Mapper.CreateMap<AchievementWinner, AchievementWinnerViewModel>();

            Mapper.CreateMap<PlayerAchievementDetails, PlayerAchievementViewModel>();

            Mapper.CreateMap<IAchievement, PlayerAchievementViewModel>(MemberList.Source)
                .ForMember(m => m.AchievementId, opt => opt.MapFrom(achievement => achievement.Id))
                .ForMember(m => m.AchievementName, opt => opt.MapFrom(achievement => achievement.Name))
                .ForSourceMember(m => m.DescriptionFormat, opt => opt.Ignore())
                .ForSourceMember(m => m.Group, opt => opt.Ignore());

            Mapper.CreateMap<AggregateAchievementSummary, AchievementTileViewModel>();

            Mapper.CreateMap<PlayerAchievementWinner, PlayerAchievementSummaryViewModel>();

            Mapper.CreateMap<AchievementRelatedGameDefinitionSummary, GameDefinitionSummaryListViewModel>();

            Mapper.CreateMap<AchievementRelatedPlayedGameSummary, PlayedGameQuickStatsViewModel>()
                .ForMember(m => m.BoardGameGeekUri,
                    opt => opt.MapFrom(y => BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(y.BoardGameGeekGameDefinitionId)));
        }
    }
}