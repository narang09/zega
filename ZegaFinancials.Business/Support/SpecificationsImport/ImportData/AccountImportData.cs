using CsvHelper.Configuration;
using System;
using ZegaFinancials.Business.Impl.SpecificationsImport;
using ZegaFinancials.Nhibernate.Entities.Accounts;

namespace ZegaFinancials.Business.Support.SpecificationsImport.ImportData
{
    public class AccountImportData
    {
        public AccountImportData(string accountNumber) 
        {
            AccountNumber = accountNumber;
        }
        public string AccountNumber { get; set; }
        public string ClientName { get; set; }
        public decimal? AccountValue { get; set; }
        public string RepCode { get; set; }
        public decimal? CashNetBal { get; set; }
        public decimal? CashEq { get; set; }
        public decimal? SBuyingPower { get; set; }
        public decimal? OBuyingPower { get; set; }
        public DateTime? VEOImportDate { get; set; }
    }
   
    public class AccountFileRowData
    {
       
        public string AccountNumber { get; set; }
        public string ModelName { get; set; }
        public string AccountName { get; set; }
        public DateTime? AllocationDate { get; set; }
        public decimal? CashBalance { get; set; }
        public AccountType? AccountType { get; set; }
        public AccountStatus? AccountStatus { get; set; }
        public WithdrawlorDepositStatus? Future_Withdrwal { get; set; }
        public decimal? Withdrawal_Amount { get; set; }
        public Frequency? Withdrwal_Frequency { get; set; }
        public  DateTime? Withdrawal_Date { get; set; }
        public WithdrawlorDepositStatus? One_Time_Withdrawal { get; set; }
        public decimal? One_Time_Withdrawal_Amount { get; set; }
        public DateTime? One_Time_Withdrawal_Date { get; set; }
        public WithdrawlorDepositStatus? Deposit_Status { get; set; }
        public decimal? Deposit_Amount { get; set; }
        public Frequency? Deposit_Frequency { get; set; }
        public DateTime? Deposit_Date { get; set; }
        public WithdrawlorDepositStatus? ZEGA_Confirmed { get; set; }
        public DateTime? ZEGA_Alert_Date { get; set; }
        public string ZEGA_Notes { get; set; }
        public DateTime? ImportDate { get; set; }
        
       
        

    }

    public class AccountFileRowDataMap : ClassMap<AccountFileRowData>
    {
       public  AccountFileRowDataMap()
        {
            Map(x => x.AccountNumber).Name("Account No.");
            Map(x => x.ModelName).Name("Model");
            Map(x => x.AllocationDate).Name("Allocation Date").TypeConverter<ImportDateTimeConverter>();
            Map(x => x.CashBalance).Name("Cash Balance ($)").TypeConverter<ImportDecimalConverter>();
            Map(x => x.AccountType).Name("Account Type").TypeConverter<ImportEnumConverter<AccountType>>();
            Map(x => x.AccountStatus).Name("Account Status").TypeConverter<ImportEnumConverter<AccountStatus>>();
            Map(x => x.Future_Withdrwal).Name("Additional Withdrawals In The Future").TypeConverter<ImportEnumConverter<WithdrawlorDepositStatus>>();
            Map(x => x.Withdrawal_Amount).Name("Withdrawal Amount").TypeConverter<ImportDecimalConverter>();
            Map(x => x.Withdrwal_Frequency).Name("Withdrawal Frequency").TypeConverter<ImportEnumConverter<Frequency>>();
            Map(x => x.Withdrawal_Date).Name("Withdrawal Date").TypeConverter<ImportDateTimeConverter>();
            Map(x => x.One_Time_Withdrawal).Name("One Time Withdrawal").TypeConverter<ImportEnumConverter< WithdrawlorDepositStatus >>();
            Map(x => x.One_Time_Withdrawal_Amount).Name("One Time Withdrawal Amount").TypeConverter<ImportDecimalConverter>();
            Map(x => x.One_Time_Withdrawal_Date).Name("One Time Withdrawal Date").TypeConverter<ImportDateTimeConverter>();
            Map(x => x.Deposit_Status).Name("Deposit Status").TypeConverter<ImportEnumConverter<WithdrawlorDepositStatus>>();
            Map(x => x.Deposit_Amount).Name("Deposit Amount").TypeConverter<ImportDecimalConverter>();
            Map(x => x.Deposit_Frequency).Name("Deposit Frequency").TypeConverter<ImportEnumConverter<Frequency>>();
            Map(x => x.Deposit_Date).Name("Deposit Date").TypeConverter<ImportDateTimeConverter>();
            Map(x => x.ZEGA_Confirmed).Name("ZEGA Confirmed").TypeConverter<ImportEnumConverter<WithdrawlorDepositStatus>>();
            Map(x => x.ZEGA_Alert_Date).Name("ZEGA Alert Date").TypeConverter<ImportDateTimeConverter>();
            Map(x => x.ZEGA_Notes).Name("ZEGA Notes");
            Map(x => x.ImportDate).Name("Import Date").TypeConverter<ImportDateTimeConverter>();
            Map(x => x.AccountName).Name("Account Name");
        }
    }
}
