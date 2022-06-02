
using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Strategies;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Strategies
{
    public interface IStrategyDao: INHibernateDao<Strategy>
    {
        IEnumerable<Strategy> GetStrategiesByFilter(DataGridFilterModel strategyModel, out int count, bool IsAdmin);
        IEnumerable<Strategy> GetByModel(int modelId);
        bool IsExists(int strategyId, string Name);
        bool IsExists(int strategyId);
        IEnumerable<StrategyModel> GetStrategyModelList();
        IEnumerable<Strategy> GetStrategyByIds(int[] ids);
        Strategy GetBlendedStrategy();
    }
}
