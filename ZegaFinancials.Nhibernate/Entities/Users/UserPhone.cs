using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Users
{
    [Logable(EntityType.User, isLogRequire: false)]
    public class UserPhone : ZegaEntity
    {
        public virtual User User { get; set; }
        public virtual string CountryCode { get; set; }
        public virtual string PhoneNo { get; set; }
        public virtual bool IsPrimary { get; set; }

        [LogSignature]

        public virtual string UserPhoneNo
        {
            get
            {
                return string.Format(" Phone No. = {0} , User = {1}",
                    PhoneNo,
                    User != null && User.Details != null ? User.Details.FirstName + " " + User.Details.LastName : string.Empty
                    );
            }
        }
    }
}
