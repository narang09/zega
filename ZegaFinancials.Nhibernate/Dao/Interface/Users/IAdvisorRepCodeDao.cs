using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Users
{
    public interface IAdvisorRepCodeDao : INHibernateDao<AdvisorRepcode>
    {
      IList<User> GetUsersByRepCodeIds(IEnumerable<int> ids);
        IList<RepCode> GetAllRepCodes();
    }
    
}
