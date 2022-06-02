using System.Collections.Generic;
namespace ZegaFinancials.Services.Models.Models
{
    public class SleeveModel: ZegaModel
    {
        public SleeveModel()
        {
            Items = new List<SleeveModel>();
        }
        public string Description { get; set; }
        public decimal Allocation { get; set; }
        public decimal AllocationUI { get { return Allocation * 100.00m; } }
        public IList<SleeveModel> Items { get; set; }
        public bool IsBlendModel { get; set; }
    }
}
