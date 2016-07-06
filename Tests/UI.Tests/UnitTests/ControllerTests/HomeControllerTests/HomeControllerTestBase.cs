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

using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using StructureMap;
using StructureMap.AutoMocking;
using UI.Controllers;
using UI.Mappers;
using UI.Mappers.CustomMappers;
using UI.Mappers.Interfaces;
using UI.Models.Achievements;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class HomeControllerTestBase
    {
        protected RhinoAutoMocker<HomeController> _autoMocker; 

        [SetUp]
        public virtual void SetUp()
        {
            AutomapperConfiguration.Configure();
            _autoMocker = new RhinoAutoMocker<HomeController>();

            _autoMocker.Inject<IMapperFactory>(new MapperFactory(new Container(c =>
            {
                c.AddRegistry<TestRegistry>();
            })));
        }
    }
}
