using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
