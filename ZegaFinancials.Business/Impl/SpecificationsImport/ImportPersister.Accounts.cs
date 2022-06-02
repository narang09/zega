using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZegaFinancials.Business.Support.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport.ImportData;
using ZegaFinancials.Nhibernate.Dao.Interface.Accounts;
using ZegaFinancials.Nhibernate.Dao.Interface.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Business.Impl.SpecificationsImport
{
    public partial class ImportPersister
    {
        private readonly IAccountDao _accountDao;
        private readonly IRepCodeDao _repCodeDao;
        private readonly ILogger<ImportPersister> _logger;
        private readonly IAdvisorRepCodeDao _advisorRepCodeDao;
        private readonly IModelDao _modelDao;
        public ImportPersister(IAccountDao accountDao, IRepCodeDao repCodeDao, IAdvisorRepCodeDao advisorRepCodeDao, ILogger<ImportPersister> logger ,IModelDao modelDao)
        {
            _accountDao = accountDao;
            _repCodeDao = repCodeDao;
            _advisorRepCodeDao = advisorRepCodeDao;
            _logger = logger;
            _modelDao = modelDao;
        }
        /// <summary>
        /// Convert the accounts import data into models.
        /// </summary>
        /// <param name="importData">The import data.</param>
        /// <param name="importCache">The import cache.</param>
        /// <param name="fileImportResult">The file import result.</param>
        /// <returns></returns>
        private bool ExtractAccountsImportData(ImportDataItem<AccountImportData> importData, ImportCache importCache)
        {
            if (importData == null || importData.Data == null)
                return false;
            if (importData.Data.Count == 0)
            {
                importPersistResult = new ImportPersistResult
                {
                    ErrorMsg = importData.Messages,
                    TotalCount = importData.Data.Count,
                    SuccessfullyCount = 0
                };

                return false;
            }
            _logger.LogInformation("Validate account data.");

            var successfullyCount = 0;

            _logger.LogInformation("Load account data from database.");
            importCache.LoadAccounts(_accountDao, importData.Data.Select(p => p.AccountNumber));
            _logger.LogInformation(string.Format("Done loading process of '{0}' account data", importCache.GetAccounts().Count));
            
            _logger.LogInformation(string.Format("Start accounts entities preparation for {0} accounts", importData.Data.Count));          
            foreach (var a in importData.Data)
            {
                var account = importCache.GetAccount(a.AccountNumber);

                var isNew = account == null;
                if (isNew)
                {
                    account = _accountDao.Create();
                    account.Number = a.AccountNumber == null ? account.Number : a.AccountNumber;
                    account.AccountDetail = new()
                    {
                        AccountValue = a.AccountValue,
                        CashNetBal = a.CashNetBal,
                        CashEq = a.CashEq,
                        SBuyingPower = a.SBuyingPower,
                        Account = account,
                        OBuyingPower = a.OBuyingPower, 
                        VeoImportDate = DateTime.Now
                    };
                }
                else if (account.AccountDetail != null)
                {
                    account.AccountDetail.AccountValue = a.AccountValue;
                    account.AccountDetail.CashNetBal = a.CashNetBal;
                    account.AccountDetail.CashEq = a.CashEq;
                    account.AccountDetail.OBuyingPower = a.OBuyingPower;
                    account.AccountDetail.SBuyingPower = a.SBuyingPower;
                    account.AccountDetail.VeoImportDate = DateTime.Now;
                }
                account.ClientName = a.ClientName;
                account.Broker = Broker.TDA;

                RepCode repCode;

                if (account.RepCode == null || !string.Equals(account.RepCode.Code, a.RepCode, StringComparison.CurrentCultureIgnoreCase))               
                    repCode = importCache.GetRepCode(_repCodeDao, a.RepCode);
                else
                    repCode = account.RepCode;

                if (repCode != null)
                    account.RepCode = repCode;

                if (isNew)
                    importCache.AddAccount(account);
                successfullyCount++;
            }           
            _logger.LogInformation("Done Validate process account data.");
            importPersistResult = new ImportPersistResult
            {
                ErrorMsg = importData.Messages,
                TotalCount = importData.Data.Count,
                SuccessfullyCount = successfullyCount
            };

            return successfullyCount > 0;
        }

        /// <summary>
        /// Convert the accounts Upload data into models.
        /// </summary>
        /// <param name="importData">The import data.</param>
        /// <param name="importCache">The import cache.</param>
        /// <param name="totalRows">The total no. of rows.</param>
        /// <returns></returns>
        private bool ExtractAccountsUploadData(ImportDataItem<AccountFileRowData> importData, ImportCache importCache ,int totalRows = 0)
        {
            if (importData == null || importData.Data == null)
                return false;
            if (importData.Data.Count == 0)
            {
                importPersistResult = new ImportPersistResult
                {
                    TotalCount = totalRows,
                    SuccessfullyCount = 0
                };

                return false;
            }
            _logger.LogInformation("Validate account data.");

            var successfullyCount = 0;

            _logger.LogInformation("Load account data from database.");
            importCache.LoadAccounts(_accountDao, importData.Data.Select(p => p.AccountNumber));
            _logger.LogInformation(string.Format("Done loading process of '{0}' account data", importCache.GetAccounts().Count));
             
            _logger.LogInformation(string.Format("Start accounts entities preparation for {0} accounts", importData.Data.Count));
            List<string> processedAccounts = new List<string>();
            foreach (var a in importData.Data)
            {
                if (processedAccounts.Contains(a.AccountNumber))
                {
                    importData.Messages.Add(string.Format("Account Number({0}) already Processed", a.AccountNumber));
                    continue;
                }
                var account = importCache.GetAccount(a.AccountNumber);
                var isAccountNotFound = account == null;
                if (isAccountNotFound)
                {
                    importData.Messages.Add(string.Format("Invalid Account Number({0})", a.AccountNumber));
                    continue;
                }
                else
                {
                    if(!string.IsNullOrEmpty(a.ModelName))
                    {
                        var model = _modelDao.GetModelByName(a.ModelName);
                        if (model == null)
                        {
                            importData.Messages.Add(string.Format("Model Validation Failled .Account Number({0}) , Invalid Model Name ({1})", a.AccountNumber, a.ModelName));
                            continue;
                        }
                        else
                            account.Model = model;
                    }
                    if (account.AccountDetail == null)
                        account.AccountDetail = new AccountDetails();

                    if (((account.AccountDetail.Future_Withdrawal == WithdrawlorDepositStatus.No && a.Future_Withdrwal == null) || (a.Future_Withdrwal != null && a.Future_Withdrwal  == WithdrawlorDepositStatus.No)) && (a.Withdrawal_Amount != null || a.Withdrawal_Date != null || a.Withdrwal_Frequency != null))
                    {
                            importData.Messages.Add(string.Format("Addition Withdrawal Details is Invalid For Account No.{0} ,Addition Withdrawal In Future Set as :No",a.AccountNumber));
                            continue;
                    }
                    else if (((account.AccountDetail.Future_Withdrawal == WithdrawlorDepositStatus.Yes && a.Future_Withdrwal == null) || (a.Future_Withdrwal != null && a.Future_Withdrwal == WithdrawlorDepositStatus.Yes)) && (a.Withdrawal_Amount == null || a.Withdrawal_Date == null || a.Withdrwal_Frequency == null))
                    {
                        importData.Messages.Add(string.Format("Addition Withdrawal Details is Invalid For Account No.{0} ,Addition Withdrawal In Future Set as :Yes", a.AccountNumber));
                        continue;
                    }
                    if(((account.AccountDetail.One_Time_Withdrawal == WithdrawlorDepositStatus.No && a.One_Time_Withdrawal == null) || (a.One_Time_Withdrawal != null && a.One_Time_Withdrawal == WithdrawlorDepositStatus.No )) && (a.One_Time_Withdrawal_Amount != null || a.One_Time_Withdrawal_Date != null))
                    {
                        importData.Messages.Add(string.Format("One Time  Withdrawal Details is Invalid For Account No.{0} , one  time Withdrawal In Future Set as : No", a.AccountNumber));
                        continue;
                    }
                    else if (((account.AccountDetail.One_Time_Withdrawal == WithdrawlorDepositStatus.Yes && a.One_Time_Withdrawal == null) || (a.One_Time_Withdrawal != null && a.One_Time_Withdrawal == WithdrawlorDepositStatus.Yes)) && (a.One_Time_Withdrawal_Amount == null || a.One_Time_Withdrawal_Date == null))
                    {
                        importData.Messages.Add(string.Format("One Time  Withdrawal Details is Invalid For Account No.{0} , one  time Withdrawal In Future Set as : Yes", a.AccountNumber));
                        continue;
                    }
                    if (((account.AccountDetail.D3_Deposit_Status == WithdrawlorDepositStatus.No && a.Deposit_Status == null) || (a.Deposit_Status != null && a.Deposit_Status == WithdrawlorDepositStatus.No)) && (a.Deposit_Amount != null || a.Deposit_Date != null || a.Deposit_Frequency != null ))
                    {
                        importData.Messages.Add(string.Format(" Additional Deposit Details is Invalid For Account No.{0} ,Deposit Status Set as : No", a.AccountNumber));
                        continue;
                    }
                    else if (((account.AccountDetail.D3_Deposit_Status == WithdrawlorDepositStatus.Yes && a.Deposit_Status == null) || (a.Deposit_Status != null && a.Deposit_Status == WithdrawlorDepositStatus.Yes)) && (a.Deposit_Amount == null || a.Deposit_Date == null || a.Deposit_Frequency == null))
                    {
                        importData.Messages.Add(string.Format(" Additional Deposit Details is Invalid For Account No.{0} ,Deposit Status Set as : Yes", a.AccountNumber));
                        continue;
                    }
                       
                        account.AccountDetail.AccountType = ((AccountType)(a.AccountType != null ? a.AccountType : account.AccountDetail.AccountType));
                        account.AccountStatus = (AccountStatus)(a.AccountStatus != null ? a.AccountStatus : account.AccountStatus);
                        account.AccountDetail.B2_Allocation_Start_Date = a.AllocationDate != null ? a.AllocationDate : account.AccountDetail.B2_Allocation_Start_Date;
                        account.Name = a.AccountName != null ? a.AccountName : account.Name;
                        account.AccountDetail.Future_Withdrawal = (WithdrawlorDepositStatus)(a.Future_Withdrwal != null ? a.Future_Withdrwal : account.AccountDetail.Future_Withdrawal);
                    if(account.AccountDetail.Future_Withdrawal == WithdrawlorDepositStatus.No)
                    {
                        account.AccountDetail.C1_Withdrawal_Amount =  null ;
                        account.AccountDetail.C4_Withdrawal_Frequency = Frequency.NotSet;
                        account.AccountDetail.C2_Withdrawal_Date = null;
                    }
                    else
                    {
                        account.AccountDetail.C1_Withdrawal_Amount = a.Withdrawal_Amount != null ? a.Withdrawal_Amount : account.AccountDetail.C1_Withdrawal_Amount;
                        account.AccountDetail.C4_Withdrawal_Frequency = (Frequency)(a.Withdrwal_Frequency != null ? a.Withdrwal_Frequency : account.AccountDetail.C4_Withdrawal_Frequency);
                        account.AccountDetail.C2_Withdrawal_Date = a.Withdrawal_Date != null ? a.Withdrawal_Date : account.AccountDetail.C2_Withdrawal_Date;
                    }
                    account.AccountDetail.One_Time_Withdrawal = (WithdrawlorDepositStatus)(a.One_Time_Withdrawal != null ? a.One_Time_Withdrawal : account.AccountDetail.One_Time_Withdrawal);
                    if (account.AccountDetail.One_Time_Withdrawal == WithdrawlorDepositStatus.No)
                    {
                        account.AccountDetail.One_Time_Withdrawal_Amount = null;
                        account.AccountDetail.One_Time_Withdrawal_Date = null; 
                    }
                    else
                    {
                        account.AccountDetail.One_Time_Withdrawal_Amount = a.One_Time_Withdrawal_Amount != null ? a.One_Time_Withdrawal_Amount : account.AccountDetail.One_Time_Withdrawal_Amount;
                        account.AccountDetail.One_Time_Withdrawal_Date = a.One_Time_Withdrawal_Date != null ? a.One_Time_Withdrawal_Date : account.AccountDetail.One_Time_Withdrawal_Date;
                    }
                    account.AccountDetail.D3_Deposit_Status = (WithdrawlorDepositStatus)(a.Deposit_Status != null ? a.Deposit_Status : account.AccountDetail.D3_Deposit_Status);
                    if (account.AccountDetail.D3_Deposit_Status == WithdrawlorDepositStatus.No)
                    {
                        account.AccountDetail.D1_Deposit = null;
                        account.AccountDetail.D2_Deposit_Date = null;
                        account.AccountDetail.D4_Deposit_Frequency = Frequency.NotSet;

                    }
                    else
                    {
                        account.AccountDetail.D1_Deposit = a.Deposit_Amount != null ? a.Deposit_Amount : account.AccountDetail.D1_Deposit;
                        account.AccountDetail.D2_Deposit_Date = a.Deposit_Date != null ? a.Deposit_Date : account.AccountDetail.D2_Deposit_Date;
                        account.AccountDetail.D4_Deposit_Frequency = (Frequency)(a.Deposit_Frequency != null ? a.Deposit_Frequency : account.AccountDetail.D4_Deposit_Frequency);

                    }
                        account.AccountDetail.Z2_ZEGA_Confirmed = (WithdrawlorDepositStatus)(a.ZEGA_Confirmed != null ? a.ZEGA_Confirmed : account.AccountDetail.Z2_ZEGA_Confirmed);
                        account.AccountDetail.Z3_ZEGA_Alert_Date = a.ZEGA_Alert_Date != null ? a.ZEGA_Alert_Date : account.AccountDetail.Z3_ZEGA_Alert_Date;
                        account.AccountDetail.Z4_ZEGA_Notes = a.ZEGA_Notes != null ? a.ZEGA_Notes : account.AccountDetail.Z4_ZEGA_Notes;
                   successfullyCount++;
                }
            }
            _logger.LogInformation("Done Validate process account data.");
            importPersistResult = new ImportPersistResult
            {
                ErrorMsg = importData.Messages,
                TotalCount =totalRows,
                SuccessfullyCount = successfullyCount
            };
            return successfullyCount > 0;
        }

    }
}
