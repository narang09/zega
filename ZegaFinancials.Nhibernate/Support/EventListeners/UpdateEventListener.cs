using NHibernate.Event;
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
    public class UpdateEventListener : IPreUpdateEventListener , IPostUpdateEventListener
    {
        private readonly IAuditEventListener _auditEventListener;
        public UpdateEventListener(IAuditEventListener auditEventListener)
        {
            _auditEventListener = auditEventListener;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            if (!(@event.Entity is IAuditableEntity audit))
                return false;

            audit.UpdatedAt = DateTime.Now;
            _auditEventListener.UpdateStateOfEvent(@event.Persister, @event.State, "UpdatedAt", audit.UpdatedAt);

            return false;
        }

        public Task<bool> OnPreUpdateAsync(PreUpdateEvent @event, CancellationToken _) => Task.FromResult(OnPreUpdate(@event));

        public void OnPostUpdate(PostUpdateEvent @event)
        {
            var audit = CreateLogEntry(@event);

            if (audit != null)
                _auditEventListener.SaveAudit(@event.Session, audit);
        }

        public async Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellation)
        {
            var audit = CreateLogEntry(@event);

            if (audit != null)
                await _auditEventListener.SaveAuditAsync(@event.Session, audit, cancellation);
        }

        private AuditLog CreateLogEntry(PostUpdateEvent @event)
        {
            var entityType = @event.Entity.GetType();

            if (!_auditEventListener.IsAcceptable(entityType, @event.Persister.PropertyNames.ToList()))
                return null;

            var hs = _auditEventListener.Types[entityType];
            var entity = @event.Entity as ZegaEntity;
            var sb = new StringBuilder();
            sb.AppendFormat("{0} \"{1}\" was modified", entityType.Name.Replace("Entity", string.Empty), _auditEventListener.GetSignature(entity));

            var changedProperties = new List<string>();
            if (@event.OldState != null && @event.State != null)
            {
                for (var i = 0; i < @event.State.Length; i++)
                {
                    if (hs.PropertyIndexes.Contains(i) && !Equals(@event.State[i], @event.OldState[i]))
                    {
                        if (hs.ShowValues.Contains(i))
                        {
                            if (@event.State[i] is ZegaEntity || @event.OldState[i] is ZegaEntity)
                            {
                                var newVal = _auditEventListener.GetSignature(@event.State[i] as ZegaEntity);
                                var oldVal = _auditEventListener.GetSignature(@event.OldState[i] as ZegaEntity);

                                changedProperties.Add(
                                    $"{@event.Persister.PropertyNames[i]} changed from \"{oldVal ?? "[Empty]"}\" to \"{newVal ?? "[Empty]"}\"");
                            }
                            else
                            {
                                var newVal = @event.State[i];
                                var oldVal = @event.OldState[i];

                                if (newVal is string val && string.IsNullOrEmpty(val))
                                    newVal = null;
                                if (oldVal is string val2 && string.IsNullOrEmpty(val2))
                                    oldVal = null;
                                if (newVal != null || oldVal != null)
                                    changedProperties.Add(
                                        $"{@event.Persister.PropertyNames[i]} changed from \"{oldVal ?? "[Empty]"}\" to \"{newVal ?? "[Empty]"}\"");
                            }
                        }
                        else
                            changedProperties.Add($"{@event.Persister.PropertyNames[i]} has been changed");
                    }
                }

            }
            if (changedProperties.Count <= 0)
                return null;
            sb.AppendFormat("({0}).", string.Join(", ", changedProperties));

            var audit = new AuditLog
            {
                Date = DateTime.Now,
                EntityType = _auditEventListener.GetMessageType(entityType),
                Message = sb.ToString(),
            };

            if (entity != null && entity.Id != 0)
            {
                audit.EntityId = entity.Id;
                audit.Type = entityType.Name.Replace("Entity", string.Empty);
                audit.EntityIndentifier = _auditEventListener.GetSignature(entity);
            }

            return audit;
        }
    }
}
