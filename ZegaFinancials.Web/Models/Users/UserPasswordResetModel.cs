using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZegaFinancials.Web.Models.Users
{
    public class UserPasswordResetModel
    {
        public string Login { get; set; }
        public string Password { get ; set; }
        public string AuthToken { get; set; }
    }
}
