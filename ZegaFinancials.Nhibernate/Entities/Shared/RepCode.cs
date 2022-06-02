using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Shared
{
    [Logable(EntityType.RepCode, isLogRequire: false)]
    public class RepCode : ZegaEntity
    {
        [LogSignature]
        public virtual string Code { get; set; }
        public virtual RepCodeType Type { get; set; }

    }
}
