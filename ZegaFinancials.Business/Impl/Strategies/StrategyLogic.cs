using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Business.Interfaces.Strategies;
using ZegaFinancials.Nhibernate.Dao.Interface.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.Strategies;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Strategies;

namespace ZegaFinancials.Business.Impl.Strategies
{
    public class StrategyLogic : ZegaLogic, IStrategyLogic
    {
        private readonly IStrategyDao _strategyDao;
        private readonly IModelDao _modelDao;

        public StrategyLogic(IStrategyDao strategyDao, IModelDao modelDao)
        {
            _strategyDao = strategyDao;
            _modelDao = modelDao;
        }

        public IEnumerable<Strategy> GetStrategiesByFilter(DataGridFilterModel dataGridFilterModel, out int count, bool IsAdmin)
        {
            if (!IsAdmin)
            {
                count = 0;
                return new List<Strategy>();
            }
            return _strategyDao.GetStrategiesByFilter(dataGridFilterModel, out count, IsAdmin);
        }

        public Strategy GetStrategyById(int strategyId)
        {
            return _strategyDao.Get(strategyId);
        }

        public void DeleteStrategyById(int strategyId)
        {
            _strategyDao.Delete(strategyId);
        }

        public Strategy CreateStrategy()
        {
            return _strategyDao.Create();

        }

        public void Persist(Strategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException("strategy");
            if (_strategyDao.IsExists(strategy.Id, strategy.Name))
                throw new ZegaLogicException(string.Format("Strategy with the same name already exists: '{0}'.", strategy.Name));
            _strategyDao.Persist(strategy);

        }

        public IEnumerable<Strategy> GetByModel(int id)
        {
           var strategies = _strategyDao.GetByModel(id);
            if (strategies == null)
                return new List<Strategy>();
            else
                return strategies;
        }
        public bool IsStrategyExists(int strategyId)
        {
            return _strategyDao.IsExist(strategyId);
        }
        public void Persist(IEnumerable<Strategy> strategies)
        {
                _strategyDao.Persist(strategies);
        }
        public IEnumerable<Strategy> GetAllStrategies()
        {
            var strategies = _strategyDao.GetAll();
            if (strategies != null)
                return strategies;
            else
                return new List<Strategy>();
        }

        public IEnumerable<StrategyModel> GetStrategyModelList()
        {
            return _strategyDao.GetStrategyModelList();
        }
        public IEnumerable<Strategy> GetStrategyByIds(int[] ids)
        {
            return _strategyDao.GetStrategyByIds(ids);
        }
        public Strategy GetBlendedStrategy()
        {
             return _strategyDao.GetBlendedStrategy();
        }
    }
}
