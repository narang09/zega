using System.Collections.Generic;
using System.IO;
using ZegaFinancials.Business.Support.SpecificationsImport.Model;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Models;
using ZegaFinancials.Services.Models.Import;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Interfaces.SpecificationsImport
{
    public interface ISpecificationsImportService
    {
        ImportResult Import(int profileId, UserContextModel userContext);
        IEnumerable<ZegaModel> GetRepCodesList(UserContextModel userContext);
        ImportProfileModel GetImportProfile(UserContextModel userContext);
        int SaveImportProfile(ImportProfileModel profileModel, UserContextModel userContext);
        DataGridModel LoadImportHistoryByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext);
        ImportResult ReadFileAndImportAccounts(Stream ms,string fileName, UserContextModel userContext);
    }
}
