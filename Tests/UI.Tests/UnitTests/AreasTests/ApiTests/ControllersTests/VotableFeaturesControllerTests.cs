using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    [TestFixture]
    public class VotableFeaturesControllerTests
    {
        //private RhinoAutoMocker<VotableFeaturesController> autoMocker;
        //private VotableFeatureController featureVotingController;
        //private IVotableFeatureRetriever featureInterestSummaryRetriever;

        //[SetUp]
        //public void SetUp()
        //{
        //    //featureInterestSummaryRetriever = MockRepository.GenerateMock<IVotableFeatureRetriever>();
        //    //featureVotingController = MockRepository.GeneratePartialMock<VotableFeatureController>(featureInterestSummaryRetriever);
        //    //var automocker = new RhinoAutoMocker<VotableFeatureController>(MockMode.AAA);
        //}

        [Test]
        public void ShowThatAutoMockerIsBlowingUp()
        {
            var x = new RhinoAutoMocker<VotableFeatureRetriever>(MockMode.AAA);
        }


        //[Test]
        //public void ItReturnsTheFeatureInterestSummaryForTheGivenFeature()
        //{
        //    VotableFeature expectedVotableFeature = new VotableFeature();
        //    int featureId = 1;
        //    autoMocker.Get<IVotableFeatureRetriever>().Expect(mock => mock.RetrieveVotableFeature(featureId))
        //              .Return(expectedVotableFeature);
        //    VotableFeature votableFeature = autoMocker.ClassUnderTest.Get(featureId);
        //    //FeatureInterestInfo actualFeatureInterestSummary = featureVotingController.GetFeatureInterestInfo("some feature");

        //    //Assert.That(actualFeatureInterestSummary, Is.EqualTo());
        //}
    }
}
