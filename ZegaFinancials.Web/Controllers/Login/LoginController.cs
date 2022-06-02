using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NHibernate;
using System;
using ZegaFinancials.Services.Interfaces.Users;
using ZegaFinancials.Services.Models.Users;
using ZegaFinancials.Web.App_Start;
using ZegaFinancials.Web.Models.Support;
using ZegaFinancials.Web.Models.Users;

namespace ZegaFinancials.Web.Controllers
{
    public class LoginController : ZegaController
    {
        private readonly ILogger<LoginController> _logger;
        private readonly IUserService _userService;
        public LoginController(IUserService userService, ISession session, ILogger<LoginController> logger, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetLoginResponse(LoginModel loginInfo)
        {
            UserInfoModel userInfoModel = new UserInfoModel();
            try
            {
                userInfoModel.IsSuccessfullLogin = true;
                var currentSession = UserContext;

                if (currentSession == null)
                {
                    var loginResponse = _userService.GetUserPermission(loginInfo);

                    if (loginResponse.userContext != null)
                    {
                        string oldSessionId;
                        var IsUserLoginInfoExist = _memoryCache.TryGetValue(loginResponse.userContext.Id, out oldSessionId);
                        var currentRequestClientIp = GetClientIp();
                        if (loginInfo.IsForceLogin)
                        {
                            _userService.DeleteLoginActivity(oldSessionId);
                            _memoryCache.Remove(oldSessionId);
                        }
                       else if (IsUserLoginInfoExist)
                        {
                            var loginActivity = _userService.GetLoginActivity(loginResponse.userContext.Id, oldSessionId);
                            var lastActivitytime = _memoryCache.Get(oldSessionId);
                            if (lastActivitytime != null && (DateTime.Now - (DateTime)lastActivitytime).TotalMinutes >= 20)
                            {
                                _userService.DeleteLoginActivity(oldSessionId);
                            }
                             else if (HttpContext.Session.Id != oldSessionId && !string.IsNullOrEmpty(loginActivity.IpAddress))
                            {
                                if (!string.IsNullOrEmpty(currentRequestClientIp) && loginActivity.IpAddress == currentRequestClientIp)
                                    throw new Exception("The user is already logged in on another browser, Do you want to login forcefully?");
                                else if (!string.IsNullOrEmpty(currentRequestClientIp) && !string.IsNullOrEmpty(loginActivity.IpAddress) && loginActivity.IpAddress != currentRequestClientIp)
                                    throw new Exception("The user is already logged in on another machine, Do you want to login forcefully?");
                            }
                        }
                        userInfoModel.IsSuccessfullLogin = true;
                        userInfoModel.LoginId = loginResponse.userContext.Login;
                        userInfoModel.Timestamp = DateTime.Now;
                        userInfoModel.IsAdmin = loginResponse.userContext.IsAdmin;
                        CreateSession(loginResponse);
                        _memoryCache.Set(loginResponse.userContext.Id, HttpContext.Session.Id);
                        _userService.SaveLoginActivity(loginResponse.userContext.Id, GetClientIp(), HttpContext.Session.Id);
                        _logger.LogInformation($"User , {UserContext.Name}! has logged in !");
                    }
                }
                else if (currentSession.Login.ToLower() != loginInfo.Login.ToLower())
                    throw new Exception("Another user logged in on this browser");
                else
                { // Already Logged in! from Same Session. sending info for UI purpose. 
                    _userService.CheckUserPermissionForLogin(UserContext);
                    userInfoModel.IsSuccessfullLogin = UserContext.LogedIn;
                    userInfoModel.LoginId = UserContext.Login;
                    userInfoModel.Name = UserContext.Name;
                    userInfoModel.IsAdmin = UserContext.IsAdmin;
                    userInfoModel.Id = UserContext.Id;
                    userInfoModel.Timestamp = UserContext.LoginTime;
                }
                return Json(new { success = true, response = userInfoModel });
            }
            catch (Exception ex)
            {
                if (ex.Message == "The user is already logged in on another browser, Do you want to login forcefully?" || ex.Message == "The user is already logged in on another machine, Do you want to login forcefully?")
                    return Json(new { success = true, message = ex.Message, responseCode = (int)ZegaResponseCodes.AlreadyLoggedIn });
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult ForgotPassword([FromBody] string userEmail)
        {
            var result = _userService.SendForgetPasswordEmail(userEmail, HttpContext.Request);
            return Json(new { success = true, response = result, message = "Password sent to Primary Email!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult ResetPassword(UserPasswordResetModel userPasswordReset)
        {
            var isPasswordReseted = _userService.ResetPassword(userPasswordReset.Login, userPasswordReset.Password, userPasswordReset.AuthToken);
            if (isPasswordReseted)
                return Json(new { success = true, message = "Password update successful !" });
            else
                return Json(new { success = false, message = "Password update unsuccessful !" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult LogoutUser()
        {
            if (UserContext != null)
            {
                _logger.LogInformation($"User , {UserContext.Name} has logged out !");
            }
            HttpContext.Session.Clear();
            _memoryCache.Remove(HttpContext.Session.Id);
            _userService.DeleteLoginActivity(HttpContext.Session.Id);
            return Json(new { success = true, response = "", message = "User has been successfully logged out!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult UnloadUser(LoginModel loginInfo)
        {
            JsonResult result;
            var user = _userService.VerifyUserCredentials(loginInfo);
            if (user != null)
                result = LogoutUser();
            else
                result = Json(new { success = true, response = "" });
            return result;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetUserPermission(LoginModel loginInfo)
        {
            var result = _userService.GetUserPermission(loginInfo).userContext;
            result.Authorization = _userService.GetJwtAuthToken(loginInfo.Login);
            return Json(new { success = true, response = result, message = "User has been successfully logged in !" });
        }


    }
}
