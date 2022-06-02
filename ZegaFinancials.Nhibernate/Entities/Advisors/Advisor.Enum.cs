using System.ComponentModel;

namespace ZegaFinancials.Nhibernate.Entities.Advisors
{
    public enum AdvisorType
    {
        [Description("Asset Based")]
         AssetBased = 0,
        [Description("Comission based")]
         CommisionBased = 0,
    }
}
