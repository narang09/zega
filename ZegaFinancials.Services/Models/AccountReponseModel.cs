using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZegaFinancials.Services.Models
{
    public class AccountReponseModel : ZegaModel
    {
        public int AccountId { get ; set; }
        public IList<int> AvisorIds { get ; set;}
    }
}
