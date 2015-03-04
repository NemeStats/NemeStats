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
using BusinessLogic.Models;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.NemesisTests
{
    [TestFixture]
    public class SameNemesisTests
    {
        [Test]
        public void NewObjectsAreTheSame()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis();

            Assert.True(nemesis1.SameNemesis(nemesis2));
        }

        [Test]
        public void ANullObjectIsNotTheSame()
        {
            Nemesis nemesis1 = new Nemesis();

            Assert.False(nemesis1.SameNemesis(null));
        }

        [Test]
        public void ItReturnsFalseIfTheNemesisPlayerIdIsDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { NemesisPlayerId = 1 };

            Assert.False(nemesis1.SameNemesis(nemesis2));
        }

        [Test]
        public void ItReturnsFalseIfTheMinionPlayerIdIsDifferent()
        {
            Nemesis nemesis1 = new Nemesis();
            Nemesis nemesis2 = new Nemesis() { MinionPlayerId = 1 };

            Assert.False(nemesis1.SameNemesis(nemesis2));
        }
    }
}
