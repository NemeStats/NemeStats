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

using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GamingGroupsTests.GamingGroupSaverTests
{
    [TestFixture]
    public class CreateGamingGroupAsyncTests : GamingGroupSaverTestBase
    {
        protected GamingGroup expectedGamingGroup;
        protected Player expectedPlayer;
        protected UserGamingGroup expectedUserGamingGroup;
        protected ApplicationUser appUserRetrievedFromFindMethod;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            expectedGamingGroup = new GamingGroup() { Id = currentUser.CurrentGamingGroupId.Value };
            autoMocker.Get<IDataContext>().Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(expectedGamingGroup);
            expectedPlayer = new Player();
            autoMocker.Get<IPlayerSaver>().Expect(mock => mock.Save(Arg<Player>.Is.Anything, Arg<ApplicationUser>.Is.Anything)).Return(expectedPlayer);
            
            expectedUserGamingGroup = new UserGamingGroup
            {
                ApplicationUserId = currentUser.Id,
                GamingGroupId = expectedGamingGroup.Id
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.Save<UserGamingGroup>(Arg<UserGamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                           .Return(expectedUserGamingGroup);

            appUserRetrievedFromFindMethod = new ApplicationUser()
            {
                Id = currentUser.Id
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<ApplicationUser>(Arg<ApplicationUser>.Is.Anything))
                .Return(appUserRetrievedFromFindMethod);
            autoMocker.Get<IDataContext>().Expect(mock => mock.Save(Arg<ApplicationUser>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                 .Return(new ApplicationUser());
        }

        [Test]
        public void ItSetsTheOwnerToTheCurrentUser()
        {
            autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, TransactionSource.RestApi, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.OwningUserId == currentUser.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheGamingGroupName()
        {
            autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, TransactionSource.RestApi, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock =>
                mock.Save(Arg<GamingGroup>.Matches(group => group.Name == gamingGroupName),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItReturnsTheNewlyCreatedGamingGroupResult()
        {
            NewlyCreatedGamingGroupResult actualResult = autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, TransactionSource.RestApi, currentUser);

            Assert.AreSame(expectedGamingGroup, actualResult.NewlyCreatedGamingGroup);
            Assert.AreSame(expectedPlayer, actualResult.NewlyCreatedPlayer);
        }

        [Test]
        public void ItAssociatesTheUserWithTheGamingGroup()
        {
            autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, TransactionSource.RestApi, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save<UserGamingGroup>(
                Arg<UserGamingGroup>.Matches(ugg => ugg.ApplicationUserId == currentUser.Id
                    && ugg.GamingGroupId == expectedGamingGroup.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheCurrentUsersGamingGroup()
        {
            autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, TransactionSource.RestApi, currentUser);

            autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(Arg<ApplicationUser>.Matches(
                user => user.CurrentGamingGroupId == expectedGamingGroup.Id 
                    && user.Id == appUserRetrievedFromFindMethod.Id),
                    Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItCreatesANewPlayerNamedAfterTheUserName()
        {
            autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, TransactionSource.RestApi, currentUser);

            autoMocker.Get<IPlayerSaver>().AssertWasCalled(mock => mock.Save(Arg<Player>.Matches(
                                        player => player.Name == currentUser.UserName
                                            && player.ApplicationUserId == currentUser.Id
                                            && player.Active), 
                                            Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItTracksTheGamingGroupCreation()
        {
            GamingGroup expectedGamingGroup = new GamingGroup() { Id = 123 };
            autoMocker.Get<IDataContext>().Expect(mock => mock.Save(Arg<GamingGroup>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Repeat.Once()
                .Return(expectedGamingGroup);
            TransactionSource expectedRegistrationSource = TransactionSource.RestApi;

            autoMocker.ClassUnderTest.CreateNewGamingGroup(gamingGroupName, expectedRegistrationSource, currentUser);

            autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackGamingGroupCreation(expectedRegistrationSource));
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGamingGroupNameIsNull()
        {
            Exception exception = Assert.Throws<ArgumentException>(() => autoMocker.ClassUnderTest.CreateNewGamingGroup(null, TransactionSource.RestApi, currentUser));

            Assert.That(exception.Message, Is.EqualTo(GamingGroupSaver.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK));
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfTheGamingGroupNameIsWhitespace()
        {
            Exception exception = Assert.Throws<ArgumentException>(() => autoMocker.ClassUnderTest.CreateNewGamingGroup("    ", TransactionSource.RestApi, currentUser));

            Assert.That(exception.Message, Is.EqualTo(GamingGroupSaver.EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK));
        }
    }
}
