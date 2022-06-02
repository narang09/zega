using System.Collections.Generic;
using System.Xml.Linq;
using ZegaFinancials.Business.Support.SpecificationsImport.Model;

namespace ZegaFinancials.Business.Support.TDVeo
{
    public interface ITDVeoImportProvider
    {
        IEnumerable<XElement> GetAccounts(IEnumerable<string> repcodes, ImportResult importResult, string profileName,string token, int batchNo = 0, int batchSize = 0, bool isAutoImport = false);
        string AuthenticateProfile(string userId, string Password, ImportResult importResult,string profileName);
    }
}
