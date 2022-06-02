
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZegaFinancials.Nhibernate.Entities;
using ZegaFinancials.Nhibernate.Entities.Logging;

namespace ZegaFinancials.Nhibernate.Support.EventListeners
{
    public interface IAuditEventListener
    {
        Dictionary<Type,LogInfo> Types { get; }

         void Enable();
        void Disable();
        void SaveAudit(ISession session, AuditLog audit);
        Task SaveAuditAsync(ISession session, AuditLog audit, CancellationToken cancellation);
        void UpdateStateOfEvent(IEntityPersister persister, object[] state, string propertyName, object value);
        bool IsAcceptable([NotNull] Type entityType, [NotNull] IList<string> propertyNames);
        string GetSignature([CanBeNull] ZegaEntity entity);
        EntityType GetMessageType([NotNull] Type entityType);
    }
}
