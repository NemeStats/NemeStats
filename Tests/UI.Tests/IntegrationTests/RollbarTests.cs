using NUnit.Framework;
using System;
using System.Linq;
using RollbarSharp;

namespace UI.Tests.IntegrationTests
{
    [TestFixture]
    public class RollbarTests
    {
        [Test, Ignore("Integration test")]
        public void ItPushesErrorsUpToTheServer()
        {
            RollbarClient rollbarClient = new RollbarClient();
            rollbarClient.SendErrorException(new Exception("Integration Test"));
        }
    }
}
