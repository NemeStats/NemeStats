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
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Transformations;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GamingGroupControllerTestBase
    {
        protected GamingGroupController gamingGroupControllerPartialMock;
        protected IDataContext dataContext;
        protected IGamingGroupViewModelBuilder gamingGroupViewModelBuilderMock;
        protected IGamingGroupAccessGranter gamingGroupAccessGranterMock;
        protected IGamingGroupSaver gamingGroupSaverMock;
        protected IGamingGroupRetriever gamingGroupRetrieverMock;
        protected IShowingXResultsMessageBuilder showingXResultsMessageBuilderMock;
        protected IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilderMock;
        protected IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilderMock;
        protected IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilderMock;
        protected IGamingGroupContextSwitcher gamingGroupContextSwitcherMock;
        protected ICookieHelper cookieHelperMock;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            dataContext = MockRepository.GenerateMock<IDataContext>();
            gamingGroupViewModelBuilderMock = MockRepository.GenerateMock<IGamingGroupViewModelBuilder>();
            gamingGroupAccessGranterMock = MockRepository.GenerateMock<IGamingGroupAccessGranter>();
            gamingGroupSaverMock = MockRepository.GenerateMock<IGamingGroupSaver>();
            gamingGroupRetrieverMock = MockRepository.GenerateMock<IGamingGroupRetriever>();
            showingXResultsMessageBuilderMock = MockRepository.GenerateMock<IShowingXResultsMessageBuilder>();
            playerWithNemesisViewModelBuilderMock = MockRepository.GenerateMock<IPlayerWithNemesisViewModelBuilder>();
            playedGameDetailsViewModelBuilderMock = MockRepository.GenerateMock<IPlayedGameDetailsViewModelBuilder>();
            gameDefinitionSummaryViewModelBuilderMock = MockRepository.GenerateMock<IGameDefinitionSummaryViewModelBuilder>();
            gamingGroupContextSwitcherMock = MockRepository.GenerateMock<IGamingGroupContextSwitcher>();
            cookieHelperMock = MockRepository.GenerateMock<ICookieHelper>();
            gamingGroupControllerPartialMock = MockRepository.GeneratePartialMock<GamingGroupController>(
                gamingGroupViewModelBuilderMock, 
                gamingGroupAccessGranterMock,
                gamingGroupSaverMock,
                gamingGroupRetrieverMock,
                showingXResultsMessageBuilderMock,
                playerWithNemesisViewModelBuilderMock,
                playedGameDetailsViewModelBuilderMock,
                gameDefinitionSummaryViewModelBuilderMock,
                gamingGroupContextSwitcherMock,
                cookieHelperMock);
            currentUser = new ApplicationUser()
            {
                Id = "user  id",
                CurrentGamingGroupId = 1315
            };
        }
    }
}
