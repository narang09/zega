using ZegaFinancials.Nhibernate.Entities.Strategies;

namespace ZegaFinancials.Nhibernate.Mappings.Strategies
{
    public class StrategyMap : ZegaMap<Strategy>
    {
        public StrategyMap()
        {
            Map(x => x.Name).Length(255);
            Map(x => x.Description).Length(4001);
            Map(x => x.IsBlendedStrategy);
            HasMany(x => x.Models).AsBag().KeyColumns.Add("Strategy").Cascade.AllDeleteOrphan();
        }
    }
}
