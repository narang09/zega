using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZegaFinancials.Business.Support.TDVeo
{
    public interface ITdVeoImportWebService
    {
        Task AuthorizeAsync(string login, string password);
        Task AuthorizeAsync(string login, string password, string apikey);
        Task LogoutAsync();
        Task LogoutAsync(string sessionToken);

        Task<string> GetBalancesAsync(IEnumerable<string> repCodes);
        Task<string> GetBalancesAsync(string sessionToken, IEnumerable<string> repCodes = null, IEnumerable<string> accountGroupIds = null);

        void Authorize(string login, string password);
        void Authorize(string login, string password, string apikey);

        string AuthorizeAndGetSessionToken(string login, string password);
        void Logout();

        void Logout(string sessionToken);

        string GetBalances(IEnumerable<string> repCodes);
        string GetBalances(string sessionToken, IEnumerable<string> repCodes);

    }
}
