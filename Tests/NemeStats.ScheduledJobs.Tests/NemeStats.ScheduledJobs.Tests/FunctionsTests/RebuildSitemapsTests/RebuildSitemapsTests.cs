using System.IO;
using BusinessLogic.Jobs.SitemapGeneratorService;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Moq;
using NUnit.Framework;
using StructureMap;
using StructureMap.AutoMocking.Moq;

namespace NemeStats.ScheduledJobs.Tests.FunctionsTests.RebuildSitemapsTests
{
    public class RebuildSitemapsTests
    {
        private Mock<IContainer> _containerMock;
        private Mock<TextWriter> _textWriterMock;
        private Mock<TimerInfo> _timerInfoMock;
        private Mock<ISitemapGeneratorService> _sitemapGeneratorServiceMock;
        private RegenerateSitemapsJobResult _expectedJobResult;

        [SetUp]
        public void SetUp()
        {
            _containerMock = new Mock<IContainer>();
            Program.Container = _containerMock.Object;

            _textWriterMock = new Mock<TextWriter>();
            var timerSchedule = new Mock<TimerSchedule>().Object;
            _timerInfoMock = new Mock<TimerInfo>(timerSchedule);
            _sitemapGeneratorServiceMock = new Mock<ISitemapGeneratorService>();

            _containerMock.Setup(x => x.GetInstance<ISitemapGeneratorService>()).Returns(_sitemapGeneratorServiceMock.Object);

            _expectedJobResult = new RegenerateSitemapsJobResult
            {
                TimeElapsedInMilliseconds = 1000,
                NumberOfSitemapsGenerated = 20
            };
            _sitemapGeneratorServiceMock.Setup(x => x.RegenerateSitemaps())
                .Returns(_expectedJobResult)
                .Verifiable();

            _textWriterMock.Setup(x => x.WriteLine(It.IsAny<string>())).Verifiable();

        }

        [Test]
        public void It_Rebuilds_The_Sitemap()
        {
            //--arrange

            //--act
            Functions.RebuildSitemaps(_timerInfoMock.Object, _textWriterMock.Object);

            //--assert
            _sitemapGeneratorServiceMock.Verify();
        }

        [Test]
        public void It_Logs_The_Results_Of_The_Job()
        {
            //--arrange

            //--act
            Functions.RebuildSitemaps(_timerInfoMock.Object, _textWriterMock.Object);

            //--assert
            _textWriterMock.Verify(x => x.WriteLine(It.Is<string>(message => message == $"Generated {_expectedJobResult.NumberOfSitemapsGenerated} sitemaps in {_expectedJobResult.TimeElapsedInMilliseconds} milliseconds.")));
        }
    }
}
