namespace ZegaFinancials.Nhibernate.Entities.Import.ImportSettings
{
    public class TDAmertideSettings : ZegaEntity
    {
        public virtual ImportProfile Profile { get; set; }
        public virtual string UserId { get; set; }
        public virtual string Password { get; set; }
        public virtual string RepCodeIds { get; set; }
        public virtual string Batches { get; set; }
    }
}
