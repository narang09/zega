using System.Collections.Generic;

namespace ZegaFinancials.Business.Support.SpecificationsImport
{
    public class ImportPersistResult
    {
		public int TotalCount { get; set; }
		public int SuccessfullyCount { get; set; }
		public virtual List<string> ErrorMsg { get; set; }
	}	
}
