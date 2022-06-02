using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Business.Interfaces.Accounts;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Nhibernate.Dao.Interface.Accounts;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Business.Impl.Accounts
{
    public class AccountLogic : ZegaLogic, IAccountLogic
    {
        private readonly IAccountDao _accountDao;
        private readonly IUserLogic _userLogic;

        public AccountLogic(IAccountDao accountDao, IUserLogic userLogic)
        {
            _accountDao = accountDao;
            _userLogic = userLogic;
        }

        public IEnumerable<Account> GetAccountsByFilter(DataGridFilterModel model, IEnumerable<int> repcodeIds, bool isAdmin, out int count)
        {
            if (!isAdmin && (repcodeIds == null || !repcodeIds.Any()))
            {
                count = 0;
                return new List<Account>();
            }


            return _accountDao.GetAccountsByFilter(model, repcodeIds, isAdmin, out count);
        }

        public Account CreateAccountEntity()
        {
            return _accountDao.Create();
        }
        public Account GetAccountById(int accountId)
        {
            return _accountDao.Get(accountId);
        }
        public void Persist(Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");
            if (_accountDao.IsExists(account.Id, account.Number))
                throw new ZegaLogicException("Account with the same Account Number already exists.");

            _accountDao.Persist(account);

        }

        public void Persist(IEnumerable<Account> accounts)
        {
            if (accounts == null)
                throw new ArgumentNullException("accounts");

            _accountDao.Persist(accounts);
        }

        public IEnumerable<Account> GetAccountsByIds(int[] accountIds)
        {
            var accounts = _accountDao.GetAccountsByIds(accountIds);
            return accounts;
        }

        public long GetAccountsCountByRepCode(int[] repCodeIds)
        {
            return _accountDao.GetAccountsCountByRepCode(repCodeIds);
        }

        public long GetClientsCountByRepCode(int[] repCodeIds)
        {
            return _accountDao.GetClientsCountByRepCode(repCodeIds);
        }
        public void DeleteAccountByid(int accountId)
        {
            _accountDao.Delete(accountId);
        }

        public IEnumerable<RepCode> GetRepCodesListByAdvisorId(int advisorId)
        {
            return _userLogic.GetRepCodesByAdvisorId(advisorId);
        }
        public bool IsExist(int accountId)
        {
            return _accountDao.IsExist(accountId);
        }
        public IEnumerable<User> GetAdvisorsByAccountIds(int[] accountIds)
        {
            return _accountDao.GetAdvisorsByAccountIds(accountIds);
        }
        public IEnumerable<Account> LoadAll()
        {
            return _accountDao.LoadAll();
        }
        public IEnumerable<Account> GetBySleeves(string[] sleeveIds)
        {
            return _accountDao.GetBySleeves(sleeveIds);
        }
    }
}
