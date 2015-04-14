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
using UI.Areas.Api.Models;
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
            autoMocker.Get<IVotableFeatureRetriever>().Expect(mock => mock.RetrieveVotableFeature(Arg<string>.Is.Anything))
                      .Throw(new EntityDoesNotExistException<VotableFeature>(""));

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
                      .Throw(new EntityDoesNotExistException<VotableFeature>(""));

            HttpResponseException actualException = Assert.Throws<HttpResponseException>(() => autoMocker.ClassUnderTest.Post(new FeatureVote()));
            Assert.That(HttpStatusCode.NotFound, Is.EqualTo(actualException.Response.StatusCode));
        }
    }
}
