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
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class AdminSaveTests : NemeStatsDataContextTestBase
    {
        [Test]
        public void It_Throws_An_ArgumentNullException_If_The_Entity_Is_Null()
        {
            Exception expectedException = new ArgumentNullException("entity");

            Exception actualException = Assert.Throws<ArgumentNullException>(() => dataContext.AdminSave<GamingGroup>(null));

            expectedException.Message.ShouldBe(actualException.Message);
        }

        internal class FakeSecuredEntity : SecuredEntityWithTechnicalKey, IEntityWithTechnicalKey
        {
            public int GamingGroupId { get; set; }
            public bool AlreadyInDatabase()
            {
                throw new NotImplementedException();
            }

            public object GetIdAsObject()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void It_Throws_An_Exception_If_The_Entity_Is_A_Secured_Entity_Without_A_GamingGroupId_Set()
        {
            Exception expectedException = new ArgumentException("GamingGroupId must be set on an ISecuredEntityWithTechnicalKey");

            Exception actualException = Assert.Throws<ArgumentException>(() => dataContext.AdminSave(new FakeSecuredEntity()));
       
            expectedException.Message.ShouldBe(actualException.Message);
        }

        [Test]
        public void It_Adds_Or_Saves_The_Entity_And_Returns_It()
        {
            //--arrange
            entityWithGamingGroup.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            dataContext.Expect(mock => mock.AddOrInsertOverride(entityWithGamingGroup))
                .Repeat.Once()
                .Return(entityWithGamingGroup);

            //--act
            var actualResult = dataContext.AdminSave(entityWithGamingGroup);

            //--assert
            dataContext.AssertWasCalled(mock => mock.AddOrInsertOverride(entityWithGamingGroup));
            dataContext.AssertWasCalled(mock => mock.CommitAllChanges());
            actualResult.ShouldBeSameAs(entityWithGamingGroup);
        }
    }
}
