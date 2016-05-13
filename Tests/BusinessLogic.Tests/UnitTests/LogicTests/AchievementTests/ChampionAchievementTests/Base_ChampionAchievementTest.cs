using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.ChampionAchievementTests
{
    public abstract class Base_ChampionAchievementTest
    {
        public ChampionAchievement Achievement;
        public readonly List<Champion> ChampionedGames = new List<Champion>();
        public int PlayerId = 1;
        public int OtherPlayerId = 2;
        public IDataContext DataContext;

        [SetUp]
        public virtual void SetUp()
        {
            Achievement = AchievementFactory.GetAchievement<ChampionAchievement>();
            
            DataContext = MockRepository.GenerateStub<IDataContext>();
            DataContext.Stub(s => s.GetQueryable<Champion>()).Return(ChampionedGames.AsQueryable());
        }

        public void InsertChampionedGames(int champions, int playerId)
        {
            for (var i = 0; i < champions; i++)
            {
                ChampionedGames.Add(new Champion() { PlayerId = playerId, GameDefinitionId = new Random().Next() });
            }
        }
    }
}