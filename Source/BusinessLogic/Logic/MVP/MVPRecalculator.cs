using System.Data.Entity;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.MVP
{
    public class MVPRecalculator : IMVPRecalculator
    {
        private readonly IMVPRepository _mvpRepository;
        private readonly IDataContext _dataContext;

        public MVPRecalculator(IMVPRepository mvpRepository, IDataContext dataContext)
        {
            _mvpRepository = mvpRepository;
            _dataContext = dataContext;
        }

        public Models.MVP RecalculateMVP(int gameDefinitionId, ApplicationUser applicationUser, bool allowedToClearExistingMVP = true)
        {
            var gameDefinition = _dataContext.GetQueryable<GameDefinition>().Include(g=>g.MVP).FirstOrDefault(g=>g.Id == gameDefinitionId);

            var currentMVPData = _mvpRepository.GetMVPData(gameDefinitionId);

            if (currentMVPData == null)
            {
                if (allowedToClearExistingMVP)
                {
                    ClearMVP(applicationUser, gameDefinition);
                }

                return null;
            }

            if (gameDefinition.MVP.PlayerId == currentMVPData.PlayerId)
            {
                if (gameDefinition.MVP.PointsScored < currentMVPData.PointsScored)
                {
                    //Same player beating his record
                    gameDefinition.MVP.DatePlayed = currentMVPData.DatePlayed;
                    gameDefinition.MVP.PointsScored = currentMVPData.PointsScored;
                    gameDefinition.MVP.PlayedGameResultId = currentMVPData.PlayedGameResultId;

                    _dataContext.Save(gameDefinition, applicationUser);
                }

                return gameDefinition.MVP;
            }

                var newMVP = new Models.MVP
                {
                    PlayerId = currentMVPData.PlayerId,
                    PlayedGameResultId = currentMVPData.PlayedGameResultId,
                    PointsScored = currentMVPData.PointsScored,
                    DatePlayed = currentMVPData.DatePlayed
                };

                var savedMVP = _dataContext.Save(newMVP, applicationUser);

                gameDefinition.PreviousMVPId = gameDefinition.MVPId;
                gameDefinition.MVPId = savedMVP.Id;
                _dataContext.Save(gameDefinition, applicationUser);

                return savedMVP;
           
            
        }

        private void ClearMVP(ApplicationUser applicationUser, GameDefinition gameDefinition)
        {
            if (gameDefinition.MVPId != null)
            {
                gameDefinition.PreviousMVPId = gameDefinition.MVPId;
                gameDefinition.MVPId = null;
                _dataContext.Save(gameDefinition, applicationUser);
            }
        }
    }
}
