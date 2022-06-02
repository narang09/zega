using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Advisors
{
    [Logable(EntityType.User, isLogRequire: false)]
    public class AdvisorModel : ZegaEntity
    {
        public virtual User Advisor { get; set; }
        public virtual Model Model { get; set; }
        [LogSignature]
        public virtual string UserModelName
        {
            get
            {
                return string.Format("Model = {0} , User = {1}",
                    Model.Name,
                    Advisor.Details.FirstName + " " + Advisor.Details.LastName
                    );
            }
        }
    }
}
