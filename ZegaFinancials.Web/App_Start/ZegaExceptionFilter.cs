using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ZegaFinancials.Services;
using System.ServiceModel;
using System;

namespace ZegaFinancials.Web.App_Start
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ZegaExceptionFilter : ExceptionFilterAttribute,IExceptionFilter
    {
        public override void  OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                if (context.Exception is FaultException<ZegaUnAuthorizedAccessException>)
                {
                    context.Result = new JsonResult(new { success = false, message = context.Exception.Message });
                    context.HttpContext.Response.StatusCode = 401;
                }
                else
                {
                    context.Result = new JsonResult(new { success = false, message = context.Exception.Message });
                }
                context.ExceptionHandled = true;
            }
        }
    }
}

