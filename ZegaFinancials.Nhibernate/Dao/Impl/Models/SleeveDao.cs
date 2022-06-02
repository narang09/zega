using NHibernate;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Models;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support.QueryGenerator;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Models
{
    public class SleeveDao : NHibernateDao<Sleeve>, ISleeveDao
    {
        public SleeveDao(ISession session) : base(session)
        { }
        public IEnumerable<Sleeve> GetSleevesByFilter(DataGridFilterModel model, out int count)
        {  
            var sql = "FROM Sleeve AS s WHERE 1=1";

            var qg = new QueryGenerator("s", model);
            var qb = qg.ApplyBasicMultiFilters<Sleeve>(_session, sql);

            var sleeves = qb.GetResult<Sleeve>(0);
            count = (int)qb.GetResult<long>(1).Single();
            return sleeves;
        }
        public virtual bool IsExists(int sleeveId, string name)
        {
            var query = _session.CreateQuery(@"
	SELECT COUNT(*) FROM
	Sleeve
	WHERE Id <> :sId AND Name = :Name")
                .SetParameter("sId", sleeveId)
                .SetParameter("Name", name)
                .UniqueResult<long>();

            return query > 0;
        }
        public virtual bool IsExists(int Id)
        {
            var query = _session.CreateQuery(@"
	SELECT COUNT(*) FROM
	Sleeve
	WHERE Id = :Id")
                .SetParameter("Id", Id)
                .UniqueResult<long>();

            return query > 0;
        }
       
    }
}
