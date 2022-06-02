using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Nhibernate.Entities.Config
{
    public class ApplicationConfig : ZegaEntity
    {
        public virtual string KeySetting { get; set; }
        public virtual string Value { get; set; }
        public virtual DataType ValueType { get; set; }

    }
}
