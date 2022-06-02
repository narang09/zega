using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using ZegaFinancials.Services.Interfaces.CustomFields;
using ZegaFinancials.Web.App_Start;

namespace ZegaFinancials.Web.Controllers.CustomField
{
    public class CustomFieldController : ZegaController
    {
        private readonly ICustomFieldService _customFieldService;
        public CustomFieldController(ICustomFieldService customFieldService, ISession session, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _customFieldService = customFieldService;
        }

        [HttpGet, ZegaExceptionFilter,Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult LoadFieldsByEntityType()
        {
            var result = _customFieldService.LoadFieldsByEntityType(UserContext);
            return Json(new { success = true, response = result });
        }

    }
}
