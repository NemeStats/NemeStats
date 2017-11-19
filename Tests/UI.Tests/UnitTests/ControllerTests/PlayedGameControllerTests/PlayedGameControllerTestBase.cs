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

using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using StructureMap.AutoMocking;
using UI.Controllers;
using UI.Mappers.Interfaces;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	public class PlayedGameControllerTestBase
	{
	    protected RhinoAutoMocker<PlayedGameController> AutoMocker; 
		protected string TestUserName = "the test user name";
		protected ApplicationUser CurrentUser;
	    protected const int EXPECTED_GAMING_GROUP_ID = 1;
		protected List<GameDefinitionSummary> GameDefinitionSummaries;
		protected List<PublicGameSummary> ExpectedViewModel;
		protected PlayedGameEditViewModel ExpectedPopulatedCompletedGameViewModel;
		protected List<Player> PlayerList;
		protected List<SelectListItem> PlayerSelectList;
		protected List<GameDefinition> GameDefinitionList;
		protected List<SelectListItem> GameDefinitionSelectList;

		[SetUp]
		public virtual void TestSetUp()
		{
            AutoMocker = new RhinoAutoMocker<PlayedGameController>();
            AutoMocker.PartialMockTheClassUnderTest();

            CurrentUser = new ApplicationUser()
			{
				CurrentGamingGroupId = EXPECTED_GAMING_GROUP_ID
            };
			GameDefinitionSummaries = new List<GameDefinitionSummary>();
            AutoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitions(EXPECTED_GAMING_GROUP_ID))
				.Repeat.Once()
				.Return(GameDefinitionSummaries);

		    AutoMocker.ClassUnderTest.Url = MockRepository.GenerateMock<UrlHelper>();

            var expectedMapper = MockRepository.GenerateMock<ICustomMapper<SavePlayedGameRequest, NewlyCompletedGame>>();
            AutoMocker.Get<IMapperFactory>()
                .Expect(mock => mock.GetMapper<SavePlayedGameRequest, NewlyCompletedGame>())
                .Return(expectedMapper);
            expectedMapper.Expect(mock => mock.Map(Arg<SavePlayedGameRequest>.Is.Anything)).Return(new NewlyCompletedGame());

            var expectedUpdateGameMapper = MockRepository.GenerateMock<ICustomMapper<SavePlayedGameRequest, UpdatedGame>>();
            AutoMocker.Get<IMapperFactory>()
                .Expect(mock => mock.GetMapper<SavePlayedGameRequest, UpdatedGame>())
                .Return(expectedUpdateGameMapper);
            expectedUpdateGameMapper.Expect(mock => mock.Map(Arg<SavePlayedGameRequest>.Is.Anything)).Return(new UpdatedGame());
        }
	}
}
