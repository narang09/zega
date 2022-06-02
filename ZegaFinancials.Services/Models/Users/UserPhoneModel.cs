using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZegaFinancials.Services.Models.Users
{
    public  class UserPhoneModel :ZegaModel
    {
        public virtual string CountryCode { get; set; }
        public virtual string PhoneNo { get; set; }
        public virtual bool IsPrimary { get; set; }

    }
}
