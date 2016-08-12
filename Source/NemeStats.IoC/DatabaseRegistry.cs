using System.Data.Entity;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using StructureMap;
using StructureMap.Web;

namespace NemeStats.IoC
{
    public class DatabaseRegistry : Registry
    {
        public DatabaseRegistry()
        {
            this.For<DbContext>().HybridHttpOrThreadLocalScoped().Use<NemeStatsDbContext>();
            this.For<IDataContext>().HybridHttpOrThreadLocalScoped().Use<NemeStatsDataContext>();
            this.For<ApplicationUserManager>().HybridHttpOrThreadLocalScoped().Use<ApplicationUserManager>();
        }

    }
}