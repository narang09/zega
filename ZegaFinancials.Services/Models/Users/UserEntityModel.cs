using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Services.Models.Users
{
    public class UserEntityModel : ZegaModel
    {
        public virtual string Login { get; set; }
        public virtual string Password { get; set; }
        public virtual Status Status { get; set; }
        public virtual string TempPassword { get; set; }
        public virtual IList<AdvisorRepCodeModel> RepCodes { get; set; }
        public virtual IList<AdvisorModelModel> Models { get; set; }
        public virtual IList<UserPhoneModel> PhoneNumbers { get; set; }
        public virtual IList<UserEmailModel> Emails { get; set; }
        public virtual UserDetailsModel Details { get; set; }
        public virtual int RepCodesCount { get; set; }
    }
}
