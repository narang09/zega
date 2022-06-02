using System.Collections.Generic;

namespace ZegaFinancials.Business.Support.TDVeo
{
    public interface ITDVeoImportService
    {
        string Authenticate(string userId, string password);
        string GetBalances(string sessionToken,  IEnumerable<string> repcodes);
        void LogOut(string sessionToken);
    }
}
