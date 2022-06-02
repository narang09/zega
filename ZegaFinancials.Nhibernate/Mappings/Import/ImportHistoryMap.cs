using ZegaFinancials.Nhibernate.Entities.Import;

namespace ZegaFinancials.Nhibernate.Mappings.Import
{
    public class ImportHistoryMap : ZegaMap<ImportHistory>
    {
        public ImportHistoryMap()
        {
            Map(x => x.ImportType).CustomType<ImportType>();
            Map(x => x.ImportMessage).Length(4001);
            Map(x => x.Timestamp);
            Map(x => x.Status).CustomType<ImportStatus>();
        }
    }
}
