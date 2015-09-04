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

using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using BusinessLogic.Logic.VotableFeatures;

namespace BusinessLogic.Tests.UnitTests.LogicTests.VotableFeaturesTests
{
    [TestFixture]
    public class RetrieveVotableFeatureTests
    {
        private RhinoAutoMocker<VotableFeatureRetriever> autoMocker;
            
        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<VotableFeatureRetriever>(MockMode.AAA);
        }

        [Test]
        public void ItRetrievesTheGivenVotableFeature()
        {
            string featureId = "some feature id";
            VotableFeature expectedVotableFeature = new VotableFeature();
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<VotableFeature>(featureId))
                .Return(expectedVotableFeature);

            var actualResult = autoMocker.ClassUnderTest.RetrieveVotableFeature(featureId);

            Assert.That(expectedVotableFeature, Is.SameAs(actualResult));
        }
    }
}
