

namespace ZegaFinancials.Nhibernate.Entities.Models
{
    public class BlendModelDetails :ZegaEntity
    {
        public virtual Model Model { get; set; }
        public virtual Model SubModel  { get; set; }
        public virtual decimal Allocation { get; set; }
    }
}
