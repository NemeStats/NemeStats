using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Events.HandlerFactory;
using BusinessLogic.Logic.Achievements;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.AchievementTests.DiversifiedAchievementTests
{
    public abstract class Base_DiversifiedAchievementTests
    {
        public DiversifiedAchievement Achievement;
        public readonly List<PlayerGameResult> PlayedGames = new List<PlayerGameResult>();
        public int PlayerId = 1;
        public int OtherPlayerId = 2;
        public IDataContext DataContext;

        [SetUp]
        public virtual void SetUp()
        {
            Achievement = AchievementFactory.GetAchievement<DiversifiedAchievement>();

            DataContext = MockRepository.GenerateStub<IDataContext>();
            DataContext.Stub(s => s.GetQueryable<PlayerGameResult>()).Return(PlayedGames.AsQueryable());
        }

        public void InsertPlayedGames(int games, int playerId)
        {
            for (var i = 0; i < games; i++)
            {
                PlayedGames.Add(new PlayerGameResult()
                {
                    PlayerId = playerId,
                    PlayedGame = new PlayedGame
                    {
                        GameDefinition = new GameDefinition() {Id = i}
                    }
                });
            }
        }
    }
}
