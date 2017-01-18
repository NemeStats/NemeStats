using System.Collections.Generic;
using Shouldly;

namespace NemeStats.TestingHelpers.NemeStatsTestingExtensions
{
    public static class NemeStatsTestingExtensionMethods
    {
        /// <summary>
        /// Asserts that the array is only length 1 and the argument at the specified index matches the given type. This is meant to be called
        /// on the result of RhinoMocks's .GetArgumentsForCallsMadeOn which returns an IList of object[]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argumentsForCallsMadeOnMethod"></param>
        /// <param name="expectedParameterIndex"></param>
        /// <returns>The argument at the specified parameter index</returns>
        public static T AssertFirstCallIsType<T>(this IList<object[]> argumentsForCallsMadeOnMethod, int expectedParameterIndex) where T: class
        {
            argumentsForCallsMadeOnMethod.ShouldNotBeNull();
            argumentsForCallsMadeOnMethod.Count.ShouldBe(1);
            var firstCall = argumentsForCallsMadeOnMethod[0];
            var actualParameter = firstCall[expectedParameterIndex] as T;
            actualParameter.ShouldNotBeNull();
            return actualParameter;
        }
    }
}
