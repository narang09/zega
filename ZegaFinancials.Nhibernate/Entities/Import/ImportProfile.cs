using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Import
{
    [Logable(EntityType.Import)]
    public class ImportProfile : ZegaEntity
    {
        [LogSignature]
        public virtual string Name { get; set; }
        public virtual BrokerageFirm BrokerageFirm { get; set; }
        public virtual bool AutoImport { get; set; }
        public virtual string AutoImportTime { get; set; }
    }
}
