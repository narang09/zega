using System;
using System.Collections.Generic;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Services.Models.Strategies;

namespace ZegaFinancials.Services.Models.Users
{
    public class AdvisorModelModel : ZegaModel 
    {
       public virtual string Description { get; set; }
        public virtual IList<ModelSleeveModel> ModelSleeves { get; set; }
    }
}
