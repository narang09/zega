using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Support;
using System.Linq;

namespace ZegaFinancials.Nhibernate.Entities.Shared
{
    public class DataGridFilterModel
    {
        public DataGridFilterModel()
        {
            SortDescriptions = new List<SortDescription>();
            DataGridColumnObjects = new List<DataGridColumnObject>();
        }
        public virtual DataRequestSource RequestSource { get; set; }
        public virtual string GridName { get; set; }
        public List<SortDescription> SortDescriptions { get; set; }
        public string SearchText { get; set; }
        public int StartIndex { get; set; }
        public int PaginationSize { get; set; }
        public bool IsPaginationActive { get; set; }
        public List<DataGridColumnObject> DataGridColumnObjects { get; set; }
        public GridAdditionalParameters GridAdditionalParameters { get; set; }
        public string[] VisibleColumns
        {
            get { return DataGridColumnObjects.Where(col => col.IsVisible).Select(col => col.DataField).ToArray(); }
        }

    }
}
