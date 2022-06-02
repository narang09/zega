using System;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Models;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support.EventListeners;

namespace ZegaFinancials.Nhibernate.Entities.Accounts
{
    [Logable(EntityType.Account)]
    public class Account : ZegaEntity
    {
        [LogSignature]
        public virtual string Number { get; set; }
        public virtual string Name { get; set; }
        public virtual string ClientName { get; set; }
        public virtual Model Model { get; set; }
        public virtual RepCode RepCode { get; set; }
        [Log(false)]
        public virtual AccountDetails AccountDetail { get; set; }
        [Log(false)]
        public virtual bool isDeleted { get; set; }
        public virtual AccountStatus AccountStatus { get; set; }
        public virtual Broker Broker { get; set; }

        public Account()
        {
            sys_CreatedTime = DateTime.Now;
        }



    }
}
