using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Interfaces.CustomFields;
using ZegaFinancials.Services.Models.CustomFields;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Impl.CustomFields
{
    public class CustomFieldService : ICustomFieldService
    {
        public CustomFieldModel[] LoadFieldsByEntityType(UserContextModel userContext)
        {
            // new fileds Present in Zega but not in ATOM := Future_Withdrawal, C4_Withdrawal_Frequency, One_Time_Withdrawal, One_Time_Withdrawal_Amount, One_Time_Withdrawal_Date, D4_Deposit_Frequency
           
            var customFields = new CustomFieldModel[]
            { 
                new CustomFieldModel { Name = "AccountValue", DataType = DataTypes.Decimal},
                new CustomFieldModel { Name = "Acct_Type", DataType = DataTypes.List},
                new CustomFieldModel { Name = "B0_Tradable", DataType = DataTypes.List},
                new CustomFieldModel { Name = "B2_Allocation_Start_Date", DataType = DataTypes.DateTime},
                new CustomFieldModel { Name = "B3_Special_Instructions", DataType = DataTypes.Text},
                new CustomFieldModel { Name = "C1_Withdrawal_Amount", DataType = DataTypes.Decimal},
                new CustomFieldModel { Name = "C2_Withdrawal_Date", DataType = DataTypes.DateTime},
                new CustomFieldModel { Name = "C3_Withdrawal_Status", DataType = DataTypes.List},
                new CustomFieldModel { Name = "CashEq", DataType = DataTypes.Decimal},
                new CustomFieldModel { Name = "CashNetBal", DataType = DataTypes.Text},
                new CustomFieldModel { Name = "D1_Deposit", DataType = DataTypes.Decimal},
                new CustomFieldModel { Name = "D2_Deposit_Date", DataType = DataTypes.DateTime},
                new CustomFieldModel { Name = "D3_Deposit_Status", DataType = DataTypes.List},
                new CustomFieldModel { Name = "OBuyingPower", DataType = DataTypes.Decimal},
                new CustomFieldModel { Name = "SBuyingPower", DataType = DataTypes.Decimal},
                new CustomFieldModel { Name = "VEOImportDate", DataType = DataTypes.DateTime},
               // new CustomFieldModel { Name = "Z1_Closing_Date", DataType = DataTypes.DateTime}, Note:::::: not present in the ATOM
                new CustomFieldModel { Name = "Z2_ZEGA_Confirmed", DataType = DataTypes.Text},
                new CustomFieldModel { Name = "Z3_ZEGA_Alert Date", DataType = DataTypes.DateTime},
                new CustomFieldModel { Name = "Z4_ZEGA_Notes", DataType = DataTypes.Text}
            };
            return customFields;
        }
    }
}
