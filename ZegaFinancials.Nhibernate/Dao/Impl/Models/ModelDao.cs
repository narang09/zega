using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Strategies;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Nhibernate.Support.QueryGenerator;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Models
{
    public class ModelDao : NHibernateDao<Model>, IModelDao
    {
        private readonly IUserDao _userDao;
        private readonly IAdvisorRepCodeDao _advisorRepCodeDao;
        public ModelDao(ISession session,IUserDao userDao , IAdvisorRepCodeDao advisorRepCodeDao) : base(session)
        {
            _userDao = userDao;
            _advisorRepCodeDao = advisorRepCodeDao;
        }
        public Model GetModelByName(string modelName)
        {
            var sql = @" FROM Model AS m WHERE Name =(:modelName)";
            var model = _session.CreateQuery(sql).SetParameter("modelName", modelName).List<Model>().FirstOrDefault();
            return model;
        }
        public IEnumerable<Model> GetModelsByFilter(DataGridFilterModel model, IEnumerable<int> modelIds, bool isAdmin, out int count)
        {
            var sql = @" FROM Model AS m WHERE ";
            var qg = new QueryGenerator("m", model, new[]
              {
                    new QueryReplace("Sleeves.Allocation", "m.ModelSleeve.Allocation"),
                    new QueryReplace("StrategyNames",@"(SELECT STRING_AGG(Name,',') FROM Strategy WHERE Id IN (SELECT Strategy FROM StrategyModel WHERE model = m.Id))")
              });

            var param = new Dictionary<string, object>();
            if (isAdmin)
            {
                sql += "1=:admin AND m.IsLocalBlend = 0 ";
                param.Add("admin", isAdmin);
            }
            else
            {
                sql += "m.Id in (:modelIds) AND m.IsLocalBlend = 0";
                param.Add("modelIds", modelIds);
            }
            var mq = qg.ApplyBasicMultiFilters<Model>(_session, sql, param);
            var models = mq.GetResult<Model>(0);
            count = (int)mq.GetResult<long>(1).Single();
            return models;
        }

        public IEnumerable<Model> GetModelsByIds(int[] modelIds)
        {
            if (!modelIds.Any())
                return Array.Empty<Model>();
            var sql = @" FROM Model WHERE Id IN (:modelIds)";
            return _session.CreateQuery(sql)
                .SetParameterList("modelIds", modelIds).List()
                .Cast<Model>();
        }
        public void CheckModelExistence(string modelName,int modelId, bool IsBlendModel, bool IsLocalBlend,int accountId = 0)
        {
            if ((IsBlendModel && !IsLocalBlend) || (!IsBlendModel && !IsLocalBlend))
            {
                var modelchecksql = @"SELECT 1 FROM Model WHERE Id<> :modelId AND Name = :modelName AND IsLocalBlend = 0";
                var modelchecksqlquery = _session.CreateQuery(modelchecksql)
                .SetParameter("modelName", modelName)
                .SetParameter("modelId", modelId).List<int>();
                if (modelchecksqlquery.Any())
                    throw new ZegaDaoException(string.Format("Model With Same Name Already Exist Name:{0}", modelName));
            }
            else if(IsLocalBlend && accountId !=0)
            {
                var blendModelchecksql = @"SELECT 1 FROM Model WHERE Id<> :modelId AND Name = :modelName And IsBlendModel =1 AND IsLocalBlend = 0 AND AccountId = 0";
                var blendModelchecksqlquery = _session.CreateQuery(blendModelchecksql)
                .SetParameter("modelName", modelName)
                .SetParameter("modelId", modelId).List<int>();

                if (blendModelchecksqlquery.Any())
                    throw new ZegaDaoException(string.Format("Same model already exists : Name :{0}", modelName));
                else 
                {
                  var accountBlendmodelcheckSql = @"SELECT 1 FROM Model WHERE Id<> :modelId AND Name = :modelName And IsBlendModel =1 AND IsLocalBlend = 1 AND AccountId =:accountId";
                  var accountBlendmodelcheckSqlquery = _session.CreateQuery(accountBlendmodelcheckSql)
                  .SetParameter("modelName", modelName)
                  .SetParameter("modelId", modelId).SetParameter("accountId", accountId).List<int>();
                 if(accountBlendmodelcheckSqlquery.Any())
                        throw new ZegaDaoException(string.Format("Blend  Model With Same Name Already Exist Name:'{0}' For this Account !", modelName));
                }
            }
        }
        public void CheckAdvisorDependency(int modelId)
        {
            var sql = @"select Advisor From AdvisorModel as e Where e.Model =:modelId";
            var advisors = _session.CreateQuery(sql)
                .SetParameter("modelId", modelId)
                .List<User>()
                .Select(o => string.Format("{0} {1}", o.Details?.FirstName, o.Details?.LastName));
            if (advisors.Any())
                throw new ZegaDaoException(string.Format("This Model  is used in Advisors and can not be deleted ( Advisors are: {0}).", string.Join(",", advisors)));            
        }
        public void CheckStrategyDependency(int modelId)
        {
            var sql = @"select Strategy From StrategyModel as e Where e.Model =:modelId ";
            var strategies = _session.CreateQuery(sql).SetParameter("modelId", modelId).List<Strategy>().Select(o => o.Name).ToList();
            if (strategies.Any())
                throw new ZegaDaoException(string.Format("This Model is used in Strategies and can not be deleted ( Strategies are : {0}).", string.Join(",", strategies)));            
        }

        public void CheckAccountDependency(int modelId)
        {
            var sql = @"select Name From Account Where Model =:modelId ";
            var accounts = _session.CreateQuery(sql).SetParameter("modelId", modelId).List<string>();
            if (accounts.Any())
                throw new ZegaDaoException("The Model is used in Accounts hence can’t be deleted.");
        }

        public void CheckAccountandAdvisorDependency(Model model, int advisorId)
        {
            var sql = @"select Name From Account Where Model =: modelId AND RepCode IN (select RepCode FROM AdvisorRepcode WHERE Advisor =: advisorId)";
            var accounts = _session.CreateQuery(sql)
                .SetParameter("modelId", model.Id)
                .SetParameter("advisorId",advisorId)                
                .List<string>();
            if (accounts.Any())
                throw new ZegaDaoException(string.Format("This Model {0} is assigned to Accounts:{1}.", model.Name, string.Join(",", accounts)));
        }

        public virtual long GetModelsCountBySleeve(int sleeveId)
        {
            return _session.CreateQuery(@"
	            SELECT COUNT(*)
	            FROM Model AS m
	            LEFT JOIN m.ModelSleeves sleeve
	            WHERE sleeve.Sleeve.Id = :sleeveId")
            .SetParameter("sleeveId", sleeveId)
            .UniqueResult<long>();
        }

        public void CheckBlendModelDependency(int modelId)
        {
            var sql = @"select Model From BlendModelDetails as m Where m.SubModel =:modelId ";
            var blendModels = _session.CreateQuery(sql).SetParameter("modelId", modelId).List<Model>().Select(o => o.Name).ToList();
            if (blendModels.Any())
                throw new ZegaDaoException(string.Format("This Model is used in Blend Model and can not be deleted ( Blend Models are : {0}).", string.Join(",", blendModels)));
        }

        public IEnumerable<Model> GetAllModels()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<Model> GetModelsListByRepCodeId(DataGridFilterModel model, int? repCodeId, int accountId, out int count,int userId, bool isAdmin = false)
        {
            var advisors = _advisorRepCodeDao.GetUsersByRepCodeIds(new int[] { (int)repCodeId });
            var advisorsmodels = isAdmin ? advisors?.SelectMany(o => o.Models)?.Select(x => x.Model.Id).Distinct().ToArray() : advisors.FirstOrDefault(o =>o.Id == userId)?.Models?.Select(o =>o.Model.Id).ToArray();
               
            var sql = @" FROM Model AS m WHERE ";
            var qg = new QueryGenerator("m", model, new[]
              {
                    new QueryReplace("Sleeves.Allocation", "m.ModelSleeve.Allocation"),
                    new QueryReplace("StrategyNames",@"(SELECT STRING_AGG(Name,',') FROM Strategy WHERE Id IN (SELECT Strategy FROM StrategyModel WHERE model = m.Id))")

              });

            var param = new Dictionary<string, object>();
            if (advisorsmodels != null && advisorsmodels.Any() && accountId != 0)
            {
                sql += "(m.Id in (:modelIds) OR (m.AccountId =:accountId AND m.IsLocalBlend =1))";
                param.Add("modelIds", advisorsmodels);
                param.Add("accountId", accountId);
            }
            else if (advisorsmodels != null && advisorsmodels.Any() && accountId == 0)
            {
                sql += "m.Id in (:modelIds)";
                param.Add("modelIds", advisorsmodels);
            }
            else if ((advisorsmodels == null || !advisorsmodels.Any()) && accountId != 0)
            {
                sql += "m.AccountId =:accountId AND m.IsLocalBlend =1";
                param.Add("accountId", accountId);
            }
            else
            {
                count = 0;
                return new List<Model>();
            }
            var mq = qg.ApplyBasicMultiFilters<Model>(_session, sql, param);
            var models = mq.GetResult<Model>(0);
            count = (int)mq.GetResult<long>(1).Single();
            return models;
        }
        public IEnumerable<int> GetModelIdsByUserId(int userId)
        {
            return _session.CreateQuery(@"SELECT Model.Id FROM AdvisorModel WHERE Advisor = :advisorId").SetParameter("advisorId", userId).List<int>();
        }
    }
}
