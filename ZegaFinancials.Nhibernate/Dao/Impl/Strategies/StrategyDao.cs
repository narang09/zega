using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Strategies;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Strategies;
using ZegaFinancials.Nhibernate.Support.QueryGenerator;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Strategies
{
    public class StrategyDao : NHibernateDao<Strategy>, IStrategyDao
    {
        public StrategyDao(ISession session) : base(session)
        {

        }

        public IEnumerable<Strategy> GetByModel(int model)
        {
            var sql = @"From StrategyModel as s Where s.Model = :model";
            var StrategyModel = _session.CreateQuery(sql).SetParameter("model", model).List<StrategyModel>().Select(o=>o.Strategy);
            return StrategyModel;
        }

        public IEnumerable<Strategy> GetStrategiesByFilter(DataGridFilterModel strategyModel, out int count, bool IsAdmin)
        
        {
            var sql = @"
	SELECT s, (SELECT COUNT(*) From StrategyModel sm GROUP BY sm.Strategy HAVING sm.Strategy = s.Id)
	FROM Strategy AS s WHERE 1=:admin ";
             

            var qg = new QueryGenerator("s", strategyModel, new[]
              {
                    new QueryReplace("Strategy.Name", "s.Strategy.Name"),
                    new QueryReplace("ModelsCount",@"(SELECT COUNT(*) FROM StrategyModel sm WHERE sm.Strategy = s.Id)")
              }
              );

            var param = new Dictionary<string, object>(){ { "admin", IsAdmin ? 1 : 0 }
        };
            var mq = qg.ApplyBasicMultiFilters<object[]> (_session, sql, param);
            var strategies = mq.GetResult<object[]>(0);
            count = (int)mq.GetResult<long>(1).Single();
            return ConvertToStrategies(strategies.ToList());
        }
        private static List<Strategy> ConvertToStrategies(IList<object[]> list)
        {
            var result = new List<Strategy>();
            if (list == null || !list.Any())
                return result;

            for (var i = 0; i < list.Count; i++)
            {
                var currentStrategy = (Strategy)list[i][0];
                currentStrategy.ModelsCount = Convert.ToInt32(list[i][1]);
                result.Add(currentStrategy);
            }

            return result;
        }

        public virtual bool IsExists(int strategyId, string name)
        {
            var query = _session.CreateQuery(@"
	SELECT COUNT(*) FROM
	Strategy
	WHERE Id <> :sId AND Name = :Name")
                .SetParameter("sId", strategyId)
                .SetParameter("Name", name)
                .UniqueResult<long>();

            return query > 0;
        }

        public virtual bool IsExists(int Id)
        {
            var query = _session.CreateQuery(@"
	SELECT COUNT(*) FROM
	Strategy
	WHERE Id = :Id")
                .SetParameter("Id", Id)
                .UniqueResult<long>();

            return query > 0;
        }

        public IEnumerable<StrategyModel> GetStrategyModelList()
        {
            var sql = @"From StrategyModel Where 1 = 1";
            var StrategyModelS = _session.CreateQuery(sql).List<StrategyModel>();
            return StrategyModelS;
        }
        public IEnumerable<Strategy> GetStrategyByIds(int[] ids)
        {
            var sql = @" FROM Strategy WHERE Id IN (:ids)";
            return _session.CreateQuery(sql).SetParameterList("ids", ids).List<Strategy>();      
        }
        public Strategy GetBlendedStrategy()
        {
            var sql = @"From Strategy Where IsBlendedStrategy = true";
            var blendedStrategy = _session.CreateQuery(sql).List<Strategy>().FirstOrDefault();
            return blendedStrategy;
        }
    }
}
