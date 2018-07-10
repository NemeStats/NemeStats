using System.Web.Mvc;
using NUnit.Framework;
using Shouldly;
using UI.HtmlHelpers;

namespace UI.Tests.UnitTests.HtmlHelperTests.UIHelpersTests
{
    [TestFixture()]
    public class RenderGravatarTests
    {
        HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());

        [SetUp]
        public void SetUp()
        {
            helper = new HtmlHelper(new ViewContext(), new ViewPage());
        }

        [Test]
        public void It_Returns_An_Empty_String_If_The_Email_Address_Is_Null()
        {
            //--arrange

            //--act
            var result = helper.RenderGravatar(null).ToString();

            //--assert
            result.ShouldBe(string.Empty);
        }

        [Test]
        public void It_Returns_An_Empty_String_If_The_Email_Address_Is_Whitespace()
        {
            //--arrange

            //--act
            var result = helper.RenderGravatar("   ").ToString();

            //--assert
            result.ShouldBe(string.Empty);
        }

        [Test]
        public void It_Returns_An_Image_Html_String_If_There_Was_An_Email()
        {
            //--arrange
            var email = "someemail@email.com";

            //--act
            var result = helper.RenderGravatar(email).ToString();

            //--assert
            //--just make sure there is at least an image tag
            result.ShouldContain("<img");
        }
    }
}
