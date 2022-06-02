using Microsoft.AspNetCore.Mvc;
using NHibernate;
using ZegaFinancials.Web.App_Start;
using ZegaFinancials.Services.Interfaces.Models;
using ZegaFinancials.Services.Models.Models;
using Microsoft.Extensions.Caching.Memory;
using ZegaFinancials.Nhibernate.Entities.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ZegaFinancials.Web.Controllers
{
    public class SleevesController : ZegaController
    {
        private readonly ISleeveService _sleeveService;
        public SleevesController(ISleeveService sleeveService, ISession session, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _sleeveService = sleeveService;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetSleeveInformation([FromBody] int sleeveId)
        {            
                var sleeve = _sleeveService.GetSleeveById(sleeveId, UserContext);
                return Json(new { success = true, Response = sleeve });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveSleeveInformation(SleeveModel sleeveModel)
        {
                _sleeveService.SaveSleeveInfo(sleeveModel, UserContext);
                return Json(new { success = true, message = "Sleeve Saved!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult DeleteSleeves(int[] sleeveIds)
        {
                _sleeveService.DeleteSleeves(sleeveIds, UserContext);
                return Json(new { success = true, message = "Sleeve(s) Deleted Successfully !" });
        }

        [HttpGet, ZegaExceptionFilter, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetByFilter()
        {
            var result = _sleeveService.GetSleevesByFilter();
            return Json(new { success = true, response = result });
        }
    }
}
