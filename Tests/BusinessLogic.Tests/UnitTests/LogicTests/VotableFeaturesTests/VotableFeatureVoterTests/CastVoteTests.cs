using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.VotableFeatures;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.VotableFeaturesTests.VotableFeatureVoterTests
{
    [TestFixture]
    public class CastVoteTests
    {
        private RhinoAutoMocker<VotableFeatureVoter> autoMocker;
        private int votableFeatureId = 1;
        private int startingNumberOfUpvotes = 10;
        private int startingNumberOfDownvotes = 200;
        private VotableFeature expectedVotableFeature;

        [SetUp]
        public void SetUp()
        {
            autoMocker = new RhinoAutoMocker<VotableFeatureVoter>(MockMode.AAA);
            expectedVotableFeature = new VotableFeature
            {
                NumberOfDownvotes = startingNumberOfDownvotes,
                NumberOfUpvotes = startingNumberOfUpvotes
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<VotableFeature>(votableFeatureId))
                      .Return(expectedVotableFeature);
        }

        [Test]
        public void ItIncreasesUpvotesAndNotDownvotesWhenCastingAnUpvote()
        {
            autoMocker.ClassUnderTest.CastVote(votableFeatureId, true);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<VotableFeature>.Matches(x => x.NumberOfUpvotes == startingNumberOfUpvotes + 1
                    && x.NumberOfDownvotes == startingNumberOfDownvotes), 
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItIncreasesDownvotesWhenCastingADownvote()
        {
            autoMocker.ClassUnderTest.CastVote(votableFeatureId, false);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                    Arg<VotableFeature>.Matches(x => x.NumberOfDownvotes == startingNumberOfDownvotes + 1
                     && x.NumberOfUpvotes == startingNumberOfUpvotes),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheDateModified()
        {
            DateTime beforeTest = DateTime.UtcNow;
            autoMocker.ClassUnderTest.CastVote(votableFeatureId, true);
            DateTime afterTest = DateTime.UtcNow;

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                    Arg<VotableFeature>.Matches(x => x.DateModified >= beforeTest && x.DateModified <= afterTest),
                Arg<ApplicationUser>.Is.Anything));
        }
    }
}
