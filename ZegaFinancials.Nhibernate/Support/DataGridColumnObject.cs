using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Shared.Models;

namespace ZegaFinancials.Nhibernate.Support
{
    public class DataGridColumnObject
    {
        public DataGridColumnObject()
        {
            IsVisible = true;
            IsSortingEnabled = true;
            IsFilteringEnabled = true;
            FilterExpressions = new();
            DataValidators = new();
        }

        public string DisplayName { get; set; }
        public string DataField { get; set; }
        public bool IsVisible { get; set; }
        public bool IsSortingEnabled { get; set; }
        public bool IsFilteringEnabled { get; set; }
        public float MinWidth { get; set; }
        public int DisplayIndex { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEditable { get; set; }
        public GridColumnTypes Type { get; set; }
        public GridTemplateTypes TemplateType { get; set; }
        public bool IsFilterAdded { get; set; }
        public List<FilterExpressionModel> FilterExpressions { get; set; }
        public List<string> DataValidators { get; set; }
        public Dictionary<string, int> EnumValues { get; set; }
    }
}
