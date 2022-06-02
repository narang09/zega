using System.Collections.Generic;
using ZegaFinancials.Services.Models.Models;

namespace ZegaFinancials.Services.Models.Strategies
{
    public class StrategyModels : ZegaModel
    {
        public string Description { get; set; }
        public bool IsBlendedStrategy { get; set; }
        public virtual IList<ModelModel> Models { get; set; }
    }
}
