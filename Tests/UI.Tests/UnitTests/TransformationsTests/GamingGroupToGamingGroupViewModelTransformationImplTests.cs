using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.GamingGroup;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class GamingGroupToGamingGroupViewModelTransformationImplTests
    {
        private GamingGroupToGamingGroupViewModelTransformationImpl transformer;
        private GamingGroup gamingGroup;
        private GamingGroupViewModel viewModel;

        [SetUp]
        public void SetUp()
        {
            transformer = new GamingGroupToGamingGroupViewModelTransformationImpl();
            ApplicationUser owningUser = new ApplicationUser()
            {
                Id = "owning user user Id",
                Email = "owninguser@email.com",
                UserName = "username"
            };
            gamingGroup = new GamingGroup()
            {
                Id = 1,
                Name = "gaming group",
                OwningUserId = owningUser.Id,
                OwningUser = owningUser
            };

            viewModel = transformer.Build(gamingGroup);
        }

        [Test]
        public void ItCopiesTheGamingGroupId()
        {
            Assert.AreEqual(gamingGroup.Id, viewModel.Id);
        }

        [Test]
        public void ItCopiesTheOwningUserId()
        {
            Assert.AreEqual(gamingGroup.OwningUserId, viewModel.OwningUserId);
        }

        [Test]
        public void ItCopiesTheGamingGroupName()
        {
            Assert.AreEqual(gamingGroup.Name, viewModel.Name);
        }

        [Test]
        public void ItCopiesTheOwningUserName()
        {
            Assert.AreEqual(gamingGroup.OwningUser.UserName, viewModel.OwningUserName);
        }
    }
}
