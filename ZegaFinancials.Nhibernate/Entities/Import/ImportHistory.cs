using System;

namespace ZegaFinancials.Nhibernate.Entities.Import
{
    public class ImportHistory : ZegaEntity
    {
        public virtual ImportType ImportType { get; set; }
        public virtual string ImportMessage { get; set; }
        public virtual DateTime Timestamp { get; set; }
        public virtual ImportStatus Status { get; set; }
    }
}
