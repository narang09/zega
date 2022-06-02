using System.ComponentModel;

namespace ZegaFinancials.Nhibernate.Entities.Import
{
    public enum BrokerageFirm
    {
        [Description("TDAmertide")]
        TDAmertide = 0
    }
    public enum ImportStatus
    {
        Success = 1,
        Failed = 0
    }

    public enum ImportType
    {
        Manual = 0,
        Auto = 1,
        File = 2
    }
}
