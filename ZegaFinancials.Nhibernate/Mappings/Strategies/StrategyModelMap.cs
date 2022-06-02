 
using ZegaFinancials.Nhibernate.Entities.Strategies;

namespace ZegaFinancials.Nhibernate.Mappings.Strategies
{
    public class StrategyModelMap : ZegaMap<StrategyModel>
    {
        public StrategyModelMap()
        {
            References(x => x.Strategy).Column("Strategy").ForeignKey("FK_Strategy_StrategyModel").Index("IDX_Strategy_StrategyModel");
            References(x => x.Model).Column("Model").ForeignKey("FK_StrategyModel_Model").Index("IDX_StrategyModel_Model");
        }
    }
}
