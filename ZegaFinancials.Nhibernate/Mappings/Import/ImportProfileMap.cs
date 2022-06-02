using ZegaFinancials.Nhibernate.Entities.Import;

namespace ZegaFinancials.Nhibernate.Mappings.Import
{
    public class ImportProfileMap : ZegaMap<ImportProfile>
    {
        public ImportProfileMap()
        {
            Map(x => x.Name).Length(255);
            Map(x => x.BrokerageFirm).CustomType<BrokerageFirm>();
            Map(x => x.AutoImport);
            Map(x => x.AutoImportTime);
        }
    }
}
