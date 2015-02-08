using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.VotableFeatures;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;
using BusinessLogic.Models.VotableFeatures;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    [TestFixture]
    public class VotableFeaturesControllerTests
    {
        private RhinoAutoMocker<VotableFeaturesController> autoMocker;
        //private VotableFeatureController featureVotingController;
        //private IVotableFeatureRetriever featureInterestSummaryRetriever;

        [SetUp]
        public void SetUp()
        {
            //featureInterestSummaryRetriever = MockRepository.GenerateMock<IVotableFeatureRetriever>();
            //featureVotingController = MockRepository.GeneratePartialMock<VotableFeatureController>(featureInterestSummaryRetriever);
            //var automocker = new RhinoAutoMocker<VotableFeatureController>(MockMode.AAA);
        }

        [Test]
        public void ShowThatAutoMockerIsBlowingUp()
        {
            var mocker = new RhinoAutoMocker<VotableFeaturesController>(MockMode.AAA);
        }


        [Test]
        public void ItReturnsTheFeatureInterestSummaryForTheGivenFeature()
        {
            string featureId = "the feature id";
            VotableFeature votableFeature = autoMocker.ClassUnderTest.Get(featureId);
            //FeatureInterestInfo actualFeatureInterestSummary = featureVotingController.GetFeatureInterestInfo("some feature");

            //Assert.That(actualFeatureInterestSummary, Is.EqualTo());
        }
    }
}
