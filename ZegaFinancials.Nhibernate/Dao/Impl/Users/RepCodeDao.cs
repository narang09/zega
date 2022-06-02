using NHibernate;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support.QueryGenerator;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Users
{
    public class RepCodeDao : NHibernateDao<RepCode>, IRepCodeDao
    {
        public RepCodeDao(ISession session) : base(session)
        { }
        public IEnumerable<string> GetDependentAccountsbyRepCodeId(int id)
        {
            var sql = @"Select Number From Account Where RepCode =:repCodeId ";
            var accounts = _session.CreateQuery(sql).SetParameter("repCodeId", id).List<string>();
            return accounts;
        }
        public IEnumerable<RepCode> GetRepCodesByFilter(DataGridFilterModel model, out int count)
        {
            var sql = @"
	          FROM RepCode As r Where 1 = 1";
            var qg = new QueryGenerator("r", model ,new[]  
            {
                new QueryReplace("TypeValue","r.Type")
            },new Dictionary<string, string> { { "TypeValue", "RepCodeType" } 
             });
            var qb = qg.ApplyBasicMultiFilters<RepCode>(_session, sql);
            var repcodes = qb.GetResult<RepCode>(0);
            count = (int)qb.GetResult<long>(1).Single();
            return repcodes;
          
        }
        public bool IsRepCodeExist(RepCode repCode)
        {
            var sql = @" Select 1 from RepCode WHERE Id <> : id And Code = : rCode";
            var result = _session.CreateQuery(sql).SetParameter("id", repCode.Id) .SetParameter("rCode", repCode.Code).List<int>();
            return result.Any() ;     
        }
        public IEnumerable<string> GetRepCodesByIds(IEnumerable<int> Ids)
        {
            return _session.CreateQuery(@"
Select Code FROM RepCode Where Id in (:Ids)")
                .SetParameterList("Ids", Ids)
                .List<string>();               
        }
    }
}
