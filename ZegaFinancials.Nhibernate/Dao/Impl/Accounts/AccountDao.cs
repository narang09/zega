using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Accounts;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Nhibernate.Support.QueryGenerator;

namespace ZegaFinancials.Nhibernate.Dao.Impl.Accounts
{
    public class AccountDao : NHibernateDao<Account>, IAccountDao
    {
        public AccountDao(ISession session) : base(session)
        { }

        public IEnumerable<Account> GetAccountsByFilter(DataGridFilterModel model, IEnumerable<int> repCodeIds, bool isAdmin, out int count)
        {
            var sql = @"
FROM Account AS a WHERE ";
            var qg = new QueryGenerator("a", model, new[]
            {
                new QueryReplace("Model.Name", @"(Select m.Name FROM a.Model AS m)"),
                new QueryReplace("accountBasicDetails.ClientName", "a.ClientName"),
                new QueryReplace("accountBasicDetails.Number", "a.Number"),
                new QueryReplace("accountBasicDetails.CashNetBal", "a.AccountDetail.CashNetBal"),
                new QueryReplace("accountBasicDetails.VeoImportDate", "a.AccountDetail.VeoImportDate"),
                new QueryReplace("accountBasicDetails.AllocationDate", "a.AccountDetail.B2_Allocation_Start_Date"),
                new QueryReplace("accountBasicDetails.RepCode.Name", "a.RepCode.Code"),
                new QueryReplace("accountBasicDetails.AdvisorsName",  @"(SELECT STRING_AGG(CONCAT(FirstName,LastName),',') From UserDetails Where User IN (SELECT Advisor From AdvisorRepcode  Where RepCode = a.RepCode.Id))"),
                new QueryReplace("accounWithdrawlInfoModelcs.Withdrawl_Amount", "a.AccountDetail.C1_Withdrawal_Amount"),
                new QueryReplace("accountBasicDetails.CashEq", "a.AccountDetail.CashEq"),
                new QueryReplace("accountBasicDetails.AccountValue", "a.AccountDetail.AccountValue"),
                new QueryReplace("accountBasicDetails.CashEq", "a.AccountDetail.CashEq"),
                new QueryReplace("accountBasicDetails.SBuyingPower", "a.AccountDetail.SBuyingPower"),
                new QueryReplace("accountBasicDetails.OBuyingPower", "a.AccountDetail.OBuyingPower"),
                new QueryReplace("accountZegaCustomFieldsModel.Zega_Notes", "a.AccountDetail.Z4_ZEGA_Notes"),
                new QueryReplace("accountZegaCustomFieldsModel.Zega_Alert_Date", "a.AccountDetail.Z3_ZEGA_Alert_Date"),
                new QueryReplace("accountZegaCustomFieldsModel.Zega_ConfirmedValue", "a.AccountDetail.Z2_ZEGA_Confirmed"),
                new QueryReplace("accountDepositsInfoModelcs.Deposit_Date", "a.AccountDetail.D2_Deposit_Date"),
                new QueryReplace("accountDepositsInfoModelcs.Deposit_Amount", "a.AccountDetail.D1_Deposit"),
                new QueryReplace("accountDepositsInfoModelcs.Deposit_StatusValue", "a.AccountDetail.D3_Deposit_Status"),
                new QueryReplace("accountDepositsInfoModelcs.Deposit_FrequencyValue", "a.AccountDetail.D4_Deposit_Frequency"),
                new QueryReplace("accounWithdrawlInfoModelcs.Withdrawl_StatusValue", "a.AccountDetail.C3_Withdrawal_Status"),
                new QueryReplace("accounWithdrawlInfoModelcs.Withdrawl_Date", "a.AccountDetail.C2_Withdrawal_Date"),
                new QueryReplace("accounWithdrawlInfoModelcs.Withdrawl_Amount", "a.AccountDetail.C1_Withdrawal_Amount"),
                new QueryReplace("accounWithdrawlInfoModelcs.Withdrawal_FrequencyValue", "a.AccountDetail.C4_Withdrawal_Frequency"),
                new QueryReplace("accounWithdrawlInfoModelcs.One_Time_Withdrawal_Date", "a.AccountDetail.One_Time_Withdrawal_Date"),
                new QueryReplace("accounWithdrawlInfoModelcs.One_Time_Withdrawal_Amount", "a.AccountDetail.One_Time_Withdrawal_Amount"),
                new QueryReplace("accountBasicDetails.AccountTypeValue", "a.AccountDetail.AccountType"),
                new QueryReplace("accountBasicDetails.AccountStatusValue", "a.AccountStatus"),
                new QueryReplace("accountBasicDetails.BrokerValue", "a.Broker"),
                new QueryReplace("accounWithdrawlInfoModelcs.Future_Withdrawal_StatusValue","a.AccountDetail.Future_Withdrawal"),
                new QueryReplace("accountDepositsInfoModelcs.Deposit_StatusValue","a.AccountDetail.D3_Deposit_Status"),
                new QueryReplace("accountDepositsInfoModelcs.Deposit_FrequencyValue", "a.AccountDetail.D4_Deposit_Frequency"),
                new QueryReplace("accounWithdrawlInfoModelcs.One_Time_WithdrawalValue","a.AccountDetail.One_Time_Withdrawal")

            }, new Dictionary<string, string>
            {
                 { "accountBasicDetails.AccountStatusValue", "AccountStatus" } ,
                 { "accountBasicDetails.BrokerValue", "Broker"},
                 { "accountBasicDetails.AccountTypeValue", "AccountType" } ,
                 { "accounWithdrawlInfoModelcs.Withdrawl_StatusValue", "WithdrawlorDepositStatus" } ,
                 { "accountZegaCustomFieldsModel.Zega_ConfirmedValue", "WithdrawlorDepositStatus" } ,
                 { "accountDepositsInfoModelcs.Deposit_StatusValue","WithdrawlorDepositStatus"},
                 { "accounWithdrawlInfoModelcs.Future_Withdrawal_StatusValue","WithdrawlorDepositStatus"},
                 { "accounWithdrawlInfoModelcs.One_Time_WithdrawalValue","WithdrawlorDepositStatus"},
                 { "accounWithdrawlInfoModelcs.Withdrawal_FrequencyValue", "Frequency"},
                 { "accountDepositsInfoModelcs.Deposit_FrequencyValue", "Frequency" }
            });

            var param = new Dictionary<string, object>();
            if (isAdmin)
            {
                sql += "1=:admin ";
                param.Add("admin", isAdmin);
            }
            else
            {
                sql += "a.RepCode in (:repCodeIds)";
                param.Add("repCodeIds", repCodeIds);
            }

            var qb = qg.ApplyBasicMultiFilters<Account>(_session, sql, param);
            var accounts = qb.GetResult<Account>(0);
            count = (int)qb.GetResult<long>(1).Single();

            return accounts;
        }


