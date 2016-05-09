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
            this.For<DbContext>().HttpContextScoped().Use<NemeStatsDbContext>();
            this.For<IDataContext>().HttpContextScoped().Use<NemeStatsDataContext>();
            this.For<ApplicationUserManager>().HttpContextScoped().Use<ApplicationUserManager>();
        }

    }
}