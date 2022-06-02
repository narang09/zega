using System;
using System.Collections.Generic;

namespace ZegaFinancials.Business.Support.SpecificationsImport.ImportData
{
    public class ImportDataItem<T> 
    {
        private DateTime? _fileDate;

        public ImportDataItem()
        {
            Messages = new List<String>();
        }

        public List<String> Messages { get; set; }
        public List<T> Data { get; set; }

        public DateTime? FileDate
        {
            get { return _fileDate; }
            set
            {
                _fileDate = value == null ? (DateTime?)null : value.Value.Date;
            }
        }
    }
}
