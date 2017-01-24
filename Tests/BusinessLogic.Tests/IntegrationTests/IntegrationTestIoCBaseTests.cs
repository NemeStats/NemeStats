using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.IntegrationTests
{
    [TestFixture]
    public class IntegrationTestIoCBaseTests
    {
        private const int JakesMaxToleranceForNestedContainerCreationInMilliseconds = 10;

        [Test]
        public void Initializing_Classes_That_Extend_IntegrationTestIoCBase_Is_Not_Expensive()
        {
            //--arrange
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //--act
            new TestClass();

            //--assert
            stopWatch.Stop();
            stopWatch.ElapsedMilliseconds.ShouldBeLessThan(JakesMaxToleranceForNestedContainerCreationInMilliseconds);
        }

        [Test]
        public void Initializing_A_Bunch_Of_Classes_That_Extend_IntegrationTestIoCBase_Is_Not_Expensive()
        {
            //--arrange
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            int numberOfClasses = 100;

            //--act
            for (int i = 0; i < numberOfClasses; i++)
            {
                new TestClass();
            }

            //--assert
            stopWatch.Stop();
            var elapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            var averageMillisecondsToInitializeEachClass = elapsedMilliseconds / numberOfClasses;
            averageMillisecondsToInitializeEachClass.ShouldBeLessThan(JakesMaxToleranceForNestedContainerCreationInMilliseconds);
        }


        internal class TestClass : IntegrationTestIoCBase
        {
            
        }
    }
}
