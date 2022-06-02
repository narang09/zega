using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support;

namespace ZegaFinancials.Business.Shared.Models
{
    public class DataGridPreferenceModel
    {
        public DataGridPreferenceModel()
        {
            SortDescriptions = new List<SortDescription>();
            DataGridColumnObjects = new List<DataGridColumnObject>();
        }
        public virtual string GridName { get; set; }
        public List<DataGridColumnObject> DataGridColumnObjects { get; set; }
        public List<SortDescription> SortDescriptions { get; set; }
        public virtual string UserLogin {get;set;}
    }
}

