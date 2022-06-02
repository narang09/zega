using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.Threading;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Business.Utils;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Impl
{
    public class ZegaService 
    {
        private const string _word = "realylogined";

        public ZegaService(IUserLogic userLogic)
        { 
           _userLogic = userLogic;
        }
        protected IUserLogic _userLogic { get; }
        public static TD Map<TS, TD>(TS srcObject, TD destObject)
        {
            ObjectMapper.Map(srcObject, destObject);
            return destObject;
        }
        protected User CheckUserContext([NotNull] UserContextModel userContext)
        {
            if (userContext == null)
                throw new FaultException<ZegaUnAuthorizedAccessException>(new ZegaUnAuthorizedAccessException("NotLoggedIn"), new FaultReason("You are not logged in."));

            return CheckUserContextImpl(userContext.Id, userContext.Login, userContext.AdditionalInfo, userContext.LoginTime);
        }
        private User CheckUserContextImpl(int userId, string userLogin, string userAdditionalInfo, DateTime userLoginTime)
        {
            if (userAdditionalInfo != GetUserHash(userId, userLogin, userLoginTime))
                throw new FaultException<ZegaUnAuthorizedAccessException>(new ZegaUnAuthorizedAccessException("NotLoggedIn"), new FaultReason("You are not logged in."));
            var user = _userLogic.GetUserById(userId);
            if (user == null)
                throw new FaultException<ZegaUnAuthorizedAccessException>(new ZegaUnAuthorizedAccessException("UserDeleted"), new FaultReason("Your account was deleted. Please contact your administrator."));
            if (user.Status == Status.InActive)
                throw new FaultException<ZegaUnAuthorizedAccessException>(new ZegaUnAuthorizedAccessException("UserInActive"), new FaultReason("Your account is disabled, please contact your system administrator."));

            Thread.SetData(Thread.GetNamedDataSlot("UserId"),userId);
            Thread.SetData(Thread.GetNamedDataSlot("User"), user);
            Thread.SetData(Thread.GetNamedDataSlot("UserLogin"),string.Format("{0}({1})", user.Name, user.Login));
            Thread.SetData(Thread.GetNamedDataSlot("UserSessionId"), userAdditionalInfo);

            return user;
        }
        private static string GetUserHash(int userId, string userLogin, DateTime loginTime)
        {
            return userId.GetHashCode() + "" + userLogin.GetHashCode() + "" + (loginTime.GetHashCode() + _word.GetHashCode());
        }
        protected static string GetUserHash([NotNull] UserContextModel userContext)
        {
            if (userContext == null)
                throw new ArgumentNullException("userContext");

            return GetUserHash(userContext.Id, userContext.Login, userContext.LoginTime);
        }

    }
}
