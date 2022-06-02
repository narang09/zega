using Microsoft.AspNetCore.Mvc;
using NHibernate;
using ZegaFinancials.Web.App_Start;
using ZegaFinancials.Services.Interfaces.Models;
using ZegaFinancials.Services.Models.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ZegaFinancials.Web.Controllers
{
    public class ModelController : ZegaController
    {
        private readonly IModelService _modelService;
        public ModelController(ISession session, IModelService modelService, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _modelService = modelService;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveModelDetails(ModelModel model)
        {
             _modelService.SaveModel(model, UserContext);
             return Json(new { success = true, message = "Model Saved Succesfully!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult DeleteModels(int[] modelIds)
        {
            _modelService.DeleteModels(modelIds, UserContext);
            return Json(new { success = true, message = "Models Deleted Succesfully !" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetModelDetails([FromBody] int modelId)
        {
            var result = _modelService.GetModelById(modelId, UserContext);
            return Json(new { success = true, Response = result });
        }

        [HttpGet, ZegaExceptionFilter]
        public JsonResult GetDropdownsForModelDetails()
        {
            var dropDowns = _modelService.GetModelDetailsDropDowns(UserContext);

            if (dropDowns != null)
                return Json(new { success = true, Response = dropDowns });
            else
                return Json(new { success = false, message = "Dropdown data not found!" });
        }

        [HttpGet, ZegaExceptionFilter, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetByFilter()
        {
            var models = _modelService.GetByFilter();
            return Json(new { success = true, response = models });
        }

        [HttpPost, ZegaExceptionFilter, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetModel([FromBody] int modelId)
        {
            var model = _modelService.GetModel(modelId);
            return Json(new { success = true, response = model });
        }
    }
}
