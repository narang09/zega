using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Models;
using ZegaFinancials.Services.Models.Accounts;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Interfaces.Accounts
{
    public interface IAccountService
    {
        DataGridModel LoadAccountsByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext);
        AccountReponseModel SaveAccountBasicInfo(AccountInfoModelcs accountModel, UserContextModel userContext);
        AccountInfoModelcs GetAccountById(int accountId, UserContextModel userContext);
        void BulkEditAccounts(BulkEditModel bulkChanges, UserContextModel userContext);
        void DeleteAccountByIds(int[] accountIds , UserContextModel userContext);
        string GetAdvisorsNameByRepCodeId( int repCodeId ,UserContextModel userContext);
        IEnumerable<ZegaModel> GetRepCodesListByAdvisorIds(int[] advisorIds);
        void SaveAccountModelDetails(AccountInfoModelcs accountModel, UserContextModel userContext);
        void SaveAccountAdditionalWithdrawals(AccountInfoModelcs accountModel, UserContextModel userContext);
        void SaveAccountAdditionalDeposits(AccountInfoModelcs accountModel, UserContextModel userContext);
        void SaveAccountZegaCustomFields(AccountInfoModelcs accountModel, UserContextModel userContext);
        AccountModel[] LoadAllAccounts();
        Dictionary<string, Dictionary<string, decimal>> GetByFilterWithSleevePercent(string[] sleeveIds);
        IEnumerable<ZegaModel> GetAllRepCodes();
        List<BrokerModel> GetBrokerByAccountId(int id);
    }
}
