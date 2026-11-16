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
using System.Configuration;
using System.Linq;
using System.Globalization;
using AutoMapper;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Nemeses;
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
using Microsoft.Extensions.Logging.Abstractions;
using UI.Areas.Api.Models;
using UI.Models;
using UI.Models.Achievements;
using UI.Models.Champions;
using UI.Models.GameDefinitionModels;
using UI.Models.GamingGroup;
using UI.Models.Nemeses;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Models.Points;
using UI.Models.UniversalGameModels;
using UI.Models.User;

namespace UI.Transformations
{
    public class AutomapperConfiguration
    {
        public const string LicenseKeyAppSettingName = "AutoMapperLicenseKey";

        private static readonly Lazy<IMapper> LazyMapper = new Lazy<IMapper>(CreateMapper);

        public static IMapper Mapper => LazyMapper.Value;

        public static void Configure()
        {
            //builds the mapper configuration up front so mapping errors surface at startup
            var mapper = LazyMapper.Value;
        }

        private static IMapper CreateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.LicenseKey = ConfigurationManager.AppSettings[LicenseKeyAppSettingName];

                cfg.CreateMap<ApplicationLinkageMessage, ApplicationLinkage>(MemberList.Destination);
                cfg.CreateMap<ApplicationLinkage, ApplicationLinkageMessage>(MemberList.Destination);
                cfg.CreateMap<TopGamingGroupSummary, GamingGroupSummaryViewModel>(MemberList.Source);
                cfg.CreateMap<Player, PlayerInfoForUser>(MemberList.Destination)
                    .ForMember(x => x.PlayerId, opt => opt.MapFrom(y => y.Id))
                    .ForMember(x => x.PlayerName, opt => opt.MapFrom(y => y.Name));
                cfg.CreateMap<GamingGroupInfoForUser, GamingGroupSummaryViewModel>(MemberList.Destination)
                    .ForMember(x => x.GamingGroupChampion, opt => opt.MapFrom(y => y.GamingGroupChampion));
                cfg.CreateMap<VotableFeature, VotableFeatureViewModel>(MemberList.Destination);
                cfg.CreateMap<NewUserMessage, NewUser>(MemberList.Destination)
                    .ForMember(x => x.GamingGroupInvitationId, opt => opt.Ignore())
                    .ForMember(x => x.Source, opt => opt.Ignore());
                cfg.CreateMap<NewlyRegisteredUser, NewlyRegisteredUserMessage>(MemberList.Source);
                cfg.CreateMap<PlayedGameApplicationLinkage, ApplicationLinkage>(MemberList.Destination);
                cfg.CreateMap<PlayedGameSearchResult, PlayedGameSearchResultMessage>(MemberList.Destination)
                    .ForSourceMember(x => x.PlayerGameResults, opt => opt.DoNotValidate())
                    .ForMember(x => x.NemeStatsUrl, opt => opt.Ignore())
                    .ForMember(x => x.DateLastUpdated,
                        opt => opt.MapFrom(src => src.DateLastUpdated.ToString("yyyy-MM-dd")))
                    .ForMember(x => x.DatePlayed, opt => opt.MapFrom(src => src.DatePlayed.ToString("yyyy-MM-dd")))
                    .ForMember(x => x.WinnerType, opt => opt.MapFrom(src => src.WinnerType.ToString()));
                cfg.CreateMap<PlayerResult, PlayerGameResultMessage>(MemberList.Destination)
                      .ForMember(x => x.TotalNemeStatsPointsAwarded, opt => opt.MapFrom(src => src.TotalPoints));
                cfg.CreateMap<PlayerGameSummary, PlayerGameSummaryViewModel>(MemberList.Source);
                cfg.CreateMap<PlayerInfoForUser, PlayerInfoForUserMessage>(MemberList.Destination)
                    .ForMember(x => x.NemeStatsUrl, opt => opt.Ignore());
                cfg.CreateMap<GamingGroupInfoForUser, GamingGroupInfoForUserMessage>(MemberList.Destination)
                    .ForMember(x => x.NemeStatsUrl, opt => opt.Ignore());
                cfg.CreateMap<UserInformation, UserInformationMessage>(MemberList.Destination);
                cfg.CreateMap<PlayerWinRecord, PlayerSummaryViewModel>(MemberList.Destination)
                    .ForMember(x => x.SpecialBadgeTypes, opt => opt.MapFrom(src => src.MapSpecialBadges()))
                    .ForMember(x => x.PlayerName,
                        opt => opt.MapFrom(src => PlayerNameBuilder.BuildPlayerName(src.PlayerName, src.PlayerActive)))
                    .ForMember(x => x.RegisteredUserEmailAddress, opt => opt.Ignore());
                cfg.CreateMap<PlayerWinRecord, GameDefinitionPlayerSummaryViewModel>(MemberList.Destination)
                      .ForMember(x => x.SpecialBadgeTypes, opt => opt.MapFrom(src => src.MapSpecialBadges()))
                      .ForMember(x => x.RegisteredUserEmailAddress, opt => opt.Ignore())
                      .ForMember(x => x.PlayerName, opt => opt.MapFrom(src => PlayerNameBuilder.BuildPlayerName(src.PlayerName, src.PlayerActive)));
                cfg.CreateMap<GameDefinitionTotal, GameDefinitionTotalMessage>(MemberList.Destination);
                cfg.CreateMap<GameDefinitionTotals, GameDefinitionTotalsMessage>(MemberList.Destination);
                cfg.CreateMap<GameDefinition, GameDefinitionEditViewModel>(MemberList.Destination)
                                      .ForMember(x => x.GameDefinitionId, opt => opt.MapFrom(src => src.Id));
                cfg.CreateMap<GameDefinitionEditViewModel, GameDefinitionUpdateRequest>(MemberList.Destination);
                cfg.CreateMap<CreateGameDefinitionViewModel, CreateGameDefinitionRequest>(MemberList.Destination)
                      //for now, GamingGroupId is optional and only passed from the API
                      .ForMember(x => x.GamingGroupId, opt => opt.Ignore());
                cfg.CreateMap<PlayerStatistics, PlayerStatisticsMessage>(MemberList.Destination)
                      .ForMember(x => x.BaseNemePoints, opt => opt.MapFrom(src => src.NemePointsSummary.BaseNemePoints))
                      .ForMember(x => x.GameDurationBonusNemePoints, opt => opt.MapFrom(src => src.NemePointsSummary.GameDurationBonusNemePoints))
                      .ForMember(x => x.WeightBonusNemePoints, opt => opt.MapFrom(src => src.NemePointsSummary.WeightBonusNemePoints))
                      .ForMember(x => x.TotalPoints, opt => opt.MapFrom(src => src.NemePointsSummary.TotalPoints));
                cfg.CreateMap<PlayerQuickStats, PlayerQuickStatsViewModel>(MemberList.Destination)
                    .ForMember(m => m.LastGamingGroupPlayedGame, opt => opt.Ignore());
                cfg.CreateMap<NemePointsSummary, NemePointsSummaryViewModel>(MemberList.Destination)
                      .ConstructUsing(x => new NemePointsSummaryViewModel(x.BaseNemePoints, x.GameDurationBonusNemePoints, x.WeightBonusNemePoints));
                cfg.CreateMap<TrendingGame, TrendingGameViewModel>(MemberList.Destination);
                cfg.CreateMap<BoardGameGeekGameDefinition, BoardGameGeekGameDefinitionViewModel>()
                    .ForMember(m => m.BoardGameGeekUri,opt => opt.MapFrom(src => BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(src.Id)))
                    .ForMember(m => m.Categories,opt => opt.MapFrom(src => src.Categories.Select(c=>c.CategoryName)))
                    .ForMember(m => m.Mechanics, opt => opt.MapFrom(src => src.Mechanics.Select(c => c.MechanicName)))
                    .ForMember(m => m.WeightDescription, opt => opt.Ignore());
                cfg.CreateMap<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>(MemberList.Destination);

