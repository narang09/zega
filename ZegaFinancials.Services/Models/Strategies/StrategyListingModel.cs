using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZegaFinancials.Services.Models.Strategies
{
    public class StrategyListingModel: ZegaModel
    {
        public string Description { get; set; }
        public virtual int ModelsCount { get; set; }
    }
}
