using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Business.Interfaces.Accounts
{
    public interface IAccountLogic
    {
         IEnumerable<Account> GetAccountsByFilter(DataGridFilterModel model,IEnumerable<int> repcodeIds, bool isAdmin, out int count);
        Account CreateAccountEntity();
        Account GetAccountById(int accountId);
        void Persist(Account account);
        void Persist(IEnumerable<Account> accounts);
        IEnumerable<Account> GetAccountsByIds(int[] accountIds);
        long GetAccountsCountByRepCode(int[] repCodeIds);
        long GetClientsCountByRepCode(int[] repCodeIds);
        void DeleteAccountByid(int accountId);
        IEnumerable<RepCode> GetRepCodesListByAdvisorId(int advisorId);
        bool IsExist(int accountId);
        IEnumerable<User> GetAdvisorsByAccountIds(int[] accountIds);
        IEnumerable<Account> LoadAll();
        IEnumerable<Account> GetBySleeves(string[] sleeveIds);
    }
}
