using StructureMap;
using UI.DependencyResolution;

namespace UI.Tests
{
    public class TestRegistry : Registry
    {
        public TestRegistry()
        {
            WebRegistry.SetupMapperMappings(this);
        }
    }

}
