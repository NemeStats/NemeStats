using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NemeStats.TestingHelpers.NemeStatsTestingExtensions
{
    public static class NemeStatsTestingExtensionMethods
    {
        /// <summary>
        /// Asserts that the array is only length 1 and the argument at the specified index matches the given type. This is meant to be called
        /// on the result of RhinoMocks's .GetArgumentsForCallsMadeOn which returns an IList of object[]
        /// </summary>
        /// <typeparam name="T">The expected type of the parameter at index 'expectedParameterIndex'</typeparam>
        /// <param name="argumentsForCallsMadeOnMethod">The result of RhinoMocks's .GetArgumentsForCallsMadeOn which returns an IList of object[]</param>
        /// <param name="expectedParameterIndex">The index of the parameter of type T.</param>
        /// <exception cref="ArgumentException">Throws an argument exception if the requested index is greater than the number of parameters</exception>
        /// <returns>The argument at the specified parameter index</returns>
        public static T AssertFirstCallIsType<T>(this IList<object[]> argumentsForCallsMadeOnMethod, int expectedParameterIndex = 0) where T: class
        {
            return argumentsForCallsMadeOnMethod.AssertCallIsType<T>(0, expectedParameterIndex);
        }

        /// <summary>
        /// Asserts that the array is at least length of 'callIndex' and the argument at the specified index matches the given type. This is meant to be called
        /// on the result of RhinoMocks's .GetArgumentsForCallsMadeOn which returns an IList of object[]
        /// </summary>
        /// <typeparam name="T">The expected type of the parameter at index 'expectedParameterIndex'</typeparam>
        /// <param name="argumentsForCallsMadeOnMethod">The result of RhinoMocks's .GetArgumentsForCallsMadeOn which returns an IList of object[]</param>
        /// <param name="callIndex">The index of the call of the method under test. Since argumentsForCallsMadeOnMethod is an array of 0 or more calls, this is
        /// how you can specify which call is under test.</param>
        /// <param name="expectedParameterIndex">The index of the parameter of type T.</param>
        /// <returns>The argument at the specified parameter index</returns>
        public static T AssertCallIsType<T>(this IList<object[]> argumentsForCallsMadeOnMethod, int callIndex, int expectedParameterIndex = 0)
            where T : class
        {
            if (argumentsForCallsMadeOnMethod == null || argumentsForCallsMadeOnMethod.Count < 0)
            {
                Assert.Fail($"{nameof(argumentsForCallsMadeOnMethod)} cannot be null or empty.");
            }

            if (argumentsForCallsMadeOnMethod.Count <= callIndex)
            {
                Assert.Fail($"Expected at least {callIndex + 1} calls to the method, but only received {argumentsForCallsMadeOnMethod.Count}. {nameof(callIndex)} must correspond to an actual call to the method.");
            }

            var methodCall = argumentsForCallsMadeOnMethod[callIndex];
            if (expectedParameterIndex >= methodCall.Length)
            {
                throw new ArgumentException($"expectedParameterIndex value of '{expectedParameterIndex}'is greater than the number of parameters ('{methodCall.Length}').");
            }
            var actualParameter = methodCall[expectedParameterIndex] as T;

            if (actualParameter == null)
            {
                Assert.Fail($"Expected the parameter at index '{expectedParameterIndex}' to be of type '{typeof(T)}', but it was not.'");
            }
            return actualParameter;
        }

    }
}
