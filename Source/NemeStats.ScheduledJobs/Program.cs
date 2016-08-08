using System.Configuration.Abstractions;
using System.Diagnostics;
using Microsoft.Azure.WebJobs;
using NemeStats.IoC;
using StructureMap;
using IContainer = StructureMap.IContainer;

namespace NemeStats.ScheduledJobs
{
    internal class Program
    {
        private static IContainer _container;

        public static IContainer Container => _container ?? (_container = new Container(c =>
            {
                c.AddRegistry<CommonRegistry>();
                c.AddRegistry<DatabaseRegistry>();
            }));

        private static void Main()
        {
            var config = new JobHostConfiguration();

            config.Tracing.ConsoleLevel = TraceLevel.Info; 
            config.DashboardConnectionString = ConfigurationManager.Instance.AppSettings["AzureWebJobsDashboard"];
            config.StorageConnectionString = ConfigurationManager.Instance.AppSettings["AzureWebJobsStorage"];
            config.UseTimers();

            var host = new JobHost(config);

            host.RunAndBlock();
        }
    }
}
