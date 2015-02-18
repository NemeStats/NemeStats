using System.Net;
using System.Web.Http;
using AutoMapper;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;
using UI.Models;
using UI.Transformations;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests
{
    [TestFixture]
    public class VotableFeaturesControllerTests
    {
        private RhinoAutoMocker<VotableFeaturesController> autoMocker;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<VotableFeaturesController>(MockMode.AAA);
            AutomapperConfiguration.Configure();
        }

        [Test]
        public void Get_ReturnsTheFeatureInterestSummaryForTheGivenFeature()
        {
            string featureId = "some feature id";
            VotableFeature expectedVotableFeature = new VotableFeature()
            {
                Id = featureId
            };
            autoMocker.Get<IVotableFeatureRetriever>().Expect(mock => mock.RetrieveVotableFeature(featureId))
                      .Return(expectedVotableFeature);
            VotableFeatureViewModel expectedVotableFeatureViewModel = Mapper.Map<VotableFeature, VotableFeatureViewModel>(expectedVotableFeature);

            autoMocker.ClassUnderTest.Get(featureId);

            Assert.That(expectedVotableFeatureViewModel.Id, Is.EqualTo(featureId));
        }

        [Test]
        public void Get_ThrowsNotFoundHttpExceptionIfTheFeatureDoesntExist()
        {
            string featureIdThatDoesntExist = "some non-existent feature id";
            autoMocker.Get<IVotableFeatureRetriever>().Expect(mock => mock.RetrieveVotableFeature(featureIdThatDoesntExist))
                      .Throw(new EntityDoesNotExistException(""));

            HttpResponseException actualException = Assert.Throws<HttpResponseException>(() => autoMocker.ClassUnderTest.Get(featureIdThatDoesntExist));
            Assert.That(HttpStatusCode.NotFound, Is.EqualTo(actualException.Response.StatusCode));
        }
    }
}
