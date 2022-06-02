
using System;

namespace ZegaFinancials.Nhibernate.Support.EventListeners
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LogAttribute : Attribute
    {
        public bool Use { get; set; }
        public bool ShowValue { get; set; }

        public LogAttribute()
            : this(true)
        { }

        public LogAttribute(bool use)
            : this(use, true)
        {
        }

        public LogAttribute(bool use, bool showValue)
        {
            Use = use;
            ShowValue = showValue;
        }
    }
}
