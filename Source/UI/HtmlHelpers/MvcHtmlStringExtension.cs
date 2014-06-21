using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace UI.HtmlHelpers
{
    public static class MvcHtmlStringExtension
    {
        public static XElement ToXElement(this MvcHtmlString mvcHtmlString)
        {
            return XElement.Parse(mvcHtmlString.ToHtmlString());
        }
    }
}