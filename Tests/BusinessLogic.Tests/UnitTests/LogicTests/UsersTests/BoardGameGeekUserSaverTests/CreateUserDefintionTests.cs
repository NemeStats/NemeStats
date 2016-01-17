using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Core;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.BoardGameGeekUserSaverTests
{
    public class CreateUserDefintionTests : BoardGameGeekUserSaverTestBase
    {

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfTheModelIsNull()
        {
            var expectedException = new ArgumentNullException(typeof(CreateBoardGameGeekUserDefinitionRequest).ToString());

            Exception exception = Assert.Throws<ArgumentNullException>(() => autoMocker.ClassUnderTest.CreateUserDefintion(null, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }


        [Test]
        public void ItThrowsAnArgumentExceptionIfTheNameIsNull()
        {
            Exception exception = Assert.Throws<ArgumentException>(() => autoMocker.ClassUnderTest.CreateUserDefintion(new CreateBoardGameGeekUserDefinitionRequest() { Name = null, BoardGameGeekUserId = 1}, currentUser));

            Assert.IsTrue(exception.Message.Contains(typeof(CreateBoardGameGeekUserDefinitionRequest).ToString()));
        }

        [Test]
        public void ItSavesANewGameDefinition()
        {
            var request = new CreateBoardGameGeekUserDefinitionRequest
            {
                BoardGameGeekUserId = 1,
                Name = "game definition name",
                Avatar = "avatar.png"
            };
            autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<BoardGameGeekUserDefinition>()).Return(new List<BoardGameGeekUserDefinition>().AsQueryable());

            autoMocker.ClassUnderTest.CreateUserDefintion(request, currentUser);

            var args = autoMocker.Get<IDataContext>().GetArgumentsForCallsMadeOn(mock => mock.Save(
                Arg<BoardGameGeekUserDefinition>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
            var actualUserDefinition = args[0][0] as BoardGameGeekUserDefinition;
            Assert.That(actualUserDefinition, Is.Not.Null);
            Assert.That(actualUserDefinition.Id, Is.EqualTo(request.BoardGameGeekUserId));
            Assert.That(actualUserDefinition.Name, Is.EqualTo(request.Name));
            Assert.That(actualUserDefinition.Avatar, Is.EqualTo(request.Avatar));

        }
    }
}
