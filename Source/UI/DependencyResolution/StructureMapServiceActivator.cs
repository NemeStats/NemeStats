using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using UI.App_Start;

namespace UI.DependencyResolution
{
    public class StructureMapServiceActivator : IHttpControllerActivator
    {
        public StructureMapServiceActivator(HttpConfiguration configuration)
        {
        }

        public IHttpController Create(HttpRequestMessage request,
                                      HttpControllerDescriptor controllerDescriptor,
                                      Type controllerType)
        {
            return StructuremapMvc.StructureMapDependencyScope.GetInstance(controllerType) as IHttpController;
        }
    }
}