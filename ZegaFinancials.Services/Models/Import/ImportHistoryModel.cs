using System;
using ZegaFinancials.Nhibernate.Entities.Import;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.Import
{
    public class ImportHistoryModel : ZegaModel
    {
        public ImportType Type { get; set; }
        public string TypeValue { get { return EnumFunctions.GetNameEnumByValue<ImportType>((int)Type); } }
        public DateTime Timestamp { get; set; }
        public string ImportMessage { get; set; }
        public ImportStatus ImportStatus { get; set; }
        public string ImportStatusValue { get { return EnumFunctions.GetNameEnumByValue<ImportStatus>((int)ImportStatus); } }
    }

}
