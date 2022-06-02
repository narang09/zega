using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Dao.Interface.Accounts;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Business.Impl.SpecificationsImport
{
    public partial class ImportPersister
    {      

        public class ImportCache
        {
            public ImportCache()
            {
                _accounts = new Dictionary<string, Account>();
            }

            #region Accounts

            private readonly Dictionary<string, Account> _accounts;

            public void AddAccount(Account account)
            {
                _accounts.Add(account.Number, account);
            }
            public Account GetAccount(string accountNumber)
            {
                return _accounts.ContainsKey(accountNumber) ? _accounts[accountNumber] : null;
            }

            public ICollection<Account> GetAccounts()
            {
                return _accounts.Values;
            }

            public void LoadAccounts(IAccountDao accountDao, IEnumerable<string> accountNumbers)
            {
                LoadAccountsInDictionary(accountDao, accountNumbers, _accounts);
            }

            private void LoadAccountsInDictionary(IAccountDao accountDao, IEnumerable<string> accountNumbers, Dictionary<string, Account> accounts)
            {
                var newNames = accountNumbers.Except(accounts.Keys);
                if (newNames.Any())
                {
                    var existingAccountsPart = accountDao.GetBy(newNames.Distinct().ToArray());
                    foreach (var p in existingAccountsPart)
                        if (!accounts.Keys.Contains(p.Number))
                            accounts.Add(p.Number, p);
                }
            }

            #endregion            

            #region RepCodes

            private Dictionary<string, RepCode> _repCodesCache;
            public RepCode GetRepCode(IRepCodeDao repCodeDao, string repCodeName)
            {
                if (_repCodesCache == null)
                    _repCodesCache = repCodeDao.GetAll().ToDictionary(o => o.Code.ToUpper());

                var key = repCodeName.ToUpper();

                RepCode repCode;
                return _repCodesCache.TryGetValue(key, out repCode) ? repCode : null;
            }

            #endregion

            #region Advisors
            private IList<User> _advisorCache;
            public IList<User> GetAdvisor(IAdvisorRepCodeDao advisorRepCodeDao, string repCodeName)
            {
                if (_advisorCache == null)
                    _advisorCache = advisorRepCodeDao.GetAll()?.Where(o => o.RepCode.Code.Equals(repCodeName)).Select(o => o.Advisor).ToList();

                return _advisorCache;
            }
            #endregion
        }
    }
}


