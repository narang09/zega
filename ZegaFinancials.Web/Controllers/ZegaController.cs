using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using System;
using System.Linq;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Web.App_Start;

namespace ZegaFinancials.Web.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ZegaController : Controller
    {
        private readonly ISession _session;
        private ITransaction _transaction;
        protected IMemoryCache _memoryCache;

        public ZegaController(ISession session,IMemoryCache memoryCache)
        {
            _session = session;
            _memoryCache = memoryCache;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _transaction = _session.BeginTransaction();
                if (HttpContext.Session.Keys.Any() && UserContext != null)
                {
                    string oldSessionId = (string)_memoryCache.Get(UserContext.Id);
                    if (oldSessionId != HttpContext.Session.Id)
                    {
                        _memoryCache.Remove(HttpContext.Session.Id);
                        HttpContext.Session.Clear();
                    }
                    else
                        _memoryCache.Set(HttpContext.Session.Id, DateTime.Now);
                }
            
            
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {           
            if (filterContext.Exception == null)
            {
                try
                {
                    if (_transaction != null && _transaction.IsActive)
                        _transaction.Commit();
                }
                catch
                {
                    if (_transaction != null && _transaction.IsActive)
                        _transaction.Rollback();
                    throw;
                }
            }
            else
            {
                _transaction.Rollback();
            }
        }
        protected void CreateSession(UserPermissionModel userPermissionModel)
        {
            Support.SessionExtensions.Set(HttpContext.Session, "User", userPermissionModel.userContext);
        }
        protected UserContextModel UserContext
        {
            get
            {
                return Support.SessionExtensions.Get<UserContextModel>(HttpContext.Session, "User");
            }
        }
        protected string GetClientIp()
        {
            var ip = string.Empty;
            if (HttpContext.Connection.RemoteIpAddress != null)
            {
                ip = HttpContext.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }

        [ZegaExceptionFilter]
        public JsonResult InitialDetails()
        {           
            var versionNumber = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductVersion;
            return Json(new { success = true, response = versionNumber });
        }

    }
}
