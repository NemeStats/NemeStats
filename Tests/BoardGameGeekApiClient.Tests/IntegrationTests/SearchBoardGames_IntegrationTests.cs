using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Models;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    [TestFixture]
    public class SearchBoardGames_IntegrationTests : BaseBoardGameGeekApiClientIntegrationTest
    {
        protected IEnumerable<SearchBoardGameResult> Result;
        public string Query { get; set; }
        public bool Exact { get; set; }

        public class When_Make_A_Partial_Match_Query : SearchBoardGames_IntegrationTests
        {
            [SetUp]
            public virtual void SetUp()
            {
                Exact = false;
                Result = ApiClient.SearchBoardGames(Query, Exact);
            }

            public class When_Query_Match_A_Game : When_Make_A_Partial_Match_Query
            {
                [SetUp]
                public override void SetUp()
                {
                    Query = "alchemist";
                    base.SetUp();
                }

                [Test]
                public void Then_Return_Data()
                {
                    Assert.IsNotNull(Result);
                    Assert.IsNotEmpty(Result);
                }

                [Test]
                public void Then_Data_Matches_Partially_The_Query()
                {
                    var firstResult = Result.First();

                    Assert.IsTrue(firstResult.BoardGameName.ToLower().Contains(Query));
                }
            }


            public class When_Query_Not_Match_A_Game : When_Make_A_Partial_Match_Query
            {


                [SetUp]
                public override void SetUp()
                {
                    Query = "this game does not exists (or i least i hope so). now a random string: asdiasjdkoashdfkawhfi89wfy234ukzc&";
                    base.SetUp();
                }

                [Test]
                public void Then_Return_Empty_Array()
                {
                    Assert.IsNotNull(Result);
                    Assert.IsEmpty(Result);
                }

            }
        }

        public class When_Make_A_Exact_Match_Query : SearchBoardGames_IntegrationTests
        {
            [SetUp]
            public virtual void SetUp()
            {
                Exact = true;
                Result = ApiClient.SearchBoardGames(Query, Exact);
            }

            public class When_Query_Match_A_Game : When_Make_A_Exact_Match_Query
            {
                [SetUp]
                public override void SetUp()
                {
                    Query = "pandemic";
                    base.SetUp();
                }

                [Test]
                public void Then_Return_Data()
                {
                    Assert.IsNotNull(Result);
                    Assert.IsNotEmpty(Result);
                }

                [Test]
                public void Then_Data_Matches_Exactly_The_Query()
                {
                    var firstResult = Result.First();

                    Assert.AreEqual(Query, firstResult.BoardGameName.ToLower());
                }
            }


            public class When_Query_Not_Match_A_Game : When_Make_A_Partial_Match_Query
            {


                [SetUp]
                public override void SetUp()
                {
                    Query = "this game does not exists (or i least i hope so). now a random string: asdiasjdkoashdfkawhfi89wfy234ukzc&";
                    base.SetUp();
                }

                [Test]
                public void Then_Return_Empty_Array()
                {
                    Assert.IsNotNull(Result);
                    Assert.IsEmpty(Result);
                }

            }
        }
    }




}