        public virtual IEnumerable<Account> GetBy(string[] accountNumbers)
        {
            var accounts = new List<Account>();
            var tripCount = Math.Ceiling((decimal)accountNumbers.Length / MaxParametersCountPerOneQuery);
            for (var i = 0; i < tripCount; i++)
            {
                accounts.AddRange(_session.CreateQuery(@"
                FROM Account AS a
                WHERE a.Number in (:accNumbers)")
                    .SetParameterList("accNumbers", accountNumbers.Skip(i * MaxParametersCountPerOneQuery).Take(MaxParametersCountPerOneQuery))
                    .List<Account>());
            }
            return accounts;
        }
        public virtual bool IsExists(int accountId, string accountNo)
        {
            var query = _session.CreateQuery(@"
	SELECT COUNT(*) FROM
	Account
	WHERE Id <> :aId AND Number = :accountNo")
                .SetParameter("aId", accountId)
                .SetParameter("accountNo", accountNo)
                .UniqueResult<long>();

            return query > 0;
        }
        public IEnumerable<Account> GetAccountsByIds(int[] accountIds)
        {
            if (accountIds == null || !accountIds.Any())
            {
                return new List<Account>();
            }
            return _session.CreateQuery(@"
	FROM Account as a
	WHERE a.Id IN (:accountIds)
	ORDER BY a.Id")
            .SetParameterList("accountIds", accountIds)
            .List<Account>();
        }
        public virtual long GetAccountsCountByRepCode(int[] repCodeIds)
        {
            if (repCodeIds == null || !repCodeIds.Any())
                return 0;
            return _session.CreateQuery(@"
  SELECT COUNT(*)

              FROM Account AS a
      WHERE a.RepCode.Id IN (:repCodeIds)")
                .SetParameterList("repCodeIds", repCodeIds)
               .UniqueResult<long>();
        }

        public virtual long GetClientsCountByRepCode(int[] repCodeIds)
        {
            if (repCodeIds == null || !repCodeIds.Any())
                return 0;
            return _session.CreateQuery(@"
SELECT DISTINCT ClientName
	  
            FROM Account AS a
	WHERE a.RepCode.Id IN (:repCodeIds)")
                .SetParameterList("repCodeIds", repCodeIds).List().Count;
        }
        public IEnumerable<User> GetAdvisorsByAccountIds(int[] accountIds)
        {
            if (accountIds == null || accountIds.Length == 0)
                return new List<User>();
            var sql = @"Select a.Advisor from Account a WHERE a.Id in (:accountIds)";
            var advisors = _session.CreateQuery(sql).SetParameterList("accountIds", accountIds).List<User>();
            return advisors;
        }

        public bool CheckDependentAccountsbyAdvisor(List<int> repCodeIds)
        {
            var sql = @"Select 1 From Account Where RepCode IN(:repCodeids) ";
            var accounts = _session.CreateQuery(sql).SetParameterList("repCodeids", repCodeIds).List<int>();
            return accounts != null && accounts.Any();
        }

        public void CheckAccountDependencyByRepCode(RepCode repCode, User user = null)
        {
            if (user != null)
            {
                var sqladvisor = @"Select Advisor From AdvisorRepcode Where RepCode =:repCodeId  AND Advisor !=:userId";
                var advisor = _session.CreateQuery(sqladvisor).SetParameter("repCodeId", repCode.Id).SetParameter("userId", user.Id).List<User>();
                if (advisor != null && advisor.Any())
                    return;
                else
                {
                    var sqlaccount = @"From Account Where RepCode =: repCodeId";
                    var accounts = _session.CreateQuery(sqlaccount).SetParameter("repCodeId", repCode.Id).List<Account>();
                    var advisorAccount = accounts?.Select(o => o.Number).ToList();
                    if (advisorAccount != null && advisorAccount.Any())
                        throw new ZegaDaoException(string.Format("Advisor can't be update, RepCode: {0} Exist in Accounts:{1}", repCode.Code, string.Join(",", advisorAccount)));
                }
            }
            else
            {
                var sql = @"Select Number From Account Where RepCode =:repCodeId";
                var accounts = _session.CreateQuery(sql).SetParameter("repCodeId", repCode.Id).List<string>();
                if (accounts != null && accounts.Any())
                    throw new ZegaDaoException(string.Format("Repcode :{0} can't be delete ,it is used in accounts:{1}", repCode.Code, string.Join(",", accounts)));


            }
        }

            public IEnumerable<Account> LoadAll()
            {
                return _session.CreateQuery("FROM Account acc ORDER BY acc.Name").List<Account>();
            }

            public IEnumerable<Account> GetBySleeves(string[] sleeveIds)
            {
                var sql = @"
                 FROM Account a 
                 WHERE EXISTS (FROM ModelSleeve ms WHERE ms.Model.Id = a.Model.Id AND ms.Sleeve.Name IN (:sleeveIds))"; 
                return _session.CreateQuery(sql).SetParameterList("sleeveIds", sleeveIds).List<Account>();
            }
        }
    }

