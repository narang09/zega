using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Advisors;
using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Users
{
    [Logable(EntityType.User)]
    public class User : ZegaEntity
    {       
        public virtual string Login { get; set; }
        [Log(false)]
        public virtual string Password { get; set; }
        public virtual Status Status { get; set; }
        [Log(false)]
        public virtual string TempPassword { get; set; }
        [Log(false)]
        public virtual bool IsAdmin { get; set; }
        public virtual IList<AdvisorRepcode> RepCodes { get; set; }
        [Log(false)]
        public virtual IList<AdvisorModel> Models { get; set; }
        [Log(false)]
        public virtual IList<UserPhone> PhoneNumbers { get; set; }
        [Log(false)]
        public virtual IList<UserEmail> Emails { get; set; }
        [Log(false)]
        public virtual UserDetails Details { get; set; }
        [Log(false)]
        public virtual IList<GridConfig> GridConfigs { get; set; }
        public virtual int RepCodesCount { get; set; }
        [LogSignature]
        public virtual string UserName
        {
            get
            {
                return string.Format("{0}",
                     Details.FirstName + " " + Details.LastName
                    );
            }
        }
        public virtual string Name { get; set; }
        public virtual string PrimaryEmailId { get; set; }
        public virtual string PrimaryPhoneNumber { get; set; }
    }
}
