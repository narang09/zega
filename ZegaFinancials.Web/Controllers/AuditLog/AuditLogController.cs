using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Interfaces.Logging;
using ZegaFinancials.Web.App_Start;

namespace ZegaFinancials.Web.Controllers.AuditLog
{
    public class AuditLogController : ZegaController
    {
        private readonly IAuditLogService _auditLogService;
        public AuditLogController(IAuditLogService auditLogService, ISession session, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _auditLogService = auditLogService;
        }

        [HttpPost, ZegaExceptionFilter, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult FilterNSortClassesTime(DataGridFilterModel filterModel) 
        {
            var logs = _auditLogService.FilterNSortClassesTime(filterModel);
            return Json(new { success = true, response = logs });
        }
    }
}
