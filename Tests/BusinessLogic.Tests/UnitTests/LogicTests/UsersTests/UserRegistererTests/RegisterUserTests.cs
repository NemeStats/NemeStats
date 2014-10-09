using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.UserRegistererTests
{
    [TestFixture]
    public class RegisterUserTests
    {
        private IFirstTimeAuthenticator firstTimeUserAuthenticatorMock;
        private IUserRegisterer userRegisterer;
        private IUserStore<ApplicationUser> userStoreMock;
        private ApplicationUserManager applicationUserManagerMock;

        [SetUp]
        public void SetUp()
        {
            firstTimeUserAuthenticatorMock = MockRepository.GenerateMock<IFirstTimeAuthenticator>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
            userRegisterer = new UserRegisterer(applicationUserManagerMock, firstTimeUserAuthenticatorMock);
        }

        [Test]
        public async Task ItCreatesANewUser()
        {
            NewUser newUser = new NewUser();
            IdentityResult result = new IdentityResult();
            applicationUserManagerMock.Expect(mock => mock.CreateAsync(Arg<ApplicationUser>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult<IdentityResult>(result));
            firstTimeUserAuthenticatorMock.Expect(mock => mock.SignInAndCreateGamingGroup(Arg<ApplicationUser>.Is.Anything))
                .Return(Task.FromResult(new object()));

            await userRegisterer.RegisterUser(newUser);

            applicationUserManagerMock.AssertWasCalled(mock => mock.CreateAsync(
                Arg<ApplicationUser>.Matches(appUser => appUser.UserName == newUser.UserName
                    && appUser.Email == newUser.Email),
                Arg<string>.Is.Equal(newUser.Password)));
        }

        //TODO this is failing because I can't figure out how to create an IdentityResult with .Success == true
        [Test]
        public async Task ItSignsInTheUserAndCreatesANewGamingGroup()
        {
            NewUser newUser = new NewUser()
            {
                UserName = "user name",
                Email = "the email"
            };
            IdentityResult result = MockRepository.GenerateMock<IdentityResult>(new string[] { });
            result.Expect(res => res.Succeeded)
                .Return(true);
            applicationUserManagerMock.Expect(mock => mock.CreateAsync(Arg<ApplicationUser>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult<IdentityResult>(result));
            firstTimeUserAuthenticatorMock.Expect(mock => mock.SignInAndCreateGamingGroup(Arg<ApplicationUser>.Is.Anything))
                .Return(Task.FromResult(new object()));

            await userRegisterer.RegisterUser(newUser);

            firstTimeUserAuthenticatorMock.AssertWasCalled(mock => mock.SignInAndCreateGamingGroup(
                Arg<ApplicationUser>.Matches(user => user.UserName == newUser.UserName
                                                && user.Email == newUser.Email)));
        }

        [Test]
        public async Task ItDoesntSignInIfTheUserIsntCreatedSuccessfully()
        {
            NewUser newUser = new NewUser();
            IdentityResult result = new IdentityResult("an error");
            applicationUserManagerMock.Expect(mock => mock.CreateAsync(Arg<ApplicationUser>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult<IdentityResult>(result));
            firstTimeUserAuthenticatorMock.Expect(mock => mock.SignInAndCreateGamingGroup(Arg<ApplicationUser>.Is.Anything))
                .Return(Task.FromResult(new object()));

            await userRegisterer.RegisterUser(newUser);

            firstTimeUserAuthenticatorMock.AssertWasNotCalled(mock => mock.SignInAndCreateGamingGroup(
                Arg<ApplicationUser>.Is.Anything));
        }
    }
}
