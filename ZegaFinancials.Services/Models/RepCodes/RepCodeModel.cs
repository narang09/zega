using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Services.Models.RepCodes
{
   public  class RepCodeModel :ZegaModel
    {
        public virtual string Code { get; set; }
        public virtual RepCodeType Type { get; set; }
        public string TypeValue { get { return EnumFunctions.GetNameEnumByValue<RepCodeType>((int)Type); } }
    }
}
