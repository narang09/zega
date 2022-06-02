using System.Collections.Generic;
using ZegaFinancials.Business.Impl.DataGrid;
using ZegaFinancials.Business.Shared.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Interfaces.Support
{
    public interface IDataGridService
    {
        DataGridPreferenceModel GetDataGridHeader(DataRequestSource grid, UserContextModel userContext);
        byte[] ExportToCsv(IEnumerable<object> entityList, IEnumerable<DataGridColumnObject> columns, HashSet<string> unsupportedList, DataGridLogic.GetCustomFieldValue handler, UserContextModel userContext);
        void SaveDataGridPrefernces(DataRequestSource gridType, DataGridPreferenceModel gridPreferenceModel);
    }
}
