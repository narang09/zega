using System.Collections.Generic;

namespace ZegaFinancials.Services.Models.Models
{
    public class ModelModel : ZegaModel
    {
        public ModelModel()
        {
            ModelItems = new List<SleeveModel>();
            Strategies = new List<int>();
        }

        public string Description { get; set; }
        public bool IsBlendModel { get; set; }
        public int AccountId { get ;set; }
        public IList<int> Strategies { get; set; }
        public string StrategyNames { get; set; }
        public virtual IList<SleeveModel> ModelItems { get; set; }
        public bool IsLocalBlend => AccountId  != 0;        
        public bool IsSleeveUpdated { get; set; }
    }
}
