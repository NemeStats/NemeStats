using System;
using NemeStats.Hubs;
using Owin;

namespace UI
{
	public partial class Startup
    {
        public void ConfigureSignalR(IAppBuilder app)
        {
            AppDomain.CurrentDomain.Load(typeof(LongRunningTaskHub).Assembly.FullName);
            app.MapSignalR();
        }

    }
}