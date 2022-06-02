using ZegaFinancials.Nhibernate.Entities.Import.ImportSettings;

namespace ZegaFinancials.Nhibernate.Mappings.Import.ImportSettings
{
    public class TDAmertideSettingsMap : ZegaMap<TDAmertideSettings>
    {
        public TDAmertideSettingsMap()
        {
            References(x => x.Profile).Column("ImportProfile").ForeignKey("FK_TDAmertideSettings_ImportProfile").Index("IDX_TDAmertideSettings_ImportProfile").Cascade.AllDeleteOrphan();
            Map(x => x.UserId).Length(255);
            Map(x => x.Password).Length(255);
            Map(x => x.RepCodeIds).Length(4001);
            Map(x => x.Batches).Length(4001);
        }
    }
}
