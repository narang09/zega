

using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Services.Models.RepCodes;

namespace ZegaFinancials.Services.Models.Users
{
     public class UserViewModel :ZegaModel
    {
        public virtual string Login { get; set; }
        public virtual Status Status { get; set; }
        public virtual int RepCodesCount { get; set; }
        public virtual bool IsAdmin { get; set; }
        public virtual string PrimaryEmailId { get; set; }
        public virtual string PrimaryPhoneNumber { get; set; }
        public virtual IList<RepCodeModel> RepCodes { get; set; }
        public virtual IList<AdvisorModelModel> Models { get; set; }
        public virtual IList<UserPhoneModel> PhoneNumbers { get; set; }
        public virtual IList<UserEmailModel> Emails { get; set; }
        public virtual UserDetailsModel Details { get; set; }


        }
}
