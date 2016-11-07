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
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    public class NemeStatsDataContextTestBase
    {
        protected NemeStatsDataContext dataContext;
        protected NemeStatsDbContext nemeStatsDbContext;
        protected SecuredEntityValidatorFactory securedEntityValidatorFactory;
        protected IEntityWithTechnicalKey entityWithGamingGroupAndTechnicalKey;
        protected IEntityWithTechnicalKey entityWithGamingGroup;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void TestBaseSetUp()
        {
            nemeStatsDbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            securedEntityValidatorFactory = MockRepository.GeneratePartialMock<SecuredEntityValidatorFactory>();
            dataContext = MockRepository.GeneratePartialMock<NemeStatsDataContext>(nemeStatsDbContext, securedEntityValidatorFactory);
           
            entityWithGamingGroupAndTechnicalKey = MockRepository.GenerateStub<IEntityWithTechnicalKey>();
            entityWithGamingGroupAndTechnicalKey.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            entityWithGamingGroup = MockRepository.GenerateStub<IEntityWithTechnicalKey>();

            currentUser = new ApplicationUser()
            {
                Id = "application user id",
                CurrentGamingGroupId = 1
            };
        }
    }
}
