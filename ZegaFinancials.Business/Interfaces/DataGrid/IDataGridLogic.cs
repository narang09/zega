using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support;
using ZegaFinancials.Business.Impl.DataGrid;
using ZegaFinancials.Business.Shared.Models;

namespace ZegaFinancials.Business.Interfaces.DataGrid
{
    public interface IDataGridLogic
    {   
        GridConfig GetGridHeaders(DataRequestSource grid, int userId);
        byte[] ExportToCsv(IEnumerable<object> entityList, IEnumerable<DataGridColumnObject> columns, HashSet<string> unsupportedList, DataGridLogic.GetCustomFieldValue handler);
        void Persist(GridConfig headerInfo);
        DataGridPreferenceModel GetGridByPreferences(DataRequestSource grid, int userId);
    }
}      
