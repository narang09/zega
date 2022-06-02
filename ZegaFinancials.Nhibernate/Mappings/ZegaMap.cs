using FluentNHibernate.Mapping;
using ZegaFinancials.Nhibernate.Entities;

namespace ZegaFinancials.Nhibernate.Mappings
{
    public class ZegaMap<T> : ClassMap<T> where T : ZegaEntity
    {
        public ZegaMap()
        {
            Id(x => x.Id).GeneratedBy.HiLo("100");

        }
    }
}
