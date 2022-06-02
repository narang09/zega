using ZegaFinancials.Business.Support.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport.ImportData;

namespace ZegaFinancials.Business.Interfaces.SpecificationsImport
{
    public interface IImportPersister
    {
        /// <summary>
		/// Persists the data imported.
		/// </summary>
		/// <param name="importData">The import data.</param>
		/// <returns></returns>
        ImportPersistResult Persist(ImportDataItem<AccountImportData> importData, out bool isDataPersist);
        ImportPersistResult Persist(ImportDataItem<AccountFileRowData> importData, out bool isDataPersist, int totalRowsInfile = 0);
    }
}
