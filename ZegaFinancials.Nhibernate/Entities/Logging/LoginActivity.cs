using System;
using ZegaFinancials.Nhibernate.Entities.Users;

namespace ZegaFinancials.Nhibernate.Entities.Logging
{
    public class LoginActivity : ZegaEntity
    {
        public virtual User User { get; set; } 
        public virtual string IPAddress { get; set; }
        public virtual string SessionId { get; set; }
    }
}
