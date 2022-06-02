using System;
using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Shared.Enums;

namespace ZegaFinancials.Nhibernate.Shared.Models
{
    public class FilterExpressionModel
    {
        Guid _obj = Guid.NewGuid();
        public FilterExpressionModel()
        {

            DateConfig = new DateConfigModel();
            Values = new Dictionary<string, string> { { "Value1", "" }, { "Value2", "" }, { "Value3", "" }, { "Value4", "" }, { "Value5", "" } };
            StringConditionType = StringConditionTypes.Contains;
            NumericConditionType = NumericConditionTypes.Equals;
            DateTimeConditionType = DateTimeConditionTypes.WithInLast;
            SelectedValues = new List<int>();
        }

        public string ExpressionGuid => _obj.ToString().Substring(0, 8);
        public List<int> SelectedValues { get; set; }
        public Dictionary<string, string> Values { get; set; }
        public StringConditionTypes StringConditionType { get; set; }
        public NumericConditionTypes NumericConditionType { get; set; }
        public DateTimeConditionTypes DateTimeConditionType { get; set; }
        public DateConfigModel DateConfig { get; set; }
    }
}
