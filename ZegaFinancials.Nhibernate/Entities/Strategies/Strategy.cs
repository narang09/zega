using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Strategies
{
    [Logable(EntityType.Strategy)]
    public class Strategy : ZegaEntity
    {
        [LogSignature]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<StrategyModel> Models { get; set; }
        public virtual int ModelsCount { get; set; }
        [Log(false)]
        public virtual bool IsBlendedStrategy {get ;set;}
    }
}
