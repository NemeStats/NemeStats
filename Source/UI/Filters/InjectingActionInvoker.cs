//using StructureMap;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace UI.Filters
//{
//    //courtesy of Jimmy Bogard: http://lostechies.com/jimmybogard/2010/05/03/dependency-injection-in-asp-net-mvc-filters/
//    public class InjectingActionInvoker : ControllerActionInvoker
//    {
//        private readonly IContainer _container;

//        public InjectingActionInvoker(IContainer container)
//        {
//            _container = container;
//        }

//        protected override FilterInfo GetFilters(
//            ControllerContext controllerContext,
//            ActionDescriptor actionDescriptor)
//        {
//            var info = base.GetFilters(controllerContext, actionDescriptor);

//            foreach(IAuthorizationFilter filter in info.AuthenticationFilters)
//            {
//                _container.BuildUp(filter);
//            }
//            foreach (IAuthorizationFilter filter in info.ActionFilters)
//            {
//                _container.BuildUp(filter);
//            }
//            foreach (IAuthorizationFilter filter in info.ResultFilters)
//            {
//                _container.BuildUp(filter);
//            }
//            foreach (IAuthorizationFilter filter in info.ExceptionFilters)
//            {
//                _container.BuildUp(filter);
//            }

//            return info;
//        }
//    }
//}