using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Models
{
    [Logable(EntityType.Model)]
    public class Model : ZegaEntity
    {
        [LogSignature]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        [Log(false)]
        public virtual IList<ModelSleeve> ModelSleeves { get; set; }
        [Log(false)]
        public virtual IList<BlendModelDetails> SubModels { get; set; }
        [Log(false)]
        public virtual bool IsBlendModel { get; set; }
        [Log(false)]
        public virtual bool IsLocalBlend { get; set; }
        [Log(false)]
        public virtual int AccountId { get; set; }
    }
}
