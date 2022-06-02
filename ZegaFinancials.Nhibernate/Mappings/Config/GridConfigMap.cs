using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Mappings.Config
{
    public class GridConfigMap : ZegaMap<GridConfig>
    {
        public GridConfigMap()
        {
            Map(x => x.GridName).Length(255);
            Map(x => x.GridType).CustomType<DataRequestSource>();
            Map(x => x.GridColumnJSonValue).Length(4001);
            Map(x => x.IsDefault);
            References(x => x.User).Column("`User`").ForeignKey("FK_GridConfig_User").Index("IDX_GridConfig_User");
            Map(x => x.SortingJSonValue).Length(4001);
        }
    }
}
