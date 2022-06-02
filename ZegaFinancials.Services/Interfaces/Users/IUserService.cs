using System.Collections.Generic;
﻿using Microsoft.AspNetCore.Http;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Models.RepCodes;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Services.Models.Users;

namespace ZegaFinancials.Services.Interfaces.Users
{
    public interface IUserService
    {
        UserPermissionModel GetUserPermission(LoginModel userLogin);
        User VerifyUserCredentials(LoginModel loginModel);
        DataGridModel LoadUsersByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext);
        void SaveUserInfo(UserEntityModel userModel, out int ModelCount,HttpRequest request , UserContextModel userContext);
        void SaveRepCode(RepCodeModel repCodeModel, UserContextModel userContext);
        DataGridModel LoadRepCodesByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext);
        void DeleteUsersByIds(int[] Ids ,UserContextModel userContext);
        void DeleteRepCodeByIds(int[] ids, UserContextModel userContext);
        UserViewModel GetUserById(int id, UserContextModel userContext);
        SettingsModel GetSettingsData(UserContextModel userContext);
        void SaveSettings(SettingsModel settings,UserContextModel userContext);
        void UploadSettingsImg(ImageDataModel image,UserContextModel userContext);
        string SendForgetPasswordEmail(string userEmail, HttpRequest request);
        bool ResetPassword(string userLogin,string newPassword,string outhToken);
        void BulkEditUsers(BulkEditModel bulkChanges, UserContextModel userContext);
        ImageDataModel GetSettingsImg(UserContextModel userContext);
        LoginActivityModel GetLoginActivity(int userid, string SessionId);
        void SaveLoginActivity(int userId,string clientIp , string sessionId);
        void DeleteLoginActivity(string sessionid);
        void CheckUserPermissionForLogin(UserContextModel userContext);
        string GetJwtAuthToken(string login);

    }
}
   
