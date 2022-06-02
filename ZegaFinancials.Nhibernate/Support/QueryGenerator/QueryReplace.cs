using System;
using JetBrains.Annotations;

namespace ZegaFinancials.Nhibernate.Support.QueryGenerator
{
    public class QueryReplace
    {
        [NotNull]
        private readonly string _propertyName;
        [NotNull]
        private readonly string _field;

        public QueryReplace([NotNull] string propertyName, [NotNull] string field) : this(propertyName, field, false) { }

        public QueryReplace([NotNull] string propertyName, [NotNull] string field, bool isIdUsable = false)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (field == null) throw new ArgumentNullException("field");
            _propertyName = propertyName;
            _field = field;            
            IsIdUsable = isIdUsable;
        }

        [NotNull]
        public string PropertyName
        {
            get { return _propertyName; }
        }

        [NotNull]
        public string Field
        {
            get { return _field; }
        }
       
        public bool IsIdUsable { get; set; }
    }
}
