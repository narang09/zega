using System.ComponentModel;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Users
{
    [Logable(EntityType.User, isLogRequire: false)]
    public class UserDetails : ZegaEntity
    {
        public virtual User User { get; set; }
        public virtual string FirstName { get; set; }
        [Log(false)]
        public virtual string MiddleName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Company { get; set; }
        public virtual string Designation { get; set; }
        public virtual byte[] Image { get; set; }
        [LogSignature]
        public virtual string UserName
        {
            get
            {
                return string.Format(" User Name = {0}",
                     FirstName + " " + LastName
                    );
            }
        }

    }
}
