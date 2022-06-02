using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Models
{
    public class ModelSleeve : ZegaEntity
    {
        public virtual Model Model { get; set; }
        public virtual Sleeve Sleeve { get; set; }
        public virtual decimal Allocation { get; set; }
        [LogSignature]
        public virtual string ModelSleeveName
        {
            get
            {
                return string.Format("Sleeve = {0} , Model = {1}",
                    Sleeve.Name,
                    Model.Name
                    );
            }
        }
    }
}
