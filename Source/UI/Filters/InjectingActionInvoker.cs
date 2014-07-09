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

//            info.AuthorizationFilters.ForEach(_container.BuildUp);
//            info.ActionFilters.ForEach(_container.BuildUp);
//            info.ResultFilters.ForEach(_container.BuildUp);
//            info.ExceptionFilters.ForEach(_container.BuildUp);

//            return info;
//        }
//    }
//}