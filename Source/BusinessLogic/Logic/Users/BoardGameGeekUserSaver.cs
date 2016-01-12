using System;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Validation;

namespace BusinessLogic.Logic.Users
{
    public class BoardGameGeekUserSaver : BaseValidatableRequestSaver, IBoardGameGeekUserSaver
    {
        internal const string EXCEPTION_MESSAGE_CURRENT_USER_ALREADY_HAVE_BGG_ACCOUNT_LINKED = "The current user has already a BoardGameGeek account linked.";

        private readonly IDataContext _dataContext;

        public BoardGameGeekUserSaver(IDataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public BoardGameGeekUserDefinition CreateUserDefintion(CreateBoardGameGeekUserDefinitionRequest request, ApplicationUser currentUser)
        {
            ValidateRequest(request);
            ValidateCurrentUserHasBGGAccount(currentUser);

            var existingItem = _dataContext.GetQueryable<BoardGameGeekUserDefinition>().FirstOrDefault(u => u.Id == request.BoardGameGeekUserId);

            if (existingItem == null)
            {
                existingItem = new BoardGameGeekUserDefinition
                {
                    Id = request.BoardGameGeekUserId,
                    Name = request.Name,
                    ApplicationUserId = currentUser.Id,
                    Avatar = request.Avatar
                };
            }

            return _dataContext.Save(existingItem, currentUser);
        }

        public void ValidateCurrentUserHasBGGAccount(ApplicationUser currentUser)
        {
            if (_dataContext.GetQueryable<BoardGameGeekUserDefinition>().Any(u => u.ApplicationUserId == currentUser.Id))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_CURRENT_USER_ALREADY_HAVE_BGG_ACCOUNT_LINKED);
            }
        }
    }
}
