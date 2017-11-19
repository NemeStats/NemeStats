using System;
using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;

namespace NemeStats.TestingHelpers.NemeStatsTestingExtensions.NemeStatsTestingExtensionsTests
{
    [TestFixture]
    public class AssertCallIsTypeTests
    {
        private IList<object[]> _arguments;

        [Test]
        public void It_Fails_If_The_Requested_Call_Index_Is_Greater_Than_The_Number_Of_Calls_To_The_Method()
        {
            //--arrange
            _arguments = new List<object[]> { new object[1] };

            //--act
            Assert.Throws<AssertionException>(() => _arguments.AssertCallIsType<object>(1, 0));
        }

        [Test]
        public void It_Throws_An_Argument_Exception_If_The_Requested_Parameter_Index_Is_Greater_Than_The_Number_Of_Actual_Parameters()
        {
            //--arrange
            _arguments = new List<object[]> { new object[1] };

            //--act
           Assert.Throws<ArgumentException>(() => _arguments.AssertCallIsType<object>(0, 2));
        }

        [Test]
        public void It_Fails_If_Arguments_Is_Null()
        {
            //--act
            Assert.Throws<AssertionException>(() => _arguments.AssertCallIsType<object>(0, 0));
        }

        [Test]
        public void It_Fails_If_Arguments_Is_Empty()
        {
            //--act
            Assert.Throws<AssertionException>(() => _arguments.AssertCallIsType<object>(0, 0));
        }

        [Test]
        public void It_Fails_If_The_Specified_Parameter_Of_The_Call_Is_Not_Of_Specified_Type()
        {
            //--assert
            _arguments = new List<object[]> { new object[1] };

            //--act
            Assert.Throws<AssertionException>(() => _arguments.AssertCallIsType<string>(0, 0));
        }

        [Test]
        public void It_Fails_If_The_Specified_Parameter_Of_The_Specified_Call_Is_Not_Of_Specified_Type()
        {
            //--assert
            _arguments = new List<object[]> { new object[1], new object[1] };

            //--act
            Assert.Throws<AssertionException>(() => _arguments.AssertCallIsType<string>(1, 0));
        }

        [Test]
        public void It_Returns_The_Parameter_At_The_Specified_Index()
        {
            string firstParameter = "first parameter";
            string secondParameter = "second parameter";
            object[] argumentsForFirstCall = { firstParameter, secondParameter };
            _arguments = new List<object[]> { argumentsForFirstCall };

            //--act
            var result = _arguments.AssertCallIsType<object>(0, 1);

            //--assert
            result.ShouldBeSameAs(secondParameter);
        }
    }
}
