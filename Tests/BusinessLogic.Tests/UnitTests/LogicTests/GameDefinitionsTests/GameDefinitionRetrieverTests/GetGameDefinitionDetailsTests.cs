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
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetGameDefinitionDetailsTests : GameDefinitionRetrieverTestBase
    {
        //TODO integration testing instead. Will delete this if it pans out.
        //private List<GameDefinition> gameDefinitionsToQuery;

        //[Test]
        //public override void SetUp()
        //{
        //    base.SetUp();

        //    gameDefinitionsToQuery = new List<GameDefinition>();
        //    GameDefinition gameDefinition = new GameDefinition()
        //    {
        //        Id = 3,
        //        Name = "game definition name",
        //        Description = "game definition description",
        //        PlayedGames = new List<PlayedGame>()
        //    };
        //    gameDefinition.PlayedGames.Add(new PlayedGame(){ Id = 1,  })
        //    gameDefinitionsToQuery.Add(gameDefinition);
        //}

        //[Test]
        //public void ItGetsTheGameDefinitionDetails()
        //{
        //    int id = 1;
       
        //    GameDefinition gameDefinition = retriever.GetGameDefinitionDetails(id, currentUser);
        //}
    }
}
