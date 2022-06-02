using System.Collections.Generic;

namespace ZegaFinancials.Services.Models.Import
{
    public class ImportProfileModel : ZegaModel
    {
        public ImportProfileModel()
        {
            Batches = new List<List<int>>();
        }
        public IList<int> RepCodes { get; set; }
        public string Login { get; set; }
        public string Password { get; set;}
        public bool AutoImport { get; set; }
        public string SchedulerImportTime { get; set; }
        public IList<List<int>> Batches { get; set; }
    }
}
