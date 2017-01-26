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
            For<DbContext>().Transient().Use<NemeStatsDbContext>();
            For<IDataContext>().Transient().Use<NemeStatsDataContext>();
            For<ApplicationUserManager>().Transient().Use<ApplicationUserManager>();
        }

    }
}