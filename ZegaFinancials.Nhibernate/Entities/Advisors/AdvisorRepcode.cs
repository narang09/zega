using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Advisors
{
    [Logable(EntityType.User, isLogRequire: false)]
    public class AdvisorRepcode : ZegaEntity
    {
        public virtual User Advisor { get; set; }
        public virtual RepCode RepCode { get; set; }
        [LogSignature]
        public virtual string UserRepcode
        {
            get
            {
                return string.Format("RepCode = {0} , User = {1}",
                    RepCode.Code,
                    Advisor.Details.FirstName + " " + Advisor.Details.LastName
                    );
            }
        }
    }
}
