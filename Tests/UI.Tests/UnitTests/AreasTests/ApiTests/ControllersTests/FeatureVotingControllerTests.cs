using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.FeatureVoting;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Areas.Api.Controllers;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    [TestFixture]
    public class FeatureVotingControllerTests
    {
        private FeatureVotingController featureVotingController;
        private IFeatureInterestSummaryRetriever featureInterestSummaryRetriever;

        [SetUp]
        public void SetUp()
        {
            featureInterestSummaryRetriever = MockRepository.GenerateMock<IFeatureInterestSummaryRetriever>();
            featureVotingController = MockRepository.GeneratePartialMock<FeatureVotingController>(featureInterestSummaryRetriever);
            var autoMocker = new RhinoAutoMocker<FeatureVotingController>(MockMode.AAA);
        }


        [Test]
        public void ItReturnsTheFeatureInterestSummaryForTheGivenFeature()
        {
            //FeatureInterestInfo actualFeatureInterestSummary = featureVotingController.GetFeatureInterestInfo("some feature");

            //Assert.That(actualFeatureInterestSummary, Is.EqualTo());
        }
    }
}
