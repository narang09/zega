
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using System;
using System.IO;
using System.Linq;
using ZegaFinancials.Services.Interfaces.Users;
using ZegaFinancials.Services.Models.RepCodes;
using ZegaFinancials.Services.Models.Users;
using ZegaFinancials.Web.App_Start;

namespace ZegaFinancials.Web.Controllers
{
    public class UserManagementController : ZegaController
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService, ISession session, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _userService = userService;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveUserInformation(UserEntityModel user)
        {
            _userService.SaveUserInfo(user, out int modelCount, HttpContext.Request, UserContext);
            return Json(new { success = true, message = "User Data Saved!", Response = modelCount });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult DeleteUsers(int[] Ids)
        {
            _userService.DeleteUsersByIds(Ids, UserContext);
            return Json(new { success = true, message = "User Deleted Successfully !" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetUserById([FromBody] int id)
        {
            var user = _userService.GetUserById(id, UserContext);
            return Json(new { success = true, response = user });
        }

        [HttpGet, ZegaExceptionFilter]
        public JsonResult GetSettingsInformation()
        {
            var settings = _userService.GetSettingsData(UserContext);
            return Json(new { success = true, Response = settings });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveSettingsInformation(SettingsModel settings)
        {
            _userService.SaveSettings(settings, UserContext);
            return Json(new { success = true, message = "User Data Saved Successfully!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult UploadUserProfileImage()
        {
            var files = Request.Form.Files;
            if (files.Count == 1)
            {
                var userImage = files[0];
                var maxFileSize = 2 * 1024 * 1024; // 2MB
                var acceptedFileTypes = new string[] { "image/jpg", "image/jpeg", "image/png" };
                if (userImage.Length > maxFileSize)
                    throw new Exception("File size limit exceeds. Use a image of upto 2MBs!");
                if (!acceptedFileTypes.Any(t => t == userImage.ContentType))
                    throw new Exception("File type incorrect. Use only JPEG, JPG, PNG image files!");
                using var ms = new MemoryStream();
                userImage.CopyTo(ms);
                _userService.UploadSettingsImg(new ImageDataModel { RawData = ms.ToArray() }, UserContext);
                return Json(new { success = true, message = "User Profile Image Updated!" });
            }
            else
                throw new Exception("There should be a single file");
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult RemoveUserProfileImage()
        {
            _userService.UploadSettingsImg(new ImageDataModel { RawData = null }, UserContext);
            return Json(new { success = true, message = "User Profile Image Removed!" });
        }

        [HttpGet, ZegaExceptionFilter]
        public JsonResult GetUserProfileImage()
        {
            var settings = _userService.GetSettingsImg(UserContext);
            var base64 = settings.RawData != null ? Convert.ToBase64String(settings.RawData) : string.Empty;
            var imgSrc = string.IsNullOrEmpty(base64) ? string.Empty : string.Format("data:image/jpeg;base64,{0}", base64);
            return Json(new { success = true, response = new { ImageBytes = imgSrc } });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult DeleteRepCodes(int[] repCodeIds)
        {
            _userService.DeleteRepCodeByIds(repCodeIds,UserContext);
            return Json(new { success = true, message = "Selected Repcodes Deleted Successfully!"});
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveRepCode(RepCodeModel repcode)
        {
            _userService.SaveRepCode(repcode,UserContext);
            return Json(new { success = true, message = "RepCode created successfully!"});
        }

    }
}
