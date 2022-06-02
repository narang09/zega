using NHibernate;
using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Dao.Interface.DataGrid;
using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Dao.Impl.DataGrid
{
    public class DataGridDao : NHibernateDao<GridConfig>, IDataGridDao
    {       
        public DataGridDao (ISession session): base (session)
        {  }        

        public IEnumerable<GridConfig> GetGridConfig(DataRequestSource grid, int userId)
        {
            return _session.CreateQuery(@"
FROM GridConfig WHERE GridType =: gridType AND ((IsDefault = 1 AND User IS NULL) OR (IsDefault = 0 AND User =: userId))")
                .SetParameter("gridType", grid)
                .SetParameter("userId", userId)
                .List<GridConfig>();
        }

    }
}


