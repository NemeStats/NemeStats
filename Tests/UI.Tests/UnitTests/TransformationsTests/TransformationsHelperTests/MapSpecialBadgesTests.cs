using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using UI.Models.Badges;
using UI.Transformations;

namespace UI.Tests.UnitTests.TransformationsTests.TransformationsHelperTests
{
    [TestFixture]

    public class MapSpecialBadgesTests
    {
        public List<IBadgeBaseViewModel> ReturnedValue { get; set; }
    }
    public class MapFromPlayerWinRecord : MapSpecialBadgesTests
    {
        public MapFromPlayerWinRecord()
        {
            PlayerWinRecord = new PlayerWinRecord();
        }

        public PlayerWinRecord PlayerWinRecord { get; set; }


        public class When_Is_Champion : MapFromPlayerWinRecord
        {
            [SetUp]
            public void SetUp()
            {
                PlayerWinRecord.IsChampion = true;
                ReturnedValue = PlayerWinRecord.MapSpecialBadges();
            }

            [Test]
            public void ItReturnsIBadgeBaseViewModelList()
            {
                Assert.IsNotNull(ReturnedValue);
            }

            [Test]
            public void Then_ChampionBadgeViewModel_Is_Added()
            {
                Assert.That(ReturnedValue.Any(b => b.GetType() == typeof(ChampionBadgeViewModel)));
            }
        }

        public class When_Is_Former_Champion : MapFromPlayerWinRecord
        {
            [SetUp]
            public void SetUp()
            {
                PlayerWinRecord.IsFormerChampion = true;
                ReturnedValue = PlayerWinRecord.MapSpecialBadges();
            }

            [Test]
            public void ItReturnsIBadgeBaseViewModelList()
            {
                Assert.IsNotNull(ReturnedValue);
            }

            [Test]
            public void Then_FormerChampionBadgeViewModel_Is_Added()
            {
                Assert.That(ReturnedValue.Any(b => b.GetType() == typeof(FormerChampionBadgeViewModel)));
            }
        }

        public class When_Is_Former_Champion_And_Also_Current_Champion : MapFromPlayerWinRecord
        {
            [SetUp]
            public void SetUp()
            {
                PlayerWinRecord.IsFormerChampion = true;
                PlayerWinRecord.IsChampion = true;
                ReturnedValue = PlayerWinRecord.MapSpecialBadges();
            }

            [Test]
            public void ItReturnsIBadgeBaseViewModelList()
            {
                Assert.IsNotNull(ReturnedValue);
            }

            [Test]
            public void Then_ChampionBadgeViewModel_Is_Added()
            {
                Assert.That(ReturnedValue.Any(b => b.GetType() == typeof(ChampionBadgeViewModel)));
            }

            [Test]
            public void Then_FormerChampionBadgeViewModel_Is_Not_Added()
            {
                Assert.That(ReturnedValue.All(b => b.GetType() != typeof(FormerChampionBadgeViewModel)));
            }
        }
    }
}
