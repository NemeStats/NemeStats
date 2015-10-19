using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using BoardGameGeekApiClient.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekClient
{
    [TestFixture]
    public abstract class BoardGameGeekClient_BaseTest
    {
        public IBoardGameGeekApiClient BoardGameGeekApiClient { get; set; }
        public string StubResponseString { get; set; }

        protected virtual IApiDownloadService CreateStub()
        {
            var stub = MockRepository.GenerateStub<IApiDownloadService>();
            stub.Stub(s => s.DownloadApiResult(Arg<Uri>.Is.Anything))
                .Return(Task.FromResult(XDocument.Parse(StubResponseString)));

            return stub;
        }

        [SetUp]
        public virtual void SetUp()
        {
            BoardGameGeekApiClient = new Service.BoardGameGeekClient(CreateStub());
        }
    }
}