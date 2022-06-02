using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Users
{
    public interface IRepCodeDao : INHibernateDao<RepCode>
    {
        public IEnumerable<RepCode> GetRepCodesByFilter(DataGridFilterModel dataGridFilterModel, out int count);
        bool IsRepCodeExist(RepCode repCode);
        IEnumerable<string> GetDependentAccountsbyRepCodeId(int id);
        IEnumerable<string> GetRepCodesByIds(IEnumerable<int> Ids);
    }
}
