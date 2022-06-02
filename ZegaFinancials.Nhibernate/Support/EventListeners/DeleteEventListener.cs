using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZegaFinancials.Nhibernate.Entities;
using ZegaFinancials.Nhibernate.Entities.Logging;

namespace ZegaFinancials.Nhibernate.Support.EventListeners
{
    public class DeleteEventListener : DefaultDeleteEventListener, IPostDeleteEventListener 
    {
        private readonly IAuditEventListener _auditEventListener;
        public DeleteEventListener(IAuditEventListener auditEventListener)
        {
            _auditEventListener = auditEventListener;
        }

        protected override void DeleteEntity(IEventSource session, object entity, EntityEntry entityEntry, bool isCascadeDeleteEnabled, IEntityPersister persister, ISet<object> transientEntities)
        {
            var deletedAt = DateTime.Now;
            if (entity is IsOfDeleteEntity softEntity)
            {
                softEntity.IsDeleted = true;
                softEntity.DeletedAt = deletedAt;

                CascadeBeforeDelete(session, persister, entity, entityEntry, transientEntities);
                CascadeAfterDelete(session, persister, entity, transientEntities);

                var entityType = entity.GetType();

                var audit = new AuditLog
                {
                    Date = deletedAt,
                    EntityType = _auditEventListener.GetMessageType(entityType),
                    Message =
                        $"{entityType.Name} \"{_auditEventListener.GetSignature((ZegaEntity)entity)}\" was marked as deleted.",
                };
                _auditEventListener.SaveAudit(session, audit);
            }
            else
                base.DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled, persister, transientEntities);

        }

        public void OnPostDelete(PostDeleteEvent @event)
        {
            var entityType = @event.Entity.GetType();

            var argCount = entityType.CustomAttributes.FirstOrDefault()?.ConstructorArguments?.Count;
            if (argCount != null && argCount > 1)
            {
                var isLogRequire = (bool)entityType.CustomAttributes.FirstOrDefault().ConstructorArguments[1].Value;
                if (!isLogRequire)
                    return;
            }
            if (!_auditEventListener.IsAcceptable(entityType, @event.Persister.PropertyNames.ToList()))
                return;
            var entityName = entityType.Name.Replace("Entity", string.Empty);
            var message =
                $"{entityName} \"{_auditEventListener.GetSignature(@event.Entity as ZegaEntity)}\" was deleted.";
            var audit = new AuditLog
            {
                Date = DateTime.Now,
                EntityType = _auditEventListener.GetMessageType(entityType),
                Message = message,
            };

            _auditEventListener.SaveAudit(@event.Session, audit);
        }

        public async Task OnPostDeleteAsync(PostDeleteEvent @event, CancellationToken cancellationToken)
        {
            var entityType = @event.Entity.GetType();

            var argCount = entityType.CustomAttributes.FirstOrDefault().ConstructorArguments.Count;
            if (argCount > 1)
            {
                var isLogRequire = (bool)entityType.CustomAttributes.FirstOrDefault().ConstructorArguments[1].Value;
                if (!isLogRequire)
                    return;
            }

            if (!_auditEventListener.IsAcceptable(entityType, @event.Persister.PropertyNames.ToList()))
                return;
            var entityName = entityType.Name.Replace("Entity", string.Empty);

            var message =
                 $"{entityName} \"{_auditEventListener.GetSignature(@event.Entity as ZegaEntity)}\" was deleted.";

            var audit = new AuditLog
            {
                Date = DateTime.Now,
                EntityType = _auditEventListener.GetMessageType(entityType),
                Message = message,
            };

            await _auditEventListener.SaveAuditAsync(@event.Session, audit, cancellationToken);
        }
    }
}
