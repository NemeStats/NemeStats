// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;
using System.Configuration.Abstractions;
using System.Data.Entity;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.PlayedGame;
using UI.Transformations;
using UI.Transformations.Player;
using UniversalAnalyticsHttpWrapper;
namespace UI.DependencyResolution {
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
                        {
                            x.Scan(scan =>
                                    {
                                        scan.TheCallingAssembly();
                                        scan.WithDefaultConventions();
                                    });

                            //unique per request scope
                            x.For<DbContext>().LifecycleIs(new UniquePerRequestLifecycle()).Use<NemeStatsDbContext>();
                            x.For<IDataContext>().LifecycleIs(new UniquePerRequestLifecycle()).Use<NemeStatsDataContext>();

                            x.For<IGamingGroupCreator>().Use<GamingGroupCreator>();
                            x.For<IGamingGroupAccessGranter>().Use<EntityFrameworkGamingGroupAccessGranter>();
                            x.For<IGamingGroupInviteConsumer>().Use<GamingGroupInviteConsumer>();

                            //transient scope
                            x.For<IPlayerSummaryBuilder>().Use<PlayerSummaryBuilder>();
                            x.For<IPlayerRepository>().Use<EntityFrameworkPlayerRepository>();
                            x.For<IGameDefinitionRetriever>().Use<GameDefinitionRetriever>();
                            x.For<IPlayedGameRetriever>().Use<PlayedGameRetriever>();
                            x.For<Microsoft.AspNet.Identity.IUserStore<ApplicationUser>>()
                                .Use<Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>>();
                            x.For<IGameDefinitionViewModelBuilder>()
                                .Use<GameDefinitionViewModelBuilder>();
                            x.For<IShowingXResultsMessageBuilder>().Use<ShowingXResultsMessageBuilder>();
                            x.For<IGamingGroupRetriever>().Use<GamingGroupRetriever>();
                            x.For<IPendingGamingGroupInvitationRetriever>().Use<PendingGamingGroupInvitationRetriever>();
                            x.For<IPlayedGameCreator>().Use<PlayedGameCreator>();
                            x.For<NemeStatsEventTracker>().Use<UniversalAnalyticsNemeStatsEventTracker>();
                            x.For<IEventTracker>().Use<EventTracker>();
                            x.For<IUniversalAnalyticsEvent>().Use<UniversalAnalyticsEvent>();
                            x.For<IUniversalAnalyticsEventFactory>().Use<UniversalAnalyticsEventFactory>();
                            x.For<IConfigurationManager>().Use<ConfigurationManager>();
                            x.For<IPlayerSaver>().Use<PlayerSaver>();
                            x.For<IGameDefinitionSaver>().Use<GameDefinitionSaver>();
                            x.For<IPlayerRetriever>().Use<PlayerRetriever>();

                            //singleton scope
                            x.For<IGameResultViewModelBuilder>().Singleton().Use<GameResultViewModelBuilder>();
                            x.For<IPlayerDetailsViewModelBuilder>().Singleton().Use<PlayerDetailsViewModelBuilder>();
                            x.For<IGamingGroupViewModelBuilder>().Singleton()
                                .Use<GamingGroupViewModelBuilder>();
                            x.For<IGamingGroupInvitationViewModelBuilder>().Singleton()
                                .Use<GamingGroupInvitationViewModelBuilder>();
                            x.For<ITopPlayerViewModelBuilder>().Use<TopPlayerViewModelBuilder>();
                            x.For<IPlayedGameDetailsViewModelBuilder>().Singleton().Use<PlayedGameDetailsViewModelBuilder>();
                        });
            return ObjectFactory.Container;
        }
    }
}