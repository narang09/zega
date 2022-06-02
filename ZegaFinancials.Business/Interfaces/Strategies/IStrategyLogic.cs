
using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Strategies;

namespace ZegaFinancials.Business.Interfaces.Strategies
{
    public interface IStrategyLogic
    {
        Strategy GetStrategyById(int strategyId);
        void DeleteStrategyById(int strategyId);
        IEnumerable<Strategy> GetStrategiesByFilter(DataGridFilterModel dataGridFilterModel, out int count, bool IsAdmin);
        Strategy CreateStrategy();
        void Persist(Strategy strategy);
        IEnumerable<Strategy> GetByModel(int Id);
        bool IsStrategyExists(int id);
        IEnumerable<Strategy> GetAllStrategies();
        void Persist(IEnumerable<Strategy> strategies);
        IEnumerable<StrategyModel> GetStrategyModelList();
        IEnumerable<Strategy> GetStrategyByIds(int[] ids);
        Strategy GetBlendedStrategy();
    }
}

