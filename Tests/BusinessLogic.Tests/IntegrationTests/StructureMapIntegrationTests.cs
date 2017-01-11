using System;
using System.Diagnostics;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class StructureMapIntegrationTests
    {
        public interface ILoveFakeInterfaces
        {
            
        }

        public class FakeInterfaceImplementation : ILoveFakeInterfaces, IDisposable
        {
            public void Dispose()
            {
                Debug.WriteLine("Disposed a FakeInterfaceImplementation");
            }
        }

        [Test]
        public void Root_Containers_Return_A_Separate_Instance_If_The_Mapping_Is_Transient()
        {
            var container = new Container(c =>
            {
                c.For<ILoveFakeInterfaces>().Transient().Use<FakeInterfaceImplementation>();
            });

            var impl1 = container.GetInstance<ILoveFakeInterfaces>();

            var impl2 = container.GetInstance<ILoveFakeInterfaces>();

            impl1.ShouldNotBeSameAs(impl2);
        }

        [Test]
        public void Nested_Containers_Return_The_Same_Instance_If_The_Mapping_Is_Transient()
        {
            var container = new Container(c =>
            {
                c.For<ILoveFakeInterfaces>().Transient().Use<FakeInterfaceImplementation>();
            });

            var nestedContainer = container.GetNestedContainer();

            var impl1 = nestedContainer.GetInstance<ILoveFakeInterfaces>();
            var impl2 = nestedContainer.GetInstance<ILoveFakeInterfaces>();

            impl1.ShouldBeSameAs(impl2);
        }
    }
}
