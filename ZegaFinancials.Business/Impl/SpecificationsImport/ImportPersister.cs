using Microsoft.Extensions.Logging;
using ZegaFinancials.Business.Interfaces.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport.ImportData;

namespace ZegaFinancials.Business.Impl.SpecificationsImport
{
    public partial class ImportPersister : IImportPersister
    {
        ImportPersistResult importPersistResult;
        public ImportPersistResult Persist(ImportDataItem<AccountFileRowData> importData, out bool isDataPersist, int totalRowsInfile = 0)
        {
            isDataPersist = false;
            if(importData == null)
                return null;
            var importCache = new ImportCache();
            _logger.LogInformation("Getting Accounts Data");
            var hasAnyAccounts = ExtractAccountsUploadData(importData, importCache,totalRowsInfile);
            if (hasAnyAccounts)
            {
                _logger.LogInformation("Start Saving process for accounts data.");

                _accountDao.Persist(importCache.GetAccounts());
                _logger.LogInformation("Done Saving process for accounts data.");
                isDataPersist = true;
            }
            else if (importData.Messages.Count > 0)
                importPersistResult.ErrorMsg = importData.Messages;
            _logger.LogInformation("Saved Accounts Data");

            return importPersistResult;
        }
        public ImportPersistResult Persist(ImportDataItem<AccountImportData> importData, out bool isDataPersist)
        {
            isDataPersist = false;
            if (importData == null)
                return null;

            var importCache = new ImportCache();

            _logger.LogInformation("Getting Accounts Data");
            var hasAnyAccounts = ExtractAccountsImportData(importData, importCache);
            if (hasAnyAccounts)
            {
                _logger.LogInformation("Start Saving process for accounts data.");

                _accountDao.Persist(importCache.GetAccounts());
                _logger.LogInformation("Done Saving process for accounts data.");               
                isDataPersist = true;
            }

            _logger.LogInformation("Saved Accounts Data");

            return importPersistResult;
        }

    }
}
