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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using NUnit.Framework;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.TransformationsTests.PlayerTransformationTests
{
    public class PlayerEditViewModelBuilderTests
    {
        private Player player;
        private PlayerEditViewModel actualModel;

        [SetUp]
        public void SetUp()
        {
            player = new Player
            {
                Active = false,
                Id = 123,
                Name = "the name",
                GamingGroupId = 789
            };
            actualModel = new PlayerEditViewModelBuilder().Build(player);
        }

        [Test]
        public void ItCopiesTheActiveFlag()
        {
            Assert.That(actualModel.Active, Is.EqualTo(player.Active));
        }

        [Test]
        public void ItCopiesTheId()
        {
            Assert.That(actualModel.Id, Is.EqualTo(player.Id));
        }

        [Test]
        public void ItCopiesTheName()
        {
            Assert.That(actualModel.Name, Is.EqualTo(player.Name));
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.That(actualModel.GamingGroupId, Is.EqualTo(player.GamingGroupId));
        }
    }
}
