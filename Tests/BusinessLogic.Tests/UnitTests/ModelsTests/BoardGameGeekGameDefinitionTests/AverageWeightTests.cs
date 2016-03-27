using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.BoardGameGeekGameDefinitionTests
{
    [TestFixture]
    public class AverageWeightTests
    {
        [Test]
        public void ItReturnsNullIfNeitherMaxNorMinPlayTimeIsSet()
        {
            //--arrange
            var boardGameGeekGameDefinition = new BoardGameGeekGameDefinition();

            //--act/assert
            Assert.That(boardGameGeekGameDefinition.AveragePlayTime, Is.Null);
        }

        [Test]
        public void ItReturnsTheMaxPlayTimeIfMaxPlayTimeIsSetButNotMinPlayTime()
        {
            //--arrange
            var boardGameGeekGameDefinition = new BoardGameGeekGameDefinition
            {
                MaxPlayTime = 50
            };

            //--act/assert
            Assert.That(boardGameGeekGameDefinition.AveragePlayTime, Is.EqualTo(boardGameGeekGameDefinition.MaxPlayTime));
        }

        [Test]
        public void ItReturnsTheMinPlayTimeIfMinPlayTimeIsSetButNotMaxPlayTime()
        {
            //--arrange
            var boardGameGeekGameDefinition = new BoardGameGeekGameDefinition
            {
                MinPlayTime = 50
            };

            //--act/assert
            Assert.That(boardGameGeekGameDefinition.AveragePlayTime, Is.EqualTo(boardGameGeekGameDefinition.MinPlayTime));
        }

        [Test]
        public void ItReturnsTheAverageOfMaxAndMinPlayTimeIfTheyAreBothSet()
        {
            //--arrange
            var boardGameGeekGameDefinition = new BoardGameGeekGameDefinition
            {
                MinPlayTime = 100,
                MaxPlayTime = 200
            };

            //--act/assert
            Assert.That(boardGameGeekGameDefinition.AveragePlayTime, Is.EqualTo(150));
        }
    }
}
