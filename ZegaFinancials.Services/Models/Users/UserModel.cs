using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.Users
{
    public class UserModel : ZegaModel
    {       
        public string Login { get; set; }
        public int RepCodesCount { get; set; }
        public string PrimaryEmailId { get; set; }
        public Status Status { get; set; }
        public string StatusValue { get { return EnumFunctions.GetNameEnumByValue<Status>((int)Status); } }
        public object PrimaryPhoneNumber { get; internal set; }
    }
}
