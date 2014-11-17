using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.ChampionsTests.ChampionRecalculatorTests
{
    [TestFixture]
    public class RecalculateAllChampionsTests
    {
        private IDataContext dataContextMock;
        private IChampionRepository championRepositoryMock;
        private ChampionRecalculator championRecalculatorPartialMock;
        private IQueryable<GameDefinition> allGameDefinitionsQueryable;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            championRepositoryMock = MockRepository.GenerateMock<IChampionRepository>();
            championRecalculatorPartialMock = MockRepository.GeneratePartialMock<ChampionRecalculator>(dataContextMock,
                championRepositoryMock);

            List<GameDefinition> allGameDefinitions = new List<GameDefinition>
            {
                new GameDefinition { Active = true, Id = 1 },
                new GameDefinition { Active = true, Id = 2 },
                new GameDefinition { Active = false, Id = 3 }
            };

            allGameDefinitionsQueryable = allGameDefinitions.AsQueryable();

            dataContextMock.Expect(mock => mock.GetQueryable<GameDefinition>()).Return(allGameDefinitionsQueryable);
        }

        [Test]
        public void ItCalculatesTheChampionForEachActiveGameDefinition()
        {
            List<GameDefinition> activeGameDefinitions =
                allGameDefinitionsQueryable.Where(gameDefinition => gameDefinition.Active).ToList();
            championRecalculatorPartialMock.Expect(mock => mock.RecalculateChampion(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(new Champion());

            championRecalculatorPartialMock.RecalculateAllChampions();

            foreach (GameDefinition gameDefinition in activeGameDefinitions)
            {
                championRecalculatorPartialMock.AssertWasCalled(mock => 
                    mock.RecalculateChampion(Arg<int>.Is.Equal(gameDefinition.Id), Arg<ApplicationUser>.Is.Anything));
            }
        }
    }
}
