#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Champions;

namespace BusinessLogic.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecalculateChampions : DbMigration
    {
        public override void Up()
        {
            //since the model has changed, since this migration was originally written, this is no longer valid.
            //using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            //{
            //    using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            //    {
            //        IChampionRepository championRepository = new ChampionRepository(dataContext);

            //        IChampionRecalculator championRecalculator = new ChampionRecalculator(dataContext, championRepository);
            //        championRecalculator.RecalculateAllChampions();
            //    }
            //}
        }
        
        public override void Down()
        {
        }
    }
}
