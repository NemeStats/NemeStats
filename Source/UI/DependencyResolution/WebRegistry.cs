using System.Web;
using System.Web.Http.Filters;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Providers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using StructureMap;
using UI.Attributes;
using UI.Mappers.CustomMappers;
using UI.Mappers.Interfaces;
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

            r.For<ICustomMapper<Player, PlayerListSummaryViewModel>>().Use<PlayerToPlayerListSummaryViewModelMapper>();
            r.For<ICustomMapper<PlayedGameQuickStats, PlayedGameQuickStatsViewModel>>().Use<PlayedGameQuickStatsToPlayedGameQuickStatsViewModelMapper>();

        }

        private void SetupSingletonMappings()
        {
            For<System.Web.Mvc.IFilterProvider>().Singleton().Use<GlobalFilterProvider>();

            For<IGameResultViewModelBuilder>().Singleton().Use<GameResultViewModelBuilder>();

            For<IPlayerDetailsViewModelBuilder>().Singleton().Use<PlayerDetailsViewModelBuilder>();

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