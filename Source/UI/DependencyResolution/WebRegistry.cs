using System.Web;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Providers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using StructureMap;
using StructureMap.Graph;
using UI.Controllers.Helpers;
using UI.Mappers;
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
            this.Scan(
                scan =>
                {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });            

           
            this.SetupUniquePerRequestMappings();

            this.SetupTransientMappings();

            this.SetupSingletonMappings();

            this.SetupMapperMappings();
        }

        private void SetupMapperMappings()
        {
            this.For<IMapperFactory>().Use<MapperFactory>();

            this.For<ICustomMapper<GameDefinitionDisplayInfo, GameDefinitionDisplayInfoViewModel>>().Use<GameDefinitionDisplayInfoToGameDefinitionDisplayInfoViewModelMapper>();
            this.For<ICustomMapper<GameDefinitionSummary, GameDefinitionSummaryListViewModel>>().Use<GameDefinitionSummaryToGameDefinitionSummaryListViewModelMapper>();

            this.For<ICustomMapper<CreatePlayedGameRequest, NewlyCompletedGame>>().Use<CreatePlayedGameRequestToNewlyCompletedGameMapper>();

            this.For<ICustomMapper<IAchievement, AchievementSummaryViewModel>>().Use<AchievementToAchievementSummaryViewModelMapper>();
            this.For<ICustomMapper<IAchievement, AchievementViewModel>>().Use<AchievementToAchievementViewModelMapper>();

            this.For<ICustomMapper<PlayerAchievement, PlayerAchievementWinnerViewModel>>().Use<PlayerAchievementToPlayerAchievementWinnerViewModelMapper>();
            this.For<ICustomMapper<PlayerAchievement, PlayerAchievementViewModel>>().Use<PlayerAchievementToPlayerAchievementViewModelMapper>();
            this.For<ICustomMapper<PlayerAchievement, PlayerAchievementSummaryViewModel>>().Use<PlayerAchievementToPlayerAchievementSummaryViewModelMapper>();
            this.For<ICustomMapper<PlayerAchievement, PlayerAchievementSummaryViewModel>>().Use<PlayerAchievementToPlayerAchievementSummaryViewModelMapper>();

            this.For<ICustomMapper<Player, PlayerListSummaryViewModel>>().Use<PlayerToPlayerListSummaryViewModelMapper>();
            this.For<ICustomMapper<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>>().Use<PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper>();

        }

        private void SetupSingletonMappings()
        {
            this.For<IGameResultViewModelBuilder>().Singleton().Use<GameResultViewModelBuilder>();

            this.For<IPlayerDetailsViewModelBuilder>().Singleton().Use<PlayerDetailsViewModelBuilder>();

            this.For<IGamingGroupViewModelBuilder>().Singleton()
                                               .Use<GamingGroupViewModelBuilder>();

            this.For<IGamingGroupInvitationViewModelBuilder>().Singleton()
                                                         .Use<GamingGroupInvitationViewModelBuilder>();

            this.For<ITopPlayerViewModelBuilder>().Use<TopPlayerViewModelBuilder>();

            this.For<IPlayedGameDetailsViewModelBuilder>().Singleton().Use<PlayedGameDetailsViewModelBuilder>();

            this.For<IPlayerWithNemesisViewModelBuilder>().Singleton().Use<PlayerWithNemesisViewModelBuilder>();

            this.For<IMinionViewModelBuilder>().Singleton().Use<MinionViewModelBuilder>();

            this.For<INemesisChangeViewModelBuilder>().Singleton().Use<NemesisChangeViewModelBuilder>();

            this.For<IGameDefinitionDetailsViewModelBuilder>().Singleton().Use<GameDefinitionDetailsViewModelBuilder>();

            this.For<IGameDefinitionSummaryViewModelBuilder>().Singleton().Use<GameDefinitionSummaryViewModelBuilder>();

            this.For<IPlayerEditViewModelBuilder>().Singleton().Use<PlayerEditViewModelBuilder>();

            this.For<IDataProtectionProvider>().Singleton().Use<MachineKeyProtectionProvider>();

            this.For<ITransformer>().Singleton().Use<Transformer>();
        }

        private void SetupTransientMappings()
        {
            this.For<IShowingXResultsMessageBuilder>().Use<ShowingXResultsMessageBuilder>();

        }

        private void SetupUniquePerRequestMappings()
        {
            this.For<IAuthenticationManager>().Use(() => HttpContext.Current.GetOwinContext().Authentication);
        }
    }
}