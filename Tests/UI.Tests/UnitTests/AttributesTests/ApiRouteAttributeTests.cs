using System;
using System.Configuration;
using System.Configuration.Abstractions;
using System.Linq;
using System.Web.Routing;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Attributes;

namespace UI.Tests.UnitTests.AttributesTests
{
    [TestFixture]
    public class ApiRouteAttributeTests
    {
        [Test]
        public void ConstructorThrowsAnArgumentNullExceptionIfTheTemplateIsNull()
        {
            ArgumentNullException expectedException = new ArgumentNullException("template");
            Exception exception = Assert.Throws<ArgumentNullException>(() => new ApiRouteAttribute(null));

            Assert.That(exception.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public void ConstructorThrowsAnArgumentNullExceptionIfTheTemplateIsWhitespace()
        {
            ArgumentNullException expectedException = new ArgumentNullException("template");
            Exception exception = Assert.Throws<ArgumentNullException>(() => new ApiRouteAttribute("   "));

            Assert.That(exception.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public void ConstructorThrowsAnArgumentExceptionIfTheTemplateStartsWithAForwardSlash()
        {
            Exception exception = Assert.Throws<ArgumentException>(() => new ApiRouteAttribute("/GamingGroups"));

            Assert.That(exception.Message, Is.EqualTo("The route cannot start with a forward slash ('/') since it will be prefixed with the api version (e.g. api/v2/)."));
        }

        [Test]
        public void BuildRouteThrowsAConfigurationErrorsExceptionIfTheCurrentApiVersionAppSettingIsMissing()
        {
            ApiRouteAttribute routeAttribute = new ApiRouteAttribute("GamingGroups");
            Exception exception = Assert.Throws<ConfigurationErrorsException>(() => routeAttribute.BuildRoute());

            Assert.That(exception.Message, Is.EqualTo("The config file must have an appSetting with key = \"currentApiVersion\". "));
        }

        [Test]
        public void BuildRouteMakesARouteSupportingAllVersionsUpToTheCurrentVersion()
        {
            string currentVersion = "3";
            IConfigurationManager configurationManager = MockRepository.GenerateMock<IConfigurationManager>();
            IAppSettings appSettings = MockRepository.GenerateMock<IAppSettings>();
            appSettings.Expect(mock => mock.Get(ApiRouteAttribute.APP_KEY_CURRENT_API_VERSION)).Return(currentVersion);
            configurationManager.Expect(mock => mock.AppSettings).Return(appSettings);
            ApiRouteAttribute routeAttribute = new ApiRouteAttribute("GamingGroups")
            {
                ConfigurationManager = configurationManager
            };

            string actualRoute = routeAttribute.BuildRoute();

            Assert.That(actualRoute, Is.EqualTo("api/v(1|2|3)/GamingGroups"));
        }
    }
}
