using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess
{
    public class NerdScorekeeperInitializer : System.Data.Entity.DropCreateDatabaseAlways<NerdScorekeeperDbContext>
    {
        //TODO review with Clean Code book club
        protected override void Seed(NerdScorekeeperDbContext context)
        {
            DataSeeder dataSeeder = new DataSeeder(context);
            dataSeeder.SeedData();
        }
    }
}
