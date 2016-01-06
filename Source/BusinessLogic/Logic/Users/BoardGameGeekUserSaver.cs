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
        private readonly IDataContext _dataContext;

        public BoardGameGeekUserSaver(IDataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public BoardGameGeekUserDefinition CreateUserDefintion(CreateBoardGameGeekUserDefinitionRequest request, ApplicationUser currentUser)
        {
            ValidateRequest(request);

            var existingItem = _dataContext.GetQueryable<BoardGameGeekUserDefinition>().FirstOrDefault(u => u.Name.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase));

            if (existingItem == null)
            {
                existingItem = new BoardGameGeekUserDefinition
                {
                    Name = request.Name,
                    ApplicationUserId = currentUser.Id,
                    Avatar = request.Avatar
                };
            }

            return _dataContext.Save(existingItem, currentUser);
        }
    }
}
