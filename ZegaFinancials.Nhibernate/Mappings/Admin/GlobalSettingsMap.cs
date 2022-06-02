using ZegaFinancials.Nhibernate.Entities.Admin;

namespace ZegaFinancials.Nhibernate.Mappings.Admin
{
    public class GlobalSettingsMap : ZegaMap<GlobalSettings>
    {
        public GlobalSettingsMap()
        {
            Map(x => x.SchedulerImportHour);
            Map(x => x.SchedulerImportMinute);
        }
    }
}
