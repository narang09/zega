using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Strategies
{
    [Logable(EntityType.Strategy)]
    public class StrategyModel: ZegaEntity
    {
        public virtual Strategy Strategy { get; set; }
        public virtual Model Model { get; set; }

        [LogSignature]
        public virtual string StrategyModelName
        {
            get
            {
                return string.Format("Model = {0} , Strategy = {1}",
                    Model.Name,
                    Strategy.Name
                    );
            }
        }
    }
}
