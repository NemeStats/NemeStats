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
using BusinessLogic.Logic;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using StructureMap;
using StructureMap.Graph;
using System.Data.Entity;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Filters;
using UI.Models.PlayedGame;
using UI.Transformations;
using UI.Transformations.Player;
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
                            //TODO MAKE THIS PER REQUEST
                            x.For<DbContext>().Use<NemeStatsDbContext>();
                            x.For<DataContext>().Use<NemeStatsDataContext>();
                            x.For<PlayerRepository>().Use<EntityFrameworkPlayerRepository>();
                            x.For<GameDefinitionRetriever>().Use<GameDefinitionRetrieverImpl>();
                            x.For<PlayedGameRepository>().Use<EntityFrameworkPlayedGameRepository>();
                            x.For<PlayedGameDetailsViewModelBuilder>().Use<PlayedGameDetailsViewModelBuilderImpl>();
                            x.For<GameResultViewModelBuilder>().Use<GameResultViewModelBuilderImpl>();
                            x.For<PlayerDetailsViewModelBuilder>().Use<PlayerDetailsViewModelBuilderImpl>();
                            x.For<GameDefinitionRepository>().Use<EntityFrameworkGameDefinitionRepository>();
                            x.For<GamingGroupToGamingGroupViewModelTransformation>()
                                .Use<GamingGroupToGamingGroupViewModelTransformationImpl>();
                            x.For<GamingGroupInvitationToInvitationViewModelTransformation>()
                                .Use<GamingGroupInvitationToInvitationViewModelTransformationImpl>();
                            x.For<GamingGroupAccessGranter>().Use<EntityFrameworkGamingGroupAccessGranter>();
                            x.For<GamingGroupInviteConsumer>().Use<GamingGroupInviteConsumerImpl>();
                            x.For<Microsoft.AspNet.Identity.IUserStore<ApplicationUser>>()
                                .Use<Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>>();
                            x.For<GamingGroupCreator>().Use<GamingGroupCreatorImpl>();
                            x.For<GameDefinitionToGameDefinitionViewModelTransformation>()
                                .Use<GameDefinitionToGameDefinitionViewModelTransformationImpl>();
                            x.For<ShowingXResultsMessageBuilder>().Use<ShowingXResultsMessageBuilderImpl>();
                            x.For<GamingGroupRetriever>().Use<GamingGroupRetrieverImpl>();
                            x.For<PendingGamingGroupInvitationRetriever>().Use<PendingGamingGroupInvitationRetrieverImpl>();
                            //TODO finish implementing http://lostechies.com/jimmybogard/2010/05/03/dependency-injection-in-asp-net-mvc-filters/
                            //x.For<IActionInvoker>().Use<InjectingActionInvoker>();
                        });
            return ObjectFactory.Container;
        }
    }
}