using System;
 
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Services.Models.Users
{
    public class UserDetailsModel:ZegaModel
    {
        public virtual string FirstName { get; set; }
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual bool SuperUser { get; set; }
        public virtual string Company { get; set; }
        public virtual string Designation { get; set; }
        public virtual byte[] Image { get; set; }

    }
}
