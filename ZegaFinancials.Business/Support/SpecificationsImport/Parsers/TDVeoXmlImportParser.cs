using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ZegaFinancials.Business.Interfaces.SpecificationsImport;
using ZegaFinancials.Business.Support.Parser;
using ZegaFinancials.Business.Support.SpecificationsImport.ImportData;

namespace ZegaFinancials.Business.Support.SpecificationsImport.Parsers
{
    internal class TDVeoXmlImportParser : ITDVeoXmlImportParser
    {
        private IDictionary<string, Mappings> _accountMapping;
        public void SetFields(IDictionary<string, Mappings> accountMapping)
        {
            _accountMapping = accountMapping;
        }
        private static void ThrowNotExistentFieldException(string nonExistentAttribute)
        {
            throw new XmlParserException(string.Format("Element \"{0}\" was not found.", nonExistentAttribute));
        }
        public ImportDataItem<AccountImportData> Parse(IEnumerable<XElement> elements)
        {
           var ImportData = new ImportDataItem<AccountImportData>();
            if (elements != null)
            {
                var accounts = new Dictionary<string, AccountImportData>();

                foreach (var accountData in elements)
                {
                    var accountNumberData = accountData.Element("accountNumber");
                    if (accountNumberData == null)
                        ThrowNotExistentFieldException("AccountNumber");

                    var accountNumber = accountNumberData.Value;
                    var account = new AccountImportData(accountNumber);

                    ParseAccountData(_accountMapping, accountData, account);

                    if (!accounts.ContainsKey(account.AccountNumber))
                        accounts.Add(account.AccountNumber, account);
                }
                var importDate = DateTime.Now;
                ImportData = new ImportDataItem<AccountImportData> { Data = accounts.Values.ToList(), FileDate = importDate };
            }
            return ImportData;
        }
        private void ParseAccountData(IDictionary<string, Mappings> propertyMapping, XElement element, AccountImportData account)
        {
            XmlParserHelper.FillDataWithElements(propertyMapping, element, account);
        }       
    }
    }
