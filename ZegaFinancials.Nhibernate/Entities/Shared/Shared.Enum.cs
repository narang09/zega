using System.ComponentModel;

namespace ZegaFinancials.Nhibernate.Entities.Shared
{
    public enum DataType
    {
        [Description("Integer Type")]
        IntegerType = 0,
        [Description("String Type")]
        StringType = 1,
        [Description("Date Type")]
        DateType = 2,
        [Description("Boolean Type")]
        BooleanType = 3,
        [Description("Decimal Type")]
        DecimalType = 4
    }
    public enum RepCodeType
    {
        [Description("Asset Based")]
        AssetBased = 0,
        [Description("Comission based")]
        CommisionBased = 1,
    }

    public enum DataTypes
    {
        [Description("Text")]
        Text = 0,
        [Description("Boolean")]
        Boolean = 1,
        [Description("Date Time")]
        DateTime = 2,
        [Description("Decimal")]
        Decimal = 3,
        [Description("Integer")]
        Integer = 4,
        [Description("List")]
        List = 5
    }
}
