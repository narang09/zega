using NHibernate;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Users
{
    public  class AdvisorRepCodeDao :NHibernateDao<AdvisorRepcode> , IAdvisorRepCodeDao
    {
        public AdvisorRepCodeDao(ISession session) : base(session)
        { }

        public IList<User> GetUsersByRepCodeIds(IEnumerable<int> ids)
        {
            var query = _session.CreateQuery(@"FROM AdvisorRepcode WHERE RepCode IN (:ids)").SetParameterList("ids",ids);
            var users = query.List<AdvisorRepcode>().Select(o => o.Advisor).Distinct().ToList<User>();
            return users;
        }
        public IList<RepCode> GetAllRepCodes()
        {
            var query = _session.CreateQuery(@"SELECT DISTINCT RepCode FROM AdvisorRepcode");
            var repcodes = query.List<RepCode>();
            return repcodes;
        }
    }
}
