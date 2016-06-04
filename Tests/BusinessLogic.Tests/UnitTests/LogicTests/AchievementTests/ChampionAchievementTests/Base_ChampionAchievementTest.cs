using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.DiversifiedAchievementTests;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.ChampionAchievementTests
{
    public abstract class Base_ChampionAchievementTest : Base_AchievementsTests<ChampionAchievement>
    {       
        public readonly List<Champion> ChampionedGames = new List<Champion>();
        public int PlayerId = 1;
        public int OtherPlayerId = 2;
        public IDataContext DataContext;

        [SetUp]
        public virtual void SetUp()
        {
            
            InitMock();
            Achievement.Get<IDataContext>().Stub(s => s.GetQueryable<Champion>()).Return(ChampionedGames.AsQueryable());
            
        }

        public void InsertChampionedGames(int champions, int playerId)
        {
            int gameDefinitionId = 1;
            for (var i = 0; i < champions; i++)
            {
                ChampionedGames.Add(new Champion() { PlayerId = playerId, GameDefinitionId = gameDefinitionId++ });
            }
        }
    }
}