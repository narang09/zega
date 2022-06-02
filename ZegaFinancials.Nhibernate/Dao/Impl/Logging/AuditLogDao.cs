using NHibernate;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Logging;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support.QueryGenerator;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Logging
{
    public class AuditLogDao : NHibernateDao<AuditLog>, IAuditLogDao
    {
        public AuditLogDao(ISession session) : base(session)
        { }
        public IEnumerable<AuditLog> GetAuditLogByFilter(DataGridFilterModel model,string userLogin, bool IsAdmin, out int count)
        {
            var sql = @"
FROM AuditLog AS al WHERE ";

            var qg = new QueryGenerator("al", model, new[]
            {
                new QueryReplace("EntityTypeValue", "al.EntityType"),
                new QueryReplace("Name","al.UserLogin")
            },new Dictionary<string, string> { { "EntityTypeValue", "EntityType" } });

            var param = new Dictionary<string, object>();
            if (IsAdmin)
            {
                sql += "1 =: admin";
                param.Add("admin", IsAdmin);
            }
                 
            else
            {
                sql += "al.UserLogin =: userLogin ";
                param.Add("userLogin", userLogin);

            }
             var qb = qg.ApplyBasicMultiFilters<AuditLog>(_session, sql, param);
            var logs = qb.GetResult<AuditLog>(0);
            count = (int)qb.GetResult<long>(1).Single();
            return logs;
        }
    }
}
