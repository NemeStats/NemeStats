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
using System.Web.Mvc;
using NUnit.Framework;
using UI.HtmlHelpers;

namespace UI.Tests.UnitTests.HtmlHelperTests.UIHelpersTests
{
    [TestFixture]
    public class NemePointsIcoTests
    {

        HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());

        [SetUp]
        public void SetUp()
        {
            helper = new HtmlHelper(new ViewContext(), new ViewPage());

        }


        public class When_ShowTooltip_Is_False : NemePointsIcoTests
        {
            [Test]
            public void It_Renders_Span_Without_Tooltip()
            {
                var result = helper.NemePointsIco(showTooltip: false).ToXElement();

                Assert.AreEqual("span", result.Name.ToString());
                Assert.True(result.FirstAttribute.ToString().Contains(UIHelper.NEMEPOINTICO_CSS_CLASS));
                Assert.True(result.Attributes().Count().Equals(1));
            }

        }

        public class When_Tooltip_Is_Setted : NemePointsIcoTests
        {
            [Test]
            public void It_Renders_Span_With_Tooltip()
            {
                var tooltip = "test tooltip";
                var result = helper.NemePointsIco(tooltip: tooltip).ToXElement();

                Assert.AreEqual("span", result.Name.ToString());
                Assert.True(result.FirstAttribute.ToString().Contains(UIHelper.NEMEPOINTICO_CSS_CLASS));
                Assert.True(result.Attributes().ToArray()[3].ToString().Contains(tooltip));
            }

        }
    }
}
