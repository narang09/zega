using System.Collections.Generic;
using System.Xml.Linq;
using ZegaFinancials.Business.Support.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport.ImportData;

namespace ZegaFinancials.Business.Interfaces.SpecificationsImport
{
    public interface ITDVeoXmlImportParser
    {
        void SetFields(IDictionary<string, Mappings> accountMapping);
        ImportDataItem<AccountImportData> Parse(IEnumerable<XElement> elements);
    }
}
