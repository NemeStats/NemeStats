using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
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
