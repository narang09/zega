using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Users
{
    [Logable(EntityType.User, isLogRequire: false)]
    public class UserEmail : ZegaEntity
    {
        public virtual User User { get; set; }
        public virtual string Email { get; set; }
        [Log(false)]
        public virtual bool IsPrimary { get; set; }
        [LogSignature]

        public virtual string UserEmailID
        {
            get
            {
                return string.Format("Email = {0} , User = {1}",
                    Email,
                    User != null && User.Details != null ? User.Details.FirstName + " " + User.Details.LastName : string.Empty
                    );
            }
        }
    }
}
