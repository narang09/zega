using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Mappings.Config
{
    public class ApplicationConfigMap : ZegaMap<ApplicationConfig>
    {
        public ApplicationConfigMap()
        {
            Map(x => x.KeySetting).Length(255);
            Map(x => x.Value).Length(4001);
            Map(x => x.ValueType).CustomType<DataType>();
        }
    }
}