                cfg.CreateMap<PlayedGameMessage, NewlyCompletedGame>(MemberList.Destination)
                    .ForMember(m => m.TransactionSource, opt => opt.Ignore())
                    .ForMember(m => m.DatePlayed, opt => opt.MapFrom(src => ParseDatePlayed(src.DatePlayed)));

                cfg.CreateMap<UpdatedPlayedGameMessage, UpdatedGame>(MemberList.Destination)
                    .ForMember(m => m.DatePlayed, opt => opt.MapFrom(src => ParseDatePlayed(src.DatePlayed)));

                cfg.CreateMap<PlayedGameFilterMessage, PlayedGameFilter>(MemberList.Source);

                cfg.CreateMap<BoardGameGeekInfo, BoardGameGeekInfoViewModel>()
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

                cfg.CreateMap<UniversalGameStats, UniversalGameStatsViewModel>()
                    .ForMember(m => m.AveragePlayersPerGame,
                        opt => opt.MapFrom(src => $"{(src.AveragePlayersPerGame ?? 0):0.#}"));

                cfg.CreateMap<BoardGameGeekGameSummary, UniversalGameDetailsViewModel>(MemberList.Destination)
                    .ForMember(m => m.GamingGroupGameDefinitionSummary, opt => opt.Ignore());

