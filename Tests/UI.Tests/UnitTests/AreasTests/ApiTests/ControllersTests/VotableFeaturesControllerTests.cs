using System;
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
using UI.Models.API;
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
            autoMocker.Get<IVotableFeatureRetriever>().Expect(mock => mock.RetrieveVotableFeature(Arg<string>.Is.Anything))
                      .Throw(new EntityDoesNotExistException(""));

            HttpResponseException actualException = Assert.Throws<HttpResponseException>(() => autoMocker.ClassUnderTest.Get("feature that doesn't exist"));
            Assert.That(HttpStatusCode.NotFound, Is.EqualTo(actualException.Response.StatusCode));
        }

        [Test]
        public void Post_CastsAVoteForTheGivenFeature()
        {
            FeatureVote featureVote = new FeatureVote
            {
                VotableFeatureId = "some feature id",
                VoteUp = true
            };

            autoMocker.ClassUnderTest.Post(featureVote);

            autoMocker.Get<IVotableFeatureVoter>().AssertWasCalled(
                mock => mock.CastVote(
                    Arg<String>.Is.Equal(featureVote.VotableFeatureId), 
                    Arg<bool>.Is.Equal(featureVote.VoteUp)));
        }

        [Test]
        public void Post_ThrowsNotFoundHttpExceptionIfTheFeatureDoesntExist()
        {
            autoMocker.Get<IVotableFeatureVoter>().Expect(mock => mock.CastVote(Arg<string>.Is.Anything, Arg<bool>.Is.Anything))
                      .Throw(new EntityDoesNotExistException(""));

            HttpResponseException actualException = Assert.Throws<HttpResponseException>(() => autoMocker.ClassUnderTest.Post(new FeatureVote()));
            Assert.That(HttpStatusCode.NotFound, Is.EqualTo(actualException.Response.StatusCode));
        }
    }
}
