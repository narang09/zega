using ZegaFinancials.Nhibernate.Entities.Logging;

namespace ZegaFinancials.Nhibernate.Mappings.Logging
{
    public class AuditLogMap : ZegaMap<AuditLog>
    {
        public AuditLogMap()
        {
            Map(x => x.Message).Length(4001);
            Map(x => x.EntityType).CustomType<EntityType>();
            Map(x => x.EntityId);
            Map(x => x.Date);
            Map(x => x.UserLogin);
        }
    }
}
