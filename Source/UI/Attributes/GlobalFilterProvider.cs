using System.Collections.Generic;
using System.Web.Mvc;
using StructureMap;
using UI.Attributes.Filters;

namespace UI.Attributes
{
    public class GlobalFilterProvider : IFilterProvider
    {
        private readonly IContainer _container;

        public GlobalFilterProvider(IContainer container)
        {
            _container = container;
        }

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return new List<Filter>
            {
                //--at the moment, this is the only global filter that needs dependency injection
                new Filter(_container.GetInstance<EnsureValidGamingGroupContextAttribute>(), FilterScope.Global, order: null)
            };
        }
    }
}