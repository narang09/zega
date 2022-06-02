using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Dao.Interface.DataGrid
{
    public interface IDataGridDao : INHibernateDao<GridConfig>
    {
        IEnumerable<GridConfig> GetGridConfig(DataRequestSource grid, int userId);
    }
}
