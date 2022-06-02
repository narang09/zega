using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support;

namespace ZegaFinancials.Web.Models.DataGrid
{
    public class GridHeaderModel
    {
        public GridHeaderModel()
        {
            GridHeaders = new List<DataGridColumnObject>();
            SortDescriptions = new List<SortDescription>();
        }

        public List<DataGridColumnObject> GridHeaders { get; set; }
        public List<SortDescription> SortDescriptions { get; set; }
    }
}
