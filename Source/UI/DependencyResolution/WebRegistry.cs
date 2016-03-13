using System.Web;
using BusinessLogic.Providers;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using StructureMap;
using StructureMap.Graph;
using UI.Controllers.Helpers;
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