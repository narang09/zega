using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ZegaFinancials.Business.Support.TDVeo
{
    public class TDVeoImportService : ITDVeoImportService
    {
        private readonly ITdVeoImportWebService _TDVeoImportWebService;
        public TDVeoImportService(IConfiguration configuration, ITdVeoImportWebService TDVeoWebService)
        {
            if (configuration["TDVeoURI"] != null && configuration["TDVeoAPIkey"] != null)
            {
                _TDVeoImportWebService = new TdVeoImportWebService(configuration["TDVeoURI"], configuration["TDVeoAPIkey"]);
            }            
        }

        public string Authenticate(string userId, string password)
        {
            return _TDVeoImportWebService.AuthorizeAndGetSessionToken(userId, password);
        }

        public string GetBalances(string sessionToken, IEnumerable<string> repcodes)
        {
            return _TDVeoImportWebService.GetBalances(sessionToken, repcodes);
        }

        public void LogOut(string sessionToken)
        {
            _TDVeoImportWebService.Logout(sessionToken);
        }
    }
}
