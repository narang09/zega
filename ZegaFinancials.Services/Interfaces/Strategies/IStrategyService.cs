
using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Models;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Services.Models.Strategies;

namespace ZegaFinancials.Services.Interfaces.Strategies
{
   public interface IStrategyService
    {
       DataGridModel LoadStrategiesByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext);
       void DeleteStrategiesByIds(int[] strategyIds, UserContextModel userContext);
       void SaveStrategyInfo(StrategyModels strategyModel, UserContextModel userContext);
       StrategyModels GetStrategyById(int id, UserContextModel userContext);
       IList<ZegaModel> GetAllStrategies();
       IEnumerable<ZegaModel> GetAllStrategies(UserContextModel userContext);
    }
}
