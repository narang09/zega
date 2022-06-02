using System;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Entities.Logging
{
    public class AuditLog : ZegaEntity
    {
        public virtual string Message { get; set; }
        public virtual EntityType EntityType { get; set; }
        public virtual int EntityId { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual string UserLogin { get; set; }
        public virtual string EntityIndentifier { get; set; }

        public virtual string Type { get; set; }

        public AuditLog()
        {
            Date = DateTime.Now;
        }

    }
}
