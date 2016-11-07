//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BusinessLogic.Exceptions;
//using BusinessLogic.Models.User;

//namespace BusinessLogic.DataAccess.Security
//{
//    public class GenericSecuredEntityValidator : IGenericSecuredEntityValidator
//    {
//        internal const string EXCEPTION_MESSAGE_CURRENT_USER_ID_CANNOT_BE_NULL
//             = "currentUser.Id cannot be null";

//        private readonly IDataContext _dataContext;

//        public GenericSecuredEntityValidator(IDataContext dataContext)
//        {
//            _dataContext = dataContext;
//        }

//        TEntity ValidateAccess<TEntity>(object primaryKeyValue, ApplicationUser currentUser) where TEntity : class, EntityWithTechnicalKey 
//        {
//            ValidateArguments(currentUser);

//            var entity = _dataContext.FindById<TEntity>(primaryKeyValue);

//            if (entity == null)
//            {
//                throw new EntityDoesNotExistException(typeof(TEntity), primaryKeyValue);
//            }

//            var securedEntity = entity as SecuredEntityWithTechnicalKey;

//            if (securedEntity == null)
//            {
//                return entity;
//            }

//            if (securedEntity.GamingGroupId != default(int) && securedEntity.GamingGroupId != currentUser.CurrentGamingGroupId)
//            {
//                var matchingUserGamingGroup = _dataContext.GetQueryable<UserGamingGroup>()
//                    .SingleOrDefault(
//                                    x =>
//                                    x.GamingGroupId == securedEntity.GamingGroupId &&
//                                    x.ApplicationUserId == currentUser.Id);

//                if (matchingUserGamingGroup == null)
//                {
//                    throw new UnauthorizedEntityAccessException(currentUser.Id, typeof(TEntity), primaryKeyValue);
//                }
//            }

//            return entity;
//        }

//        private static void ValidateArguments(ApplicationUser currentUser)
//        {
//            if (currentUser == null)
//            {
//                throw new ArgumentNullException(nameof(currentUser));
//            }

//            if (currentUser.Id == null)
//            {
//                throw new ArgumentException(EXCEPTION_MESSAGE_CURRENT_USER_ID_CANNOT_BE_NULL);
//            }
//        }
//    }
//}
