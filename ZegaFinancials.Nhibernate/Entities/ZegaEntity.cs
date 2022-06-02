using System;

namespace ZegaFinancials.Nhibernate.Entities
{
    public class ZegaEntity
    {
        public virtual int Id { get; set; }
        public virtual string sys_CreatedBy { get; set; }
        public virtual string sys_UpdatedBy { get; set; }
        public virtual DateTime sys_CreatedTime { get; set; }
        public virtual DateTime sys_UpdatedTime { get; set; }
    }
}
