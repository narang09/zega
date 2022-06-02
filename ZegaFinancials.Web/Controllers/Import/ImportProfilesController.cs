using Microsoft.AspNetCore.Mvc;
using NHibernate;
using ZegaFinancials.Web.App_Start;
using ZegaFinancials.Services.Interfaces.SpecificationsImport;
using ZegaFinancials.Services.Models.Import;
using Microsoft.Extensions.Caching.Memory;
using ZegaFinancials.Services.Interfaces.Users;
using System.Linq;
using System.IO;
using System;
using ZegaFinancials.Nhibernate.Entities.Shared;
using System.Text;

namespace ZegaFinancials.Web.Controllers
{
    [TypeFilter(typeof(ZegaExceptionFilter))]
    public class ImportProfilesController : ZegaController
    {
        private readonly ISpecificationsImportService _specificationsImportService;
        public ImportProfilesController(ISession session, ISpecificationsImportService specifiactionsImportService,IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _specificationsImportService = specifiactionsImportService;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult Import([FromBody] int profileId)
        {
            var result = _specificationsImportService.Import(profileId, UserContext);
            return Json(new { success = true, response = result, message = "Import Successful!" });
        }

        [HttpGet, ZegaExceptionFilter]
        public JsonResult LoadRepCodeDropDown()
        {
            var result = _specificationsImportService.GetRepCodesList(UserContext);
            return Json(new { success = true, response = result });
        }

        [HttpGet, ZegaExceptionFilter]
        public JsonResult GetProfile()
        {
            var result = _specificationsImportService.GetImportProfile(UserContext);
            return Json(new { success = true, response = result });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveImportProfile(ImportProfileModel profileModel)
        {
            var profileId = _specificationsImportService.SaveImportProfile(profileModel, UserContext);
            return Json(new { success = true, message = "Import Profile Data Saved!", response = profileId });
        }
        
        [HttpPost, ZegaExceptionFilter]
        public JsonResult UploadImportFile()
        {
            if (Enum.TryParse(Request.Form["RequestSource"], out DataRequestSource gridType))
            {
                var files = Request.Form.Files;
                if (files.Count == 1)
                {
                    var importFile = files[0];
                    var acceptedFileTypes = new string[] { "application/vnd.ms-excel", "text/csv" };
                    if (!acceptedFileTypes.Any(t => t == importFile.ContentType))
                        throw new Exception("File type incorrect. Use only CSV");
                    using var ms = importFile.OpenReadStream();
                   var importResult = _specificationsImportService.ReadFileAndImportAccounts(ms,importFile.FileName, UserContext);
                   var msg = string.Format("File Import results: error count: {0}, successfully: {1}.", importResult.FailingSaved, importResult.SuccessfullyCount);
                    return Json(new { success = true, message = msg  });
                }
                else
                    throw new Exception("There should be a single file");
            }
            else
                return Json(new { success = false, messge = "Invalid Source!"});

        }
    }
}
