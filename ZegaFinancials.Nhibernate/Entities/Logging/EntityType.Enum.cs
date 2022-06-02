using System.ComponentModel;

namespace ZegaFinancials.Nhibernate.Entities.Logging
{
    public enum EntityType
    {
       [Description("Account")]
        Account = 0,
       [Description("Model")]
        Model = 1,
       [Description("Sleeve")]
        Sleeve = 2,
       [Description("User")]
        User = 3,
       [Description("Import")]
        Import = 4,
       [Description("Strategy")]
        Strategy = 5,
       [Description("RepCode")]
        RepCode = 6,
       [Description("CustomField")] //Adding Custom Field and ModelSleeve as per the requirement in Trade List project
        CustomField = 7,
       [Description("ModelSleeve")]
       ModelSleeve = 8
    }
}
