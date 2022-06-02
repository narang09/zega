using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZegaFinancials.Services.Models.Users
{
    public class UserEmailModel :ZegaModel
    {
        public virtual string Email { get; set; }
        public virtual bool IsPrimary { get; set; }
    }
}
