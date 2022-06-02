using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Services.Models.Users
{
    public class AdvisorRepCodeModel :ZegaModel
    {
        public virtual string Code { get; set; }
        public virtual RepCodeType Type { get; set; }
    }
}
