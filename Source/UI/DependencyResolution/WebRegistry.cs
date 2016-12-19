using System.Web;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Providers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using StructureMap;
using StructureMap.Graph;
using UI.Mappers.CustomMappers;
using UI.Mappers.Interfaces;
using UI.Models.Achievements;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.DependencyResolution
{
    public class WebRegistry : Registry
    {
        public WebRegistry()
        {
            Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });            

           
            SetupUniquePerRequestMappings();

            SetupSingletonMappings();

            SetupMapperMappings(this);
        }

        public static void SetupMapperMappings(Registry r)
        {
            r.For<ICustomMapper<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>>().Use<GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper>();
            r.For<ICustomMapper<GameDefinitionSummary, GameDefinitionSummaryListViewModel>>().Use<GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper>();

            r.For<ICustomMapper<SavePlayedGameRequest, NewlyCompletedGame>>().Use<CreatePlayedGameRequestToNewlyCompletedGameMapper>();
            r.For<ICustomMapper<SavePlayedGameRequest, UpdatedGame>>().Use<SavePlayedGameRequestToUpdatedGameGameMapper>();

            r.For<ICustomMapper<IAchievement, AchievementSummaryViewModel>>().Use<AchievementToAchievementSummaryViewModelMapper>();
            r.For<ICustomMapper<IAchievement, AchievementViewModel>>().Use<AchievementToAchievementViewModelMapper>();

            r.For<ICustomMapper<PlayerAchievement, PlayerAchievementWinnerViewModel>>().Use<PlayerAchievementToPlayerAchievementWinnerViewModelMapper>();
            r.For<ICustomMapper<PlayerAchievement, PlayerAchievementViewModel>>().Use<PlayerAchievementToPlayerAchievementViewModelMapper>();
            r.For<ICustomMapper<PlayerAchievement, PlayerAchievementSummaryViewModel>>().Use<PlayerAchievementToPlayerAchievementSummaryViewModelMapper>();
            r.For<ICustomMapper<PlayerAchievement, PlayerAchievementSummaryViewModel>>().Use<PlayerAchievementToPlayerAchievementSummaryViewModelMapper>();

            r.For<ICustomMapper<Player, PlayerListSummaryViewModel>>().Use<PlayerToPlayerListSummaryViewModelMapper>();
            r.For<ICustomMapper<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>>().Use<PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper>();

        }

        private void SetupSingletonMappings()
        {
            For<IGameResultViewModelBuilder>().Singleton().Use<GameResultViewModelBuilder>();

            For<IPlayerDetailsViewModelBuilder>().Singleton().Use<PlayerDetailsViewModelBuilder>();

            For<IGamingGroupViewModelBuilder>().Singleton()
                                               .Use<GamingGroupViewModelBuilder>();

            For<IGamingGroupInvitationViewModelBuilder>().Singleton()
                                                         .Use<GamingGroupInvitationViewModelBuilder>();

            For<ITopPlayerViewModelBuilder>().Use<TopPlayerViewModelBuilder>();

            For<IPlayedGameDetailsViewModelBuilder>().Singleton().Use<PlayedGameDetailsViewModelBuilder>();

            For<IPlayerWithNemesisViewModelBuilder>().Singleton().Use<PlayerWithNemesisViewModelBuilder>();

            For<IMinionViewModelBuilder>().Singleton().Use<MinionViewModelBuilder>();

            For<INemesisChangeViewModelBuilder>().Singleton().Use<NemesisChangeViewModelBuilder>();

            For<IGameDefinitionDetailsViewModelBuilder>().Singleton().Use<GameDefinitionDetailsViewModelBuilder>();

            For<IGameDefinitionSummaryViewModelBuilder>().Singleton().Use<GameDefinitionSummaryViewModelBuilder>();

            For<IPlayerEditViewModelBuilder>().Singleton().Use<PlayerEditViewModelBuilder>();

            For<IDataProtectionProvider>().Singleton().Use<MachineKeyProtectionProvider>();

            For<ITransformer>().Singleton().Use<Transformer>();
        }

        private void SetupUniquePerRequestMappings()
        {
            For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);
        }
    }
}