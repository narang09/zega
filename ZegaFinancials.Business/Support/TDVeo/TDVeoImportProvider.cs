using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ZegaFinancials.Business.Impl.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport.Model;

namespace ZegaFinancials.Business.Support.TDVeo
{
    public sealed class TDVeoImportProvider : ITDVeoImportProvider
    {
        private readonly ITDVeoImportService _importService;
        private readonly ILogger<TDVeoImportProvider> _logger;
        public TDVeoImportProvider(ITDVeoImportService importService, ILogger<TDVeoImportProvider> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        public string AuthenticateProfile(string userId, string password, ImportResult importResult , string profileName)
        {
            string token;
            ImportResult errResult;

            //Authenticate
            try
            {
                token = _importService.Authenticate(userId, password);
            }
            catch (Exception exc)
            {
                _logger.LogError("Login error.", exc);
                errResult = SpecificationsImportLogic.CreateResult("Login Error: " + profileName + " import profile", exc.Message);
                importResult.FileName = errResult.FileName;
                importResult.FailingSaved = errResult.FailingSaved;
                importResult.ErrorMsg = errResult.ErrorMsg;
                return null;
            }
            return token;
        }

        public IEnumerable<XElement> GetAccounts(IEnumerable<string> repcodes, ImportResult importResult, string profileName, string token ,int batchNo = 0 ,int batchSize = 0, bool isAutoImport= false)
        {
            // string token;
            ImportResult errResult;
                //GetXML
                var sXML = "";
                try
                {
                    sXML = _importService.GetBalances(token, repcodes);
                }
                catch (Exception exc)
                {
                    _logger.LogError("HttpRequestException: " + profileName + " import profile", exc);
                    errResult = SpecificationsImportLogic.CreateResult("HttpRequestException: " + profileName + " import profile", exc.Message);
                    importResult.FileName = errResult.FileName;
                    importResult.FailingSaved = errResult.FailingSaved;
                    importResult.ErrorMsg = errResult.ErrorMsg;
                    return null;
                }
                if (!isAutoImport)
                {
                    _importService.LogOut(token);
                }
                else
                {
                  if( batchSize == batchNo)
                        _importService.LogOut(token);
                }

                //ParseXML
                var document = XDocument.Parse(sXML);
                var balances = document.Root.XPathSelectElements("./balance");

                return balances.ToArray();
            
        }
    }
}
