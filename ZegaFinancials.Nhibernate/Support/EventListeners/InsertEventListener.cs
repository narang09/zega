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
    public class InsertEventListener : IPreInsertEventListener, IPostInsertEventListener
    {
        private readonly IAuditEventListener _auditEventListener;
        public InsertEventListener(IAuditEventListener auditEventListener)
        {
            _auditEventListener = auditEventListener;
        }

       public bool OnPreInsert(PreInsertEvent @event)
        {
            var audit = @event.Entity as IAuditableEntity;
            if (audit == null)
                return false;

            audit.UpdatedAt = audit.CreateDate = DateTime.Now;
            _auditEventListener.UpdateStateOfEvent(@event.Persister, @event.State, "UpdatedAt", audit.UpdatedAt);
            _auditEventListener.UpdateStateOfEvent(@event.Persister, @event.State, "CreateDate", audit.CreateDate);

            return false;
        }

        public Task<bool> OnPreInsertAsync(PreInsertEvent @event, CancellationToken _) => Task.FromResult(OnPreInsert(@event));

        public void OnPostInsert(PostInsertEvent @event)
        {
           
            var entityType = @event.Entity.GetType();

            if (!_auditEventListener.IsAcceptable(entityType, @event.Persister.PropertyNames.ToList()))
                return;
            var hs = _auditEventListener.Types[entityType];
            var sb = new StringBuilder();
            sb.AppendFormat("New {0} \"{1}\" was created", entityType.Name.Replace("Entity", string.Empty), _auditEventListener.GetSignature((ZegaEntity)@event.Entity));
            var changedProperties = new List<string>();
            var state = @event.State.ToList();

            for (var i = 0; i < @event.State.Length; i++)
                if (hs.PropertyIndexes.Contains(i) && hs.ShowValues.Contains(i))
                {
                    if (@event.State[i] is ZegaEntity)
                    {
                        var newVal = _auditEventListener.GetSignature(@event.State[i] as ZegaEntity);
                        changedProperties.Add($"{@event.Persister.PropertyNames[i]} = {newVal ?? "Empty"}");
                    }
                    else if (state[i] != null)
                        changedProperties.Add($"{@event.Persister.PropertyNames[i]} = {state[i]}");
                }
            sb.AppendFormat("({0}).", string.Join(", ", changedProperties));
            var entity = (ZegaEntity)@event.Entity;
            var audit = new AuditLog
            {
                Date = DateTime.Now,
                EntityType = _auditEventListener.GetMessageType(entityType),
                Message = sb.ToString(),
            };

            if (entity.Id != 0)
            {
                audit.EntityId = entity.Id;
                audit.Type = entityType.Name.Replace("Entity", string.Empty);
                audit.EntityIndentifier = _auditEventListener.GetSignature(entity);
            }

            _auditEventListener.SaveAudit(@event.Session, audit);
        }

        public async Task OnPostInsertAsync(PostInsertEvent @event, CancellationToken cancellation)
        {
            var entityType = @event.Entity.GetType();

            if (!_auditEventListener.IsAcceptable(entityType, @event.Persister.PropertyNames.ToList()))
                return;
            var hs = _auditEventListener.Types[entityType];
            var sb = new StringBuilder();
            sb.AppendFormat("New {0} \"{1}\" was created", entityType.Name.Replace("Entity", string.Empty), _auditEventListener.GetSignature((ZegaEntity)@event.Entity));
            var changedProperties = new List<string>();
            var state = @event.State.ToList();

            for (var i = 0; i < @event.State.Length; i++)
                if (hs.PropertyIndexes.Contains(i) && hs.ShowValues.Contains(i))
                {
                    if (@event.State[i] is ZegaEntity)
                    {
                        var newVal = _auditEventListener.GetSignature(@event.State[i] as ZegaEntity);
                        changedProperties.Add($"{@event.Persister.PropertyNames[i]} = {newVal ?? "Empty"}");
                    }
                    else if (state[i] != null)
                        changedProperties.Add($"{@event.Persister.PropertyNames[i]} = {state[i]}");
                }

            sb.AppendFormat("({0}).", string.Join(", ", changedProperties));

            var entity = (ZegaEntity)@event.Entity;
            var audit = new AuditLog
            {
                Date = DateTime.Now,
                EntityType = _auditEventListener.GetMessageType(entityType),
                Message = sb.ToString(),
            };

            if (entity.Id != 0)
            {
                audit.EntityId = entity.Id;
                audit.Type = entityType.Name.Replace("Entity", string.Empty);
                audit.EntityIndentifier = _auditEventListener.GetSignature(entity);
            }

            await _auditEventListener.SaveAuditAsync(@event.Session, audit, cancellation);
        }
    }

}

