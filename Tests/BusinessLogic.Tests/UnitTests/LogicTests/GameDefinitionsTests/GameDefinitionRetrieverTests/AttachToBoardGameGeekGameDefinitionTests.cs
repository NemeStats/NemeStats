using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class AttachToBoardGameGeekGameDefinitionTests
    {
        //[Test]
        //public void ItDoesNotSetAThumbnailIfTheClientDoesntReturnAGameDefinition()
        //{
        //    var gameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
        //    gameDefinition.Name = "name";
        //    gameDefinition.BoardGameGeekGameDefinitionId = 1;
        //    gameDefinition.Expect(mock => mock.AlreadyInDatabase())
        //        .Return(false);

        //    autoMocker.Get<IBoardGameGeekApiClient>().Expect(mock => mock.GetGameDetails(gameDefinition.BoardGameGeekGameDefinitionId.Value))
        //              .Return(null);
        //    autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>()).Return(new List<GameDefinition>().AsQueryable());

        //    autoMocker.ClassUnderTest.Save(gameDefinition, currentUser);

        //    autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
        //                 Arg<GameDefinition>.Matches(newGameDefinition => newGameDefinition.ThumbnailImageUrl == null),
        //                 Arg<ApplicationUser>.Is.Anything));
        //}
    }
}
