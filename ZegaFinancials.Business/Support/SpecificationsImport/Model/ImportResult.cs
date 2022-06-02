using System.Collections.Generic;

namespace ZegaFinancials.Business.Support.SpecificationsImport.Model
{
    public class ImportResult 
    {        
        public virtual string FileName { get; set; }
        public virtual List<string> ErrorMsg { get; set; }        
        public virtual string Message { get; set; }
        public virtual int SuccessfullyCount { get; set; }
        public virtual int FailingSaved { get; set; }    
        public virtual bool ImportIsBusy { get; set; }
    }

}
