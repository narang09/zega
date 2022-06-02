using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Models
{
    public interface ISleeveDao : INHibernateDao<Sleeve>
    {
        IEnumerable<Sleeve> GetSleevesByFilter(DataGridFilterModel model, out int count);
        bool IsExists(int sleeveId, string name);
        bool IsExists(int Id);
    }
}
