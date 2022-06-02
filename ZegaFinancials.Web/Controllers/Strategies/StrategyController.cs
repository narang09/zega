 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using System;
using ZegaFinancials.Services.Interfaces.Strategies;
using ZegaFinancials.Services.Interfaces.Users;
using ZegaFinancials.Services.Models.Strategies;
using ZegaFinancials.Web.App_Start;

namespace ZegaFinancials.Web.Controllers.Strategies
{
    public class StrategyController : ZegaController
    {
        private readonly IStrategyService _strategyService;
        public StrategyController(IStrategyService strategyService, ISession session, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _strategyService = strategyService;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveStrategyInformation(StrategyModels strategy)
        {
                _strategyService.SaveStrategyInfo(strategy, UserContext);
                return Json(new { success = true, message = "Strategy Data Saved!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult DeleteStrategies(int[] Ids)
        {
                _strategyService.DeleteStrategiesByIds(Ids, UserContext);
                return Json(new { success = true, message = "Strategy Deleted Successfully !" });
        }

        [HttpPost]
        public JsonResult GetStrategyById([FromBody] int id)
        {
                var strategy = _strategyService.GetStrategyById(id, UserContext);
                return Json(new { success = true, response = strategy });
        }
    }
}
    