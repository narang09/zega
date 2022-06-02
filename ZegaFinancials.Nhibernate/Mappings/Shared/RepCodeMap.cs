using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Mappings.Shared
{
    public class RepCodeMap : ZegaMap<RepCode>
    {
        public RepCodeMap()
        {
            Map(x => x.Code);
            Map(x => x.Type).CustomType<RepCodeType>();
        }
    }
}
