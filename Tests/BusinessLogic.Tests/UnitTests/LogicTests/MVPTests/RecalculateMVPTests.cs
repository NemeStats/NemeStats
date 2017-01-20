using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.MVP;
using BusinessLogic.Models;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.MVPModels;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.MVPTests
{
    [TestFixture]
    public class RecalculateMVPTests
    {
        private readonly RhinoAutoMocker<MVPRecalculator> _autoMocker = new RhinoAutoMocker<MVPRecalculator>();
        private readonly ApplicationUser _applicationUser = new ApplicationUser();

        private readonly int _gameDefinitionId = 1;

        private GameDefinition _gameDefinition;
        private readonly int _previousMVPId = 75;
        private readonly int _newMVPId = 100;
        private readonly int _playerMVPId = 99;
        private readonly int _playedGameResultId = 777;
        private readonly int _pointsScored = 10;
        private MVP _savedMVP;
        private MVP _previousMVP;


        [SetUp]
        public virtual void SetUp()
        {

            _gameDefinition = new GameDefinition
            {
                Id = _gameDefinitionId,
                MVPId = _previousMVPId,
                MVP = new MVP
                {
                    Id = _previousMVPId,
                    PlayerId = _playerMVPId,
                    PlayedGameResultId = _playedGameResultId,
                    PointsScored = _pointsScored
                }
            };

            _autoMocker.Get<IDataContext>()
                .Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(new List<GameDefinition> { _gameDefinition }.AsQueryable());

            _savedMVP = new MVP() { Id = _newMVPId };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.Save(Arg<MVP>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_savedMVP);

        }

        public class When_MVP_Is_Null : RecalculateMVPTests
        {
            public override void SetUp()
            {
                _autoMocker.Get<IMVPRepository>().Expect(mock => mock.GetMVPData(_gameDefinitionId)).Return(null);

                base.SetUp();

            }

            public class When_No_Is_Allowed_To_Clear_Existing_MVP : When_MVP_Is_Null
            {
                public override void SetUp()
                {
                    base.SetUp();

                    _savedMVP = _autoMocker.ClassUnderTest.RecalculateMVP(_gameDefinitionId, _applicationUser, false);
                }

                [Test]
                public void Then_Return_Null()
                {
                    _savedMVP.ShouldBe(null);
                }

                [Test]
                public void Then_Database_Doesnt_Change()
                {
                    _autoMocker.Get<IDataContext>()
                        .AssertWasNotCalled(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
                }
            }

            public class When_Is_Allowed_To_Clear_Existing_MVP : When_MVP_Is_Null
            {
                public override void SetUp()
                {
                    base.SetUp();

                    _savedMVP = _autoMocker.ClassUnderTest.RecalculateMVP(_gameDefinitionId, _applicationUser, true);
                }

                [Test]
                public void Then_Return_Null()
                {
                    _savedMVP.ShouldBe(null);
                }

                [Test]
                public void Then_MVP_Is_Cleared()
                {
                    _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(Arg<GameDefinition>.Matches(g => g.MVPId == null && g.PreviousMVPId == _previousMVPId), Arg<ApplicationUser>.Is.Anything));
                }
            }
        }

        public class When_MVP_Is_The_Same_Player : RecalculateMVPTests
        {
            public class When_Player_Beat_His_Own_Record : When_MVP_Is_The_Same_Player
            {
                MVPData _currentMVPData;

                public override void SetUp()
                {
                    base.SetUp();

                    _currentMVPData = new MVPData()
                    {
                        PlayerId = _playerMVPId,
                        PlayedGameResultId = _playedGameResultId + 1,
                        PointsScored = _pointsScored + 1
                    };
                    _autoMocker.Get<IMVPRepository>().Expect(mock => mock.GetMVPData(_gameDefinitionId)).Return(_currentMVPData);

                    _autoMocker.ClassUnderTest.RecalculateMVP(_gameDefinitionId, _applicationUser);
                }


                [Test]
                public void Then_MVP_Data_Is_Updated()
                {
                    _autoMocker.Get<IDataContext>()
                        .AssertWasCalled(mock => mock.Save(Arg<GameDefinition>.Matches(g => g.MVP.PointsScored == _currentMVPData.PointsScored && g.MVP.PlayedGameResultId == _currentMVPData.PlayedGameResultId && g.MVPId == _previousMVPId), Arg<ApplicationUser>.Is.Anything));
                }
            }

            public class When_Is_The_Same_Record : When_MVP_Is_The_Same_Player
            {
                public override void SetUp()
                {
                    base.SetUp();

                    _autoMocker.Get<IMVPRepository>().Expect(mock => mock.GetMVPData(_gameDefinitionId)).Return(new MVPData()
                    {
                        PlayerId = _playerMVPId,
                        PlayedGameResultId = _playedGameResultId + 1,
                        PointsScored = _pointsScored
                    });

                    _autoMocker.ClassUnderTest.RecalculateMVP(_gameDefinitionId, _applicationUser);
                }


                [Test]
                public void Then_Database_Doesnt_Change()
                {
                    _autoMocker.Get<IDataContext>()
                        .AssertWasNotCalled(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
                }
            }

        }

        public class When_MVP_Is_Other_Player : RecalculateMVPTests
        {
            MVPData _currentMVPData;

            public override void SetUp()
            {
                base.SetUp();

                
                _currentMVPData = new MVPData()
                {
                    PlayerId = _playerMVPId + 1,
                    PlayedGameResultId = _playedGameResultId + 1,
                    PointsScored = _pointsScored + 1
                };
                _autoMocker.Get<IMVPRepository>().Expect(mock => mock.GetMVPData(_gameDefinitionId)).Return(_currentMVPData);

                _autoMocker.ClassUnderTest.RecalculateMVP(_gameDefinitionId, _applicationUser);
            }

            [Test]
            public void Then_New_MVP_Is_Added()
            {
                _autoMocker.Get<IDataContext>()
                    .AssertWasCalled(mock => mock.Save(Arg<MVP>.Matches(mvp => mvp.PointsScored == _currentMVPData.PointsScored && mvp.PlayedGameResultId == _currentMVPData.PlayedGameResultId && mvp.PlayerId == _currentMVPData.PlayerId), Arg<ApplicationUser>.Is.Anything));
            }

            [Test]
            public void Then_Gamedefinition_Is_Updated()
            {
                _autoMocker.Get<IDataContext>()
                    .AssertWasCalled(mock => mock.Save(Arg<GameDefinition>.Matches(g => g.MVPId == _newMVPId && g.PreviousMVPId == _previousMVPId), Arg<ApplicationUser>.Is.Anything));
            }
        }
    }
}