                cfg.CreateMap<ChampionData, ChampionDataModel>(MemberList.Destination);

                cfg.CreateMap<AchievementRelatedPlayerSummary, PlayerListSummaryViewModel>();

                cfg.CreateMap<PlayerAchievementWinner, PlayerAchievementWinnerViewModel>();

                cfg.CreateMap<AchievementWinner, AchievementWinnerViewModel>();

                cfg.CreateMap<PlayerAchievementDetails, PlayerAchievementViewModel>();

                cfg.CreateMap<IAchievement, PlayerAchievementViewModel>(MemberList.Source)
                    .ForMember(m => m.AchievementId, opt => opt.MapFrom(achievement => achievement.Id))
                    .ForMember(m => m.AchievementName, opt => opt.MapFrom(achievement => achievement.Name))
                    .ForSourceMember(m => m.DescriptionFormat, opt => opt.DoNotValidate())
                    .ForSourceMember(m => m.Group, opt => opt.DoNotValidate());

                cfg.CreateMap<AggregateAchievementSummary, AchievementTileViewModel>();

                cfg.CreateMap<PlayerAchievementWinner, PlayerAchievementSummaryViewModel>();

                cfg.CreateMap<AchievementRelatedGameDefinitionSummary, GameDefinitionSummaryListViewModel>();

                cfg.CreateMap<AchievementRelatedPlayedGameSummary, PlayedGameQuickStatsViewModel>()
                    .ForMember(m => m.BoardGameGeekUri,
                        opt => opt.MapFrom(y => BoardGameGeekUriBuilder.BuildBoardGameGeekGameUri(y.BoardGameGeekGameDefinitionId)));

                cfg.CreateMap<GamingGroupStats, GamingGroupStatsViewModel>();

                cfg.CreateMap<BasicUserInfo, BasicUserInfoViewModel>();

                cfg.CreateMap<EditPlayedGameInfo, EditPlayedGameViewModel>()
                    .ForMember(m => m.EditMode, opt => opt.Ignore())
                    .ForMember(m => m.PlayedGameId, opt => opt.Ignore())
                    .ForMember(m => m.MostActivePlayers, opt => opt.Ignore())
                    .ForMember(m => m.RecentPlayedGames, opt => opt.Ignore())
                    .ForMember(m => m.MostPlayedGames, opt => opt.Ignore())
                    .ForMember(m => m.GameType, opt => opt.Ignore());

                cfg.CreateMap<PlayerQuickStats, HomePagePlayerSummary>()
                    .ForMember(m => m.LastGamingGroupPlayedGame, opt => opt.Ignore());

                cfg.CreateMap<HomePagePlayerSummary, PlayerQuickStatsViewModel>();

                cfg.CreateMap<NemesisChange, NemesisChangeViewModel>();

                cfg.CreateMap<ChampionChange, ChampionChangeViewModel>();

                //maps used by the ICustomMapper implementations in UI.Mappers.CustomMappers
                cfg.CreateMap<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>();
                cfg.CreateMap<SavePlayedGameRequest, NewlyCompletedGame>()
                    .ForMember(m => m.PlayerRanks, o => o.Ignore())
                    .ForMember(m => m.GamingGroupId, o => o.Ignore())
                    .ForMember(m => m.ApplicationLinkages, opt => opt.Ignore())
                    .ForMember(m => m.TransactionSource, opt => opt.Ignore());
                cfg.CreateMap<CreatePlayerRankRequest, PlayerRank>();
                cfg.CreateMap<GameDefinitionSummary, GameDefinitionSummaryListViewModel>();
                cfg.CreateMap<Player, PlayerListSummaryViewModel>()
                    .ForMember(m => m.PlayerName, o => o.MapFrom(s => s.Name))
                    .ForMember(m => m.PlayerId, o => o.MapFrom(s => s.Id))
                    .ForMember(m => m.GamingGroupId, o => o.MapFrom(s => s.GamingGroupId))
                    .ForMember(m => m.GamingGroupName, o => o.MapFrom(s => s.GamingGroup.Name));
                cfg.CreateMap<SavePlayedGameRequest, UpdatedGame>()
                    .ForMember(m => m.PlayerRanks, o => o.Ignore())
                    .ForMember(m => m.ApplicationLinkages, opt => opt.Ignore())
                    .ForMember(m => m.GamingGroupId, opt => opt.Ignore());
            }, NullLoggerFactory.Instance);

            return configuration.CreateMapper();
        }

        private static DateTime ParseDatePlayed(string datePlayed)
        {
            if (string.IsNullOrWhiteSpace(datePlayed))
            {
                return DateTime.UtcNow;
            }

            return DateTime.ParseExact(datePlayed, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
    }
}
