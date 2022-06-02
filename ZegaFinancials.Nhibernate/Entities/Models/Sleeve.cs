using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Models
{
    [Logable(EntityType.Sleeve)]
    public class Sleeve : ZegaEntity
    {
        [LogSignature]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}
