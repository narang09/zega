using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Business.Interfaces.Accounts;
using ZegaFinancials.Business.Interfaces.Models;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Interfaces.Accounts;
using ZegaFinancials.Services.Models;
using ZegaFinancials.Services.Models.Accounts;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Impl.Accounts
{
    public class AccountService : ZegaService, IAccountService
    {
        private readonly IAccountLogic _accountLogic;
        private readonly IModelLogic _modelLogic;

        public AccountService(IAccountLogic accountLogic, IModelLogic modelLogic, IUserLogic userLogic) : base(userLogic)
        {
            _accountLogic = accountLogic;
            _modelLogic = modelLogic;
        }

        public DataGridModel LoadAccountsByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext)
        {
            int count;
            CheckUserContext(userContext);
            List<int> repcodeIds = userContext.IsAdmin ? new List<int>() : _userLogic.GetRepCodesByAdvisorId(userContext.Id)?.Select(x => x.Id)?.ToList();
            var accounts = _accountLogic.GetAccountsByFilter(dataGridFilterModel, repcodeIds, userContext.IsAdmin, out count);

            var dataGridModel = new DataGridModel();
            dataGridModel.Accounts = GetAccountModelList(accounts,userContext).ToArray();
            dataGridModel.TotalRecords = count;

            return dataGridModel;
        }

        [NotNull]
        private IList<AccountInfoModelcs> GetAccountModelList([NotNull] IEnumerable<Account> accounts ,UserContextModel userContext)
        {
            if (accounts == null)
                throw new ArgumentNullException("accounts");
            var accountList = new List<AccountInfoModelcs>();
            var repCodeAdvisors = _userLogic.GetAllRepCodeAdvisors().GroupBy(o => o.RepCode.Id).ToDictionary(o => o.Key, o => o.Select(a => a.Advisor));
            foreach (var account in accounts)
            {
                var ma = new AccountInfoModelcs();
                ma.Id = account.Id;
                ma.Model = account.Model != null ? new CommonModel() { Id = account.Model.Id, Name = account.Model.Name } : null;
                ma.Name = account.Name;
                ma.accountBasicDetails.Number = account.Number;
                ma.accountBasicDetails.ClientName = account.ClientName;
                ma.accountBasicDetails.RepCode = account.RepCode != null ? new CommonModel() { Id = account.RepCode.Id, Name = account.RepCode.Code } : null;
                if (account.AccountDetail != null)
                {
                    ma.accountBasicDetails.AccountType = account.AccountDetail.AccountType;
                    ma.accountBasicDetails.AccountStatus = account.AccountStatus;
                    ma.accountBasicDetails.Broker = account.Broker;
                    ma.accountBasicDetails.AllocationDate = account.AccountDetail.B2_Allocation_Start_Date;
                    ma.accounWithdrawlInfoModelcs.Withdrawl_Amount = account.AccountDetail.C1_Withdrawal_Amount;
                    ma.accounWithdrawlInfoModelcs.Withdrawl_Date = account.AccountDetail.C2_Withdrawal_Date;
                    ma.accounWithdrawlInfoModelcs.Future_Withdrawal = account.AccountDetail.Future_Withdrawal;
                    ma.accountDepositsInfoModelcs.Deposit_Frequency = account.AccountDetail.D4_Deposit_Frequency;
                    ma.accounWithdrawlInfoModelcs.One_Time_Withdrawal = account.AccountDetail.One_Time_Withdrawal;
                    ma.accounWithdrawlInfoModelcs.Withdrawl_Frequency = account.AccountDetail.C4_Withdrawal_Frequency;
                    ma.accounWithdrawlInfoModelcs.One_Time_Withdrawal_Amount = account.AccountDetail.One_Time_Withdrawal_Amount;
                    ma.accounWithdrawlInfoModelcs.One_Time_Withdrawal_Date = account.AccountDetail.One_Time_Withdrawal_Date;
                    ma.accountDepositsInfoModelcs.Deposit_Amount = account.AccountDetail.D1_Deposit;
                    ma.accountDepositsInfoModelcs.Deposit_Date = account.AccountDetail.D2_Deposit_Date;
                    ma.accountDepositsInfoModelcs.Deposit_Status = account.AccountDetail.D3_Deposit_Status;
                    ma.accountDepositsInfoModelcs.Deposit_Frequency = account.AccountDetail.D4_Deposit_Frequency;
                    ma.accountZegaCustomFieldsModel.Zega_Confirmed = account.AccountDetail.Z2_ZEGA_Confirmed;
                    ma.accountZegaCustomFieldsModel.Zega_Alert_Date = account.AccountDetail.Z3_ZEGA_Alert_Date;
                    ma.accountZegaCustomFieldsModel.Zega_Notes = account.AccountDetail.Z4_ZEGA_Notes;
                    ma.accountBasicDetails.OBuyingPower = account.AccountDetail.OBuyingPower;
                    ma.accountBasicDetails.SBuyingPower = account.AccountDetail.SBuyingPower;
                    ma.accountBasicDetails.VeoImportDate = account.AccountDetail.VeoImportDate;
                    ma.accountBasicDetails.CashNetBal = account.AccountDetail.CashNetBal;
                    ma.accountBasicDetails.CashEq = account.AccountDetail.CashEq;
                    ma.accountBasicDetails.AccountValue = account.AccountDetail.AccountValue;
                    if (ma.accountBasicDetails.RepCode != null && repCodeAdvisors != null && repCodeAdvisors.ContainsKey(ma.accountBasicDetails.RepCode.Id))
                    {
                        var advisorDetails = repCodeAdvisors[ma.accountBasicDetails.RepCode.Id];
                        
                           ma.accountBasicDetails.AdvisorsName = userContext.IsAdmin ? string.Join(",", advisorDetails.Select(o => o.Details != null ? o.Details.FirstName + (o.Details.LastName != null ? o.Details.LastName : "") : "").ToList()) :
                        advisorDetails.FirstOrDefault(o => o.Id == userContext.Id)?.Details != null
                        ? advisorDetails.FirstOrDefault(o => o.Id == userContext.Id)?.Details.FirstName + (advisorDetails.FirstOrDefault(o => o.Id == userContext.Id)?.Details.LastName != null ? advisorDetails.FirstOrDefault(o => o.Id == userContext.Id)?.Details.LastName : "") : "";
                    }
                }
                accountList.Add(ma);
            }
            return accountList;
        }
        public AccountInfoModelcs GetAccountById(int accountId, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            Account account;
            AccountInfoModelcs accountModel = new();
            if (accountId != 0)
                account = _accountLogic.GetAccountById(accountId);
            else
                return new();
            Map(account, accountModel);
            accountModel.Model = account.Model != null ? new CommonModel() { Id = account.Model.Id, Name = account.Model.Name } : null;
            Map(account, accountModel.accountBasicDetails);
            if (account.AccountDetail != null)
            {
                accountModel.accountBasicDetails.AllocationDate = account.AccountDetail.B2_Allocation_Start_Date;
                accountModel.accountZegaCustomFieldsModel.Zega_Confirmed = account.AccountDetail.Z2_ZEGA_Confirmed;
                accountModel.accountZegaCustomFieldsModel.Zega_Alert_Date = account.AccountDetail.Z3_ZEGA_Alert_Date;
                accountModel.accountZegaCustomFieldsModel.Zega_Notes = account.AccountDetail.Z4_ZEGA_Notes;
                accountModel.accountDepositsInfoModelcs.Deposit_Amount = account.AccountDetail.D1_Deposit;
                accountModel.accountDepositsInfoModelcs.Deposit_Date = account.AccountDetail.D2_Deposit_Date;
                accountModel.accountDepositsInfoModelcs.Deposit_Status = account.AccountDetail.D3_Deposit_Status;
                accountModel.accountDepositsInfoModelcs.Deposit_Frequency = account.AccountDetail.D4_Deposit_Frequency;
                accountModel.accounWithdrawlInfoModelcs.Withdrawl_Amount = account.AccountDetail.C1_Withdrawal_Amount;
                accountModel.accounWithdrawlInfoModelcs.Withdrawl_Date = account.AccountDetail.C2_Withdrawal_Date;
                accountModel.accounWithdrawlInfoModelcs.Withdrawl_Frequency = account.AccountDetail.C4_Withdrawal_Frequency;
                accountModel.accounWithdrawlInfoModelcs.One_Time_Withdrawal_Date = account.AccountDetail.One_Time_Withdrawal_Date;
                accountModel.accountBasicDetails.Notes = account.AccountDetail?.Notes;
                accountModel.accountBasicDetails.RepCode = account.RepCode != null ? new CommonModel() { Id = account.RepCode.Id } : null;
                if (accountModel.accountBasicDetails.RepCode != null)
                {
                    var accountAdvisors = _userLogic.GetUsersByRepCodeIds(new List<int> { accountModel.accountBasicDetails.RepCode.Id });
                    if (userContext.IsAdmin)
                        accountModel.accountBasicDetails.AdvisorsName = string.Join(",", accountAdvisors?.Select(o => o.Details != null ? o.Details.FirstName + (o.Details.LastName != null ? o.Details.LastName : "") : "").ToList());
                    else
                    {
                        var accountAdvisor = accountAdvisors.FirstOrDefault(o => o.Id == userContext.Id);
                        accountModel.accountBasicDetails.AdvisorsName = accountAdvisor != null && accountAdvisor.Details != null ? accountAdvisor.Details.FirstName + (accountAdvisor.Details.LastName != null ? accountAdvisor.Details.LastName : "") : "";
                    }
                }
                Map(account.AccountDetail, accountModel.accounWithdrawlInfoModelcs);
                Map(account.AccountDetail, accountModel.accountBasicDetails);
            }
            return accountModel;
        }
        public void BulkEditAccounts(BulkEditModel bulkChanges, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            if (!bulkChanges.DataStoreIds.Any())
                return;
            var accounts = _accountLogic.GetAccountsByIds(bulkChanges.DataStoreIds.ToArray());
            var model = _modelLogic.GetModelById(bulkChanges.ModelId);
            foreach (var account in accounts)
            {
                if (bulkChanges.AccountStatus != null)
                    account.AccountStatus = (AccountStatus)bulkChanges.AccountStatus;
                if (bulkChanges.AccountType != null)
                    account.AccountDetail.AccountType = (AccountType)bulkChanges.AccountType;
                if (model != null)
                    account.Model = model;
            }
            _accountLogic.Persist(accounts);
        }
        public void DeleteAccountByIds(int[] accountIds, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            if (accountIds == null || !accountIds.Any())
                throw new ZegaServiceException("Account Not Selected!");
            foreach (var accountId in accountIds)
            {
                var account = _accountLogic.GetAccountById(accountId);
                if (account == null)
                    throw new ZegaServiceException("Invalid Account id");
                _accountLogic.DeleteAccountByid(accountId);

            }
        }
        public string GetAdvisorsNameByRepCodeId(int repCodeId , UserContextModel userContext)
        {
            var accountAdvisors = _userLogic.GetUsersByRepCodeIds(new[] { repCodeId });
            var advisorsName = "";
            if(userContext.IsAdmin)
                advisorsName = accountAdvisors != null ? string.Join(",", accountAdvisors.Select(o => o.Details != null ? o.Details.FirstName + (o.Details.LastName != null ? o.Details.LastName : "") : "").ToList()) :"";
            else
            {
                var accountAdvisor = accountAdvisors.FirstOrDefault(o => o.Id == userContext.Id);
                advisorsName = accountAdvisor != null && accountAdvisor.Details != null ? accountAdvisor.Details.FirstName + (accountAdvisor.Details.LastName != null ? accountAdvisor.Details.LastName : "") : "";
            }
            return advisorsName;
        }

        public IEnumerable<ZegaModel> GetAllRepCodes()
        {
            var repCodes = _userLogic.GetAllRepCodes();
            if (repCodes == null)
                return new List<ZegaModel>();
            return repCodes.Select(o => new ZegaModel() { Id = o.Id,Name = o.Code}).OrderBy(o =>o.Name);
        }
        public IEnumerable<ZegaModel> GetRepCodesListByAdvisorIds(int[] advisorIds)
        {
            if (advisorIds == null || !advisorIds.Any())
                return new List<ZegaModel>();
            var advisors = _userLogic.GetUsersByIds(advisorIds);
            var repCodeWithItsCount = advisors?.SelectMany(o => o.RepCodes)?.Select(x => x.RepCode)?.GroupBy(o => o).Select(x => new { key = x.Key, count = x.ToList().Count });
            var commonRepCodesAmongAdvisors = repCodeWithItsCount != null ? repCodeWithItsCount.Where(x => x.count == advisors.Count())?.Select(o => new ZegaModel { Id = o.key.Id, Name = o.key.Code }) : null;
            return commonRepCodesAmongAdvisors;
        }
        public AccountReponseModel SaveAccountBasicInfo(AccountInfoModelcs accountModel, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            Account account;
            if (accountModel.Id != 0)
                account = _accountLogic.GetAccountById(accountModel.Id);
            else
                account = _accountLogic.CreateAccountEntity();
            if (account != null)
            {
                if (account.AccountDetail == null)
                    account.AccountDetail = new AccountDetails();
                if (accountModel.accountBasicDetails != null)
                {
                    account.Name = accountModel.accountBasicDetails.Name;
                    account.ClientName = accountModel.accountBasicDetails.ClientName;
                   // account.AccountDetail.VeoImportDate = accountModel.accountBasicDetails.VeoImportDate; // No need to update this property , it's every time when import run.
                    account.AccountDetail.AccountType = accountModel.accountBasicDetails.AccountType;
                    account.AccountStatus = accountModel.accountBasicDetails.AccountStatus;
                    account.Broker = accountModel.accountBasicDetails.Broker;
                    if(string.IsNullOrEmpty(accountModel.accountBasicDetails.Number))
                        throw new ZegaServiceException("Account Number Can't null or empty.");
                    if ( accountModel.accountBasicDetails.Number.StartsWith("0"))
                        throw new ZegaServiceException("Account Number Can't Start with 0.");
                    account.Number = accountModel.accountBasicDetails.Number;
                    account.AccountDetail.Notes = accountModel.accountBasicDetails.Notes;
                    account.AccountDetail.CashEq = accountModel.accountBasicDetails.CashEq;
                    account.AccountDetail.CashNetBal = accountModel.accountBasicDetails.CashNetBal;
                    account.AccountDetail.SBuyingPower = accountModel.accountBasicDetails.SBuyingPower;
                    account.AccountDetail.OBuyingPower = accountModel.accountBasicDetails.OBuyingPower;
                    account.AccountDetail.AccountValue = accountModel.accountBasicDetails.AccountValue;
                    account.AccountDetail.B2_Allocation_Start_Date = accountModel.accountBasicDetails.AllocationDate;
                    if (accountModel.accountBasicDetails.RepCode == null)
                        throw new ZegaServiceException("RepCode Can't be null or empty.");
                    account.RepCode = _userLogic.GetRepCodeById(accountModel.accountBasicDetails.RepCode.Id);
                    if (account.RepCode == null)
                        throw new ZegaServiceException("Invalid RepCode.");
                    account.AccountDetail.Account = account;
                }
                _accountLogic.Persist(account);
                var advisorIds = _userLogic.GetUsersByRepCodeIds(new int[] {account.RepCode.Id }).Select(o =>o.Id).ToList();
                return new AccountReponseModel() { AccountId = account.Id, AvisorIds = advisorIds };
            }
            return new AccountReponseModel();
        }
       
        public void SaveAccountModelDetails(AccountInfoModelcs accountModel, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var account = _accountLogic.GetAccountById(accountModel.Id);
            if (account == null)
                throw new ZegaServiceException("Account Details Not Found or Not Saved.");
            if (accountModel.Model != null)
            {
                account.Model = _modelLogic.GetModelById(accountModel.Model.Id);
                if (account.Model == null)
                    throw new ZegaServiceException(string.Format("Invalid Model {0}.", accountModel.Model.Name));
                _accountLogic.Persist(account);
            }
        }

        public void SaveAccountAdditionalWithdrawals(AccountInfoModelcs accountModel, UserContextModel userContext)
        {
            var account = _accountLogic.GetAccountById(accountModel.Id);
            if (account == null)
                throw new ZegaServiceException("Account Details Not Found or Not Saved.");
            if (accountModel.accounWithdrawlInfoModelcs != null)
            {
                account.AccountDetail.C1_Withdrawal_Amount = accountModel.accounWithdrawlInfoModelcs.Withdrawl_Amount;
                account.AccountDetail.C2_Withdrawal_Date = accountModel.accounWithdrawlInfoModelcs.Withdrawl_Date;
                account.AccountDetail.C4_Withdrawal_Frequency = accountModel.accounWithdrawlInfoModelcs.Withdrawl_Frequency;
                account.AccountDetail.Future_Withdrawal = accountModel.accounWithdrawlInfoModelcs.Future_Withdrawal;
                account.AccountDetail.One_Time_Withdrawal_Amount = accountModel.accounWithdrawlInfoModelcs.One_Time_Withdrawal_Amount;
                account.AccountDetail.One_Time_Withdrawal_Date = accountModel.accounWithdrawlInfoModelcs.One_Time_Withdrawal_Date;
                account.AccountDetail.One_Time_Withdrawal = accountModel.accounWithdrawlInfoModelcs.One_Time_Withdrawal;
                _accountLogic.Persist(account);
            }

        }

        public void SaveAccountAdditionalDeposits(AccountInfoModelcs accountModel, UserContextModel userContext)
        {
            var account = _accountLogic.GetAccountById(accountModel.Id);
            if (account == null)
                throw new ZegaServiceException("Account Details Not Found or Not Saved.");

            if (accountModel.accounWithdrawlInfoModelcs != null)
            {
                account.AccountDetail.D1_Deposit = accountModel.accountDepositsInfoModelcs.Deposit_Amount;
                account.AccountDetail.D2_Deposit_Date = accountModel.accountDepositsInfoModelcs.Deposit_Date;
                account.AccountDetail.D3_Deposit_Status = accountModel.accountDepositsInfoModelcs.Deposit_Status;
                account.AccountDetail.D4_Deposit_Frequency = accountModel.accountDepositsInfoModelcs.Deposit_Frequency;
                _accountLogic.Persist(account);
            }
        }

        public void SaveAccountZegaCustomFields(AccountInfoModelcs accountModel, UserContextModel userContext)
        {
            var account = _accountLogic.GetAccountById(accountModel.Id);
            if(account == null)
                throw new ZegaServiceException("Account Details Not Found or Not Saved.");
            if ( accountModel.accountZegaCustomFieldsModel != null)
            {
                account.AccountDetail.Z2_ZEGA_Confirmed = accountModel.accountZegaCustomFieldsModel.Zega_Confirmed;
                account.AccountDetail.Z3_ZEGA_Alert_Date = accountModel.accountZegaCustomFieldsModel.Zega_Alert_Date;
                account.AccountDetail.Z4_ZEGA_Notes = accountModel.accountZegaCustomFieldsModel.Zega_Notes;
                _accountLogic.Persist(account);
            }
        }

        public AccountModel[] LoadAllAccounts()
        {
            var accounts = _accountLogic.LoadAll();
            if (accounts == null)
                return new AccountModel[0];

            return accounts.Select(o => new AccountModel
            {
                Id = o.Id,
                Name = o.ClientName,// Decription Can't be null in TradeList,Discription is Map with Client name in Zega Also, 
                ShortName = o.Number,
                Model = o.Model != null ? new CommonModel { Id = o.Model.Id, Name = o.Model.Name } : new CommonModel { Name = "Self" },
                CustomFields = new Dictionary<string, object> // need to check for common null check for the account
                {
                    { "AccountValue", o.AccountDetail?.AccountValue},
                    { "Acct_Type", o.AccountDetail?.AccountType},
                    //{ "B0_Tradable", o.AccountDetail.},
                    { "B2_Allocation_Start_Date", o.AccountDetail?.B2_Allocation_Start_Date},
                    //{ "B3_Special_Instructions", o.AccountDetail},
                    { "C1_Withdrawal_Amount", o.AccountDetail?.C1_Withdrawal_Amount},
                    { "C2_Withdrawal_Date", o.AccountDetail?.C2_Withdrawal_Date},
                    { "CashEq", o.AccountDetail?.CashEq},
                    { "CashNetBal", o.AccountDetail?.CashNetBal},
                    { "D1_Deposit", o.AccountDetail?.D1_Deposit},
                    { "D2_Deposit_Date", o.AccountDetail?.D2_Deposit_Date},
                    { "D3_Deposit_Status", o.AccountDetail?.D3_Deposit_Status},
                    { "OBuyingPower", o.AccountDetail?.OBuyingPower},
                    { "SBuyingPower", o.AccountDetail?.SBuyingPower},
                    { "VEOImportDate", o.AccountDetail?.VeoImportDate},
                    { "Z2_ZEGA_Confirmed", o.AccountDetail?.Z2_ZEGA_Confirmed},
                    { "Z3_ZEGA_Alert Date", o.AccountDetail?.Z3_ZEGA_Alert_Date},
                    { "Z4_ZEGA_Notes", o.AccountDetail?.Z4_ZEGA_Notes}
                },
                Manager = o.RepCode != null ? new CommonModel { Id = o.RepCode.Id, Name = o.RepCode.Code } : null,
                TotalMarketValue = o.AccountDetail != null && o.AccountDetail.AccountValue != null ? (decimal) o.AccountDetail.AccountValue : 0,
                TaxStatus = o.AccountDetail != null ? o.AccountDetail.AccountType : AccountType.NotSet,
                DefaultBrokerAccount = new AccountBrokerAccountModel { AccountName = o.Number, Broker = new CommonModel { Name = o.Broker.ToString()} } 
            }).ToArray();
        }

        public Dictionary<string, Dictionary<string, decimal>> GetByFilterWithSleevePercent(string[] sleeveIds)
        {
            var accounts = _accountLogic.GetBySleeves(sleeveIds);
            var accLists = new Dictionary<string, Dictionary<string, decimal>>();
           
            foreach (var acc in accounts)
            {
                var sleeves = acc.Model.ModelSleeves.ToDictionary(x => x.Sleeve.Name, x => x.Allocation); 
                accLists.Add(acc.Number, sleeves);
            }

            return accLists;
        }
        public List<BrokerModel> GetBrokerByAccountId(int id)
        {
            var account = _accountLogic.GetAccountById(id);
            var brokers = new List<BrokerModel>();
            if(account != null)
            {
                brokers.Add(new BrokerModel() { Id = account.Broker.ToString(), Name = account.Broker.ToString() });
            }
            return brokers;
        }
    }
}
