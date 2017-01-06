using System;
using System.IO;
using BusinessLogic.Jobs.SitemapGenerator;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using NSubstitute;
using NUnit.Framework;
using StructureMap;

namespace NemeStats.ScheduledJobs.Tests.FunctionsTests.RebuildSitemapsTests
{
    public class RebuildSitemapsTests
    {
        private IContainer _containerMock;
        private TextWriter _textWriterMock;
        private TimerInfo _timerInfoMock;
        private ISitemapGeneratorService _sitemapGeneratorServiceMock;
        private RegenerateSitemapsJobResult _expectedJobResult;

        [SetUp]
        public void SetUp()
        {
            _containerMock = Substitute.For<IContainer>();
            Program.Container = _containerMock;

            _textWriterMock = Substitute.For<TextWriter>();
            var timerSchedule = Substitute.For<TimerSchedule>();
            _timerInfoMock = Substitute.For<TimerInfo>(timerSchedule);
            _sitemapGeneratorServiceMock = Substitute.For<ISitemapGeneratorService>();

            _containerMock.GetInstance<ISitemapGeneratorService>().Returns(_sitemapGeneratorServiceMock);

            _expectedJobResult = new RegenerateSitemapsJobResult
            {
                TimeElapsedInMilliseconds = 1000,
                NumberOfSitemapsGenerated = 20
            };
            _sitemapGeneratorServiceMock.RegenerateSitemaps().Returns(_expectedJobResult);
        }

        //TODO this will be completed in Issue #564
        //[Test]
        //public void It_Rebuilds_The_Sitemap()
        //{
        //    //--arrange

        //    //--act
        //    Functions.RebuildSitemaps(_timerInfoMock, _textWriterMock);

        //    //--assert
        //    _sitemapGeneratorServiceMock.Received().RegenerateSitemaps();
        //}

        //[Test]
        //public void It_Logs_The_Results_Of_The_Job()
        //{
        //    //--arrange

        //    //--act
        //    Functions.RebuildSitemaps(_timerInfoMock, _textWriterMock);

        //    //--assert
        //    _textWriterMock.Received().WriteLine(Arg.Is<string>(x => x == $"Generated {_expectedJobResult.NumberOfSitemapsGenerated} sitemaps in {_expectedJobResult.TimeElapsedInMilliseconds} milliseconds."));
        //}
    }
}
