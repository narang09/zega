using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ZegaFinancials.Nhibernate.Entities;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Support.EventListeners
{
    public class AuditLogEventListener : IAuditEventListener
    {

        [NotNull]
        private readonly HashSet<Type> _unusedTypes = new();

        private static readonly Dictionary<Type, PropertyInfo> _signatures = new();
        [NotNull]
        public Dictionary<Type, LogInfo> Types { get; } = new();
        public void SaveAudit(ISession session, AuditLog audit)
        {
           
            if (Thread.GetData(Thread.GetNamedDataSlot("UserId")) != null)
            {
                var user = session.Get<User>((int)Thread.GetData(Thread.GetNamedDataSlot("UserId")));
                audit.UserLogin = $"{user.Name}{user.Login}";
            }

            audit.Message = audit.Message.Replace("\r", "");
            audit.Message = audit.Message.Replace("\n", "");
            using (var childSession = session.SessionFactory.OpenStatelessSession())
            {
                using (var transaction = childSession.BeginTransaction())
                {
                    childSession.Insert(audit);
                    transaction.Commit();
                }
            }
        }

        public async Task SaveAuditAsync(ISession session, AuditLog audit, CancellationToken cancellation)
        {
            if (Thread.GetData(Thread.GetNamedDataSlot("UserId")) != null)
            {
                var user = await session.GetAsync<User>((int)Thread.GetData(Thread.GetNamedDataSlot("UserId")), cancellation);
                audit.UserLogin = $"{user.Name}({user.Login})";
            }

            audit.Message = audit.Message.Replace("\r", "");
            audit.Message = audit.Message.Replace("\n", "");
            await session.PersistAsync(audit, cancellation);

            await session.FlushAsync(cancellation);
        }

        public void Enable()
        {
            Thread.SetData(Thread.GetNamedDataSlot("AuditEventListenerEnableLog"), true);
        }

        public void Disable()
        {
            Thread.SetData(Thread.GetNamedDataSlot("AuditEventListenerEnableLog"), false);
        }

        private bool LogEnabled
        {
            get
            {
                var obj = Thread.GetData(Thread.GetNamedDataSlot("AuditEventListenerEnableLog"));
                return obj == null || (bool)obj;
            }
        }

        public void UpdateStateOfEvent(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
        [CanBeNull]
        private bool? IsAcceptable([NotNull] Type entityType)
        {

            bool? acceptable = null;
            if (Types.ContainsKey(entityType))
                acceptable = true;
            else if (_unusedTypes.Contains(entityType))
                acceptable = false;

            return acceptable;
        }

        public EntityType GetMessageType([NotNull] Type entityType)
        { 

            var attrs = entityType
                .GetCustomAttributes(typeof(LogableAttribute), true)
                .Cast<LogableAttribute>()
                .FirstOrDefault();

            return attrs.EntityType;
        }

        public bool IsAcceptable([NotNull] Type entityType, [NotNull] IList<string> propertyNames)
        {

            if (!LogEnabled || entityType.IsAbstract)
                return false;

            if (IsAcceptable(entityType) == null)
                InitTypes(entityType, propertyNames);

            return IsAcceptable(entityType) == true;
        }
        private void InitTypes([NotNull] Type entityType, [NotNull] IList<string> propertyNames)
        {

            var logableAttribute = entityType.GetCustomAttributes(typeof(LogableAttribute), false).Cast<LogableAttribute>().SingleOrDefault();

            if (logableAttribute == null)
            {
                _unusedTypes.Add(entityType);
                return;
            }

            var logAll = logableAttribute.LogAllFields;

            var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            Types.Add(entityType, new LogInfo());

            foreach (var p in props)
            {
                var logsAttribute = p.GetCustomAttributes(typeof(LogAttribute), true).Cast<LogAttribute>().FirstOrDefault();


                var useProp = (logAll && (logsAttribute?.Use == true || logsAttribute == null)
                                || !logAll && logsAttribute?.Use == true);

                var showValue = logsAttribute?.ShowValue != true;

                if (!useProp) continue;

                var index = propertyNames.IndexOf(p.Name);
                Types[entityType].PropertyIndexes.Add(index);

                if (showValue)
                    Types[entityType].ShowValues.Add(index);
            }
        }
        [CanBeNull]
        public virtual string GetSignature([CanBeNull] ZegaEntity entity)
        {
            if (entity == null)
                return null;

            var entityType = entity.GetType();
            lock (_signatures)
            {
                if (!_signatures.ContainsKey(entityType))
                {
                    var props = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo pi = null;

                    if (entityType.GetCustomAttributes(typeof(LogableAttribute), true).Any())
                        foreach (var prop in props)
                            if (prop.GetCustomAttributes(typeof(LogSignatureAttribute), true).Any())
                            {
                                pi = prop;
                                break;
                            }

                    if (pi == null)
                        pi = props.FirstOrDefault(o => o.Name == "Id");

                    if (pi != null)
                        _signatures.Add(entityType, pi);
                }

                var value = _signatures[entityType].GetValue(entity, null);

                if (value is ZegaEntity zegaEntity)
                    value = GetSignature(zegaEntity);

                return value?.ToString();
            }
        }
    }
}