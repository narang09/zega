using ZegaFinancials.Nhibernate.Entities.Models;

namespace ZegaFinancials.Nhibernate.Mappings.Models
{
    public class SleeveMap : ZegaMap<Sleeve>
    {
        public SleeveMap()
        {
            Map(x => x.Name).Length(255);
            Map(x => x.Description).Length(4001);
        }
    }
}
