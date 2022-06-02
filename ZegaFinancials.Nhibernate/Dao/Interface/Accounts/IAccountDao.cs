using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Dao.Interface.Accounts
{
    public interface IAccountDao : INHibernateDao<Account>
    {       
        IEnumerable<Account> GetAccountsByFilter(DataGridFilterModel model, IEnumerable<int> repCodeIds, bool isAdmin, out int count);
         bool IsExists(int accountId, string accountNo);
       IEnumerable<Account> GetAccountsByIds(int[] accountIds);
        long GetAccountsCountByRepCode(int[] repCodeIds);
        long GetClientsCountByRepCode(int[] repCodeIds);
        IEnumerable<Account> GetBy(string[] accountNumbers);
        IEnumerable<User> GetAdvisorsByAccountIds(int[] AccountIds);
        bool CheckDependentAccountsbyAdvisor(List<int> repcodeIds);
        void CheckAccountDependencyByRepCode(RepCode repCode ,User user);
        IEnumerable<Account> LoadAll();
        IEnumerable<Account> GetBySleeves(string[] sleeveIds);
    }
}
