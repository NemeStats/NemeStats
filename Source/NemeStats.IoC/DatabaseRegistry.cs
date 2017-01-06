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
            For<DbContext>().HybridHttpOrThreadLocalScoped().Use<NemeStatsDbContext>();
            For<IDataContext>().HybridHttpOrThreadLocalScoped().Use<NemeStatsDataContext>();
            For<ApplicationUserManager>().HybridHttpOrThreadLocalScoped().Use<ApplicationUserManager>();
        }

    }
}