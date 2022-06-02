namespace ZegaFinancials.Nhibernate.Entities.Admin
{
    public class GlobalSettings : ZegaEntity
    {
        public virtual int SchedulerImportHour { get; set; }
        public virtual int SchedulerImportMinute { get; set; }
    }
}
