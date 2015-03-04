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
        private string votableFeatureId = "some feature id";
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
