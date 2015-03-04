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
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using UI.Controllers;
using UI.Transformations.PlayerTransformations;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class HomeControllerTestBase
    {
        protected HomeController homeControllerPartialMock;
        protected IPlayerSummaryBuilder playerSummaryBuilderMock;
        protected ITopPlayerViewModelBuilder topPlayerViewModelBuilderMock;
        protected IPlayedGameRetriever playedGameRetrieverMock;
        protected INemesisHistoryRetriever nemesisHistoryRetrieverMock;
        protected INemesisChangeViewModelBuilder nemesisChangeViewModelBuilderMock;
        protected IGamingGroupRetriever gamingGroupRetrieverMock;

        [SetUp]
        public virtual void SetUp()
        {
            AutomapperConfiguration.Configure();
            playerSummaryBuilderMock = MockRepository.GenerateMock<IPlayerSummaryBuilder>();
            topPlayerViewModelBuilderMock = MockRepository.GenerateMock<ITopPlayerViewModelBuilder>();
            playedGameRetrieverMock = MockRepository.GenerateMock<IPlayedGameRetriever>();
            nemesisHistoryRetrieverMock = MockRepository.GenerateMock<INemesisHistoryRetriever>();
            nemesisChangeViewModelBuilderMock = MockRepository.GenerateMock<INemesisChangeViewModelBuilder>();
            gamingGroupRetrieverMock = MockRepository.GenerateMock<IGamingGroupRetriever>();
            homeControllerPartialMock = MockRepository.GeneratePartialMock<HomeController>(
                playerSummaryBuilderMock, 
                topPlayerViewModelBuilderMock,
                playedGameRetrieverMock,
                nemesisHistoryRetrieverMock,
                nemesisChangeViewModelBuilderMock,
                gamingGroupRetrieverMock);
        }
    }
}
