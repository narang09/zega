using System;
using ZegaFinancials.Nhibernate.Entities.Logging;

namespace ZegaFinancials.Nhibernate.Support.EventListeners
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class LogableAttribute : Attribute
    {
        public EntityType EntityType { get; set; }
        public bool LogAllFields { get; set; }
        public bool IsLogRequire { get; set; }

        public LogableAttribute(EntityType entityType)
            : this(entityType, true, true)
        { }

        public LogableAttribute(EntityType entityTpe, bool isLogRequire)
            : this(entityTpe, true, isLogRequire)
        { }

        public LogableAttribute(EntityType entityType, bool logAllFields, bool isLogRequire = true)
        {
            EntityType = entityType;
            LogAllFields = logAllFields;
            IsLogRequire = isLogRequire;
        }
    }
}
