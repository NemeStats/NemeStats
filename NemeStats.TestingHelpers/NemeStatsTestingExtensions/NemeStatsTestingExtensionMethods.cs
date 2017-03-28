using System;
using System.Collections.Generic;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions.NemeStatsTestingExtensionsTests;
using NUnit.Framework;
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
        /// <param name="argumentsForCallsMadeOnMethod">The result of RhinoMocks's .GetArgumentsForCallsMadeOn which returns an IList of object[]</param>
        /// <param name="expectedParameterIndex">The index of the parameter of type T.</param>
        /// <exception cref="ArgumentException">Throws an argument exception if the requested index is greater than the number of parameters</exception>
        /// <returns>The argument at the specified parameter index</returns>
        public static T AssertFirstCallIsType<T>(this IList<object[]> argumentsForCallsMadeOnMethod, int expectedParameterIndex = 0) where T: class
        {
            if (argumentsForCallsMadeOnMethod == null || argumentsForCallsMadeOnMethod.Count == 0)
            {
                Assert.Fail($"{nameof(argumentsForCallsMadeOnMethod)} cannot be null or empty.");
            }

            if (argumentsForCallsMadeOnMethod.Count > 1)
            {
                Assert.Fail($"Expected only 1 call, but received {argumentsForCallsMadeOnMethod.Count}.");
            }
            var firstCall = argumentsForCallsMadeOnMethod[0];
            if (expectedParameterIndex >= firstCall.Length)
            {
                throw new ArgumentException($"expectedParameterIndex value of '{expectedParameterIndex}'is greater than the number of parameters ('{firstCall.Length}').");
            }
            var actualParameter = firstCall[expectedParameterIndex] as T;

            if (actualParameter == null)
            {
                Assert.Fail($"Expected the parameter at index '{expectedParameterIndex}' to be of type '{typeof(T)}', but it was not.'");
            }
            return actualParameter;
        }
    }
}
