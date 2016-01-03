using System;
using System.Linq;
using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekUserDefinitionCreator : IBoardGameGeekUserDefinitionCreator
    {
        private readonly IDataContext _dataContext;
        private readonly IBoardGameGeekApiClient _boardGameGeekApiClient;

        public BoardGameGeekUserDefinitionCreator(IDataContext dataContext, IBoardGameGeekApiClient boardGameGeekApiClient)
        {
            _dataContext = dataContext;
            this._boardGameGeekApiClient = boardGameGeekApiClient;
        }

        public int? CreateBoardGameGeekUserDefinition(string boardGameGeekUserName, ApplicationUser currentUser)
        {
            try
            {
                var existingRecord = _dataContext.GetQueryable<BoardGameGeekUserDefinition>().FirstOrDefault(bgg=>bgg.Name.Equals(boardGameGeekUserName,StringComparison.InvariantCultureIgnoreCase));
                if (existingRecord != null)
                {
                    return existingRecord.Id;
                }
            }
            catch (EntityDoesNotExistException)
            {
                //this is OK, just means we need to create a new one
            }
            catch (Exception)
            {
                throw;
            }

            var bggUser = _boardGameGeekApiClient.GetUserDetails(boardGameGeekUserName);

            if (bggUser == null)
            {
                return null;
            }

            var newRecord = new BoardGameGeekUserDefinition
            {
                Id = bggUser.UserId,
                Name = bggUser.Name,
                Avatar = bggUser.Avatar
            };

            _dataContext.Save(newRecord, currentUser);

            return newRecord.Id;
        }
    }
}