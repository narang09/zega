using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHibernate;
using System;
using System.Collections.Generic;
using ZegaFinancials.Business.Interfaces;
using ZegaFinancials.Business.Interfaces.Strategies;
using ZegaFinancials.Business.Support.Extensions;
using ZegaFinancials.Nhibernate.Dao.Interface.DataGrid;
using ZegaFinancials.Nhibernate.Dao.Interface.Strategies;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Strategies;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Nhibernate.Shared.Enums;
using ZegaFinancials.Nhibernate.Support;

namespace ZegaFinancials.Business.Impl
{
    public class InitialDataLogic : ZegaLogic, IInitialDataLogic
    {
        private readonly IDataGridDao _dataGridDao;
        private readonly ISession _session;
        private readonly IUserDao _userDao;
        private readonly IStrategyDao _strategyDao;
        private readonly ILogger<InitialDataLogic> _logger;
        private readonly ILoginActivityDao _loginActivityDao;
        public InitialDataLogic(IDataGridDao gridConfigDao, IUserDao userDao, ILogger<InitialDataLogic> logger, IStrategyDao strategyDao, ILoginActivityDao loginActivityDao, ISession session)
        {
            _dataGridDao = gridConfigDao;
            _session = session;
            _userDao = userDao;
            _logger = logger;
            _loginActivityDao = loginActivityDao;
            _strategyDao = strategyDao;
        }
        public void CreateDefaultGridHeaders()
        {
            if (_dataGridDao.GetAll().Count == 0)
            {
                using (ITransaction transaction = _session.BeginTransaction())
                {
                    try
                    {
                        List<GridConfig> gridConfigs = new List<GridConfig>
                        {
                            CreateDefaultDashboardAdminHeaders(),
                            CreateDefaultDashboardAdvisorHeaders(),
                            CreateDefaultAccountListingHeaders(),
                            CreateDefaultModelListingHeaders(),
                            CreateDefaultModelListingSubGridHeaders(),
                            CreateDefaultStrategyListingHeaders(),
                            CreateDefaultSleeveListingHeaders(),
                            CreateDefaultAdvisorListingHeaders(),
                            CreateDefaultAuditLogListingHeaders(),
                            CreateDefaultUserContactNumbersHeaders(),
                            CreateDefaultUserEmailsHeaders(),
                            CreateDefaultAdvisorRepCodesHeaders(),
                            CreateDefaultModelListingSidebarHeaders(),
                            CreateDefaultImportHistoryHeaders(),
                        };
                        _dataGridDao.Persist(gridConfigs);

                        transaction.Commit();
                        _logger.LogInformation("Default Headers Created !");

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError("Header", ex.Message);
                    }
                }
            }
        }
        public void CreateDefaultUser()
        {
                using (ITransaction transaction = _session.BeginTransaction())
                {
                try
                {
                    var zegaUser = new User()
                    {
                        Login = "Admin",
                        Password = PasswordEncoder.EncryptPassword("ZegaAdmin#123"),
                        Status = Status.Active,
                        Emails = new List<UserEmail>() { new UserEmail() { IsPrimary = true, Email = "anish@hexaviewtech.com" } },
                        PhoneNumbers = new List<UserPhone>() { new UserPhone() { IsPrimary = true, CountryCode = "+1", PhoneNo = "9560644015" } },
                        IsAdmin = true
                    };
                    zegaUser.Details = new UserDetails() { User = zegaUser, FirstName = "Zega", LastName = "Admin", Company = "ZEGA Financials", Designation = "Admin" };
                    var tlUser = new User()
                    {
                        Login = "TlAdmin",
                        Password = PasswordEncoder.EncryptPassword("ZegaTlAdmin#123"),
                        Status = Status.Active,
                        Emails = new List<UserEmail>() { new UserEmail() { IsPrimary = true, Email = "alok.shriwastava@hexaviewtech.com" } },
                        PhoneNumbers = new List<UserPhone>() { new UserPhone() { IsPrimary = true, CountryCode = "+1", PhoneNo = "7607227499" } },
                        IsAdmin = true
                    };
                    tlUser.Details = new UserDetails() { User = tlUser, FirstName = "TradeList", LastName = "Admin", Company = "ZEGA Financials", Designation = "TradeList Admin" };
                    if (_userDao.GetAll() == null || _userDao.GetAll().Count == 0)
                    {
                        _userDao.Persist(zegaUser);
                        _userDao.Persist(tlUser);
                         transaction.Commit();
                        _logger.LogInformation("Default and TradeList Users Created !");

                    }
                    else if(_userDao.GetByLoginId("TlAdmin") == null)
                     {
                        _userDao.Persist(tlUser);
                         transaction.Commit();
                        _logger.LogInformation("TradeList Users Created !");
                     }
                  
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError("User", ex.Message);
                }
                
                }
        }

        private GridConfig CreateDefaultDashboardAdminHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Advisor Name", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "StatusValue", DisplayName = "Status", Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.UserActive, MinWidth = 140, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "AccountsCount", DisplayName = "No. of Accounts", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number, MinWidth = 75, DisplayIndex = 2},
                    new DataGridColumnObject { DataField = "ModelsCount", DisplayName = "No. of Models", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number, MinWidth = 75, DisplayIndex = 3},
                };

            var dashboardAdminConfig = new GridConfig()
            {
                GridName = DataRequestSource.DashboardAdmin.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.DashboardAdmin,
                IsDefault = true
            };
            return dashboardAdminConfig;
        }

        private GridConfig CreateDefaultImportHistoryHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "TypeValue", DisplayName = "Type", Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "ImportMessage", DisplayName = "Import Message", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "Timestamp", DisplayName = "Timestamp", Type = GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.DateTime, MinWidth = 150, DisplayIndex = 2},
                    new DataGridColumnObject { DataField = "ImportStatusValue", DisplayName = "Status", Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 3},
                };
            var sortingObject = new List<SortDescription>
            {
                new SortDescription { Field = "Timestamp", FieldDirection = "desc", Priority = 0}
            };

            var importHistoryConfig = new GridConfig()
            {
                GridName = DataRequestSource.ImportHistory.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.ImportHistory,
                IsDefault = true,
                SortingJSonValue = JsonConvert.SerializeObject(sortingObject)
            };
            return importHistoryConfig;
        }

        private GridConfig CreateDefaultDashboardAdvisorHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "accountBasicDetails.ClientName", DisplayName = "Description", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Account Name", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "accountBasicDetails.Number", DisplayName = "Account No.", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 75, DisplayIndex = 2},
                    new DataGridColumnObject { DataField = "Model.Name", DisplayName = "Model", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 3},
                    new DataGridColumnObject { DataField = "accountBasicDetails.CashNetBal", DisplayName = "Cash Balance ($)", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number, MinWidth = 75, DisplayIndex = 4},
                    new DataGridColumnObject { DataField = "accountBasicDetails.VeoImportDate", DisplayName = "Import Date", Type = GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.Date, MinWidth = 150, DisplayIndex = 5},
                    new DataGridColumnObject { DataField = "accountBasicDetails.AccountTypeValue" , DisplayName = "Account Type", Type = GridColumnTypes.Enum ,TemplateType = GridTemplateTypes.String, MinWidth = 150 ,DisplayIndex = 6 },
                    new DataGridColumnObject { DataField = "accountBasicDetails.AllocationDate", DisplayName = "Allocation Date",Type = GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.DateTime, MinWidth = 150 ,DisplayIndex = 7},
                    new DataGridColumnObject { DataField = "accountBasicDetails.OBuyingPower", DisplayName = "OBuyingPower" ,Type = GridColumnTypes.Number,TemplateType = GridTemplateTypes.Currency,MinWidth = 75, DisplayIndex = 8},
                    new DataGridColumnObject { DataField = "accountBasicDetails.SBuyingPower", DisplayName = "SBuyingPower", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Currency ,MinWidth = 75, DisplayIndex = 9},
                    new DataGridColumnObject { DataField = "accountBasicDetails.CashEq", DisplayName = "Cash Equivalent", Type = GridColumnTypes.Number ,TemplateType = GridTemplateTypes.Currency,MinWidth = 75, DisplayIndex = 10},
                    new DataGridColumnObject { DataField = "accountBasicDetails.AccountValue", DisplayName = "Account Value" ,Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Currency, MinWidth = 150, DisplayIndex = 11},
                    new DataGridColumnObject { DataField = "accountBasicDetails.RepCode.Name", DisplayName = "RepCode" ,Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 12}
                };

            var dashboardAdvisorGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.DashboardAdvisor.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.DashboardAdvisor,
                IsDefault = true
            };
            return dashboardAdvisorGridConfig;
        }

        private GridConfig CreateDefaultAccountListingHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "accountBasicDetails.ClientName", DisplayName = "Description", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Account Name", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "accountBasicDetails.Number", DisplayName = "Account No.", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 75, DisplayIndex = 2},
                    new DataGridColumnObject { DataField = "Model.Name", DisplayName = "Model", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 3},
                    new DataGridColumnObject { DataField = "accountBasicDetails.CashNetBal", DisplayName = "Cash Balance ($)", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number, MinWidth = 75, DisplayIndex = 4},
                    new DataGridColumnObject { DataField = "accountBasicDetails.VeoImportDate", DisplayName = "Import Date", Type = GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.Date, MinWidth = 150, DisplayIndex = 5},
                    new DataGridColumnObject { DataField = "accountBasicDetails.AccountStatusValue", DisplayName = "Account Status", Type = GridColumnTypes.Enum ,TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 6},
                    new DataGridColumnObject { DataField = "accountBasicDetails.AccountTypeValue" , DisplayName = "Account Type", Type = GridColumnTypes.Enum ,TemplateType = GridTemplateTypes.String, MinWidth = 150 ,DisplayIndex = 7 },
                    new DataGridColumnObject { DataField = "accountBasicDetails.AllocationDate", DisplayName = "Allocation Date",Type = GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.DateTime, MinWidth = 150 ,DisplayIndex = 8},
                    new DataGridColumnObject { DataField = "accounWithdrawlInfoModelcs.Future_Withdrawal_StatusValue", DisplayName = "Additional Withdrawals In The Future" ,Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 9},
                    new DataGridColumnObject { DataField = "accounWithdrawlInfoModelcs.Withdrawl_Amount" , DisplayName = "Withdrawal Amount",Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number,MinWidth = 75, DisplayIndex = 10},
                    new DataGridColumnObject { DataField = "accounWithdrawlInfoModelcs.Withdrawal_FrequencyValue", DisplayName = "Withdrawal Frequency" ,Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 11},
                    new DataGridColumnObject { DataField = "accounWithdrawlInfoModelcs.Withdrawl_Date", DisplayName = "Withdrawal Date", Type =GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.DateTime, MinWidth = 150, DisplayIndex = 12},
                    new DataGridColumnObject { DataField = "accounWithdrawlInfoModelcs.One_Time_WithdrawalValue", DisplayName = "One Time Withdrawal" ,Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 13},
                    new DataGridColumnObject { DataField = "accounWithdrawlInfoModelcs.One_Time_Withdrawal_Amount" , DisplayName = "One Time Withdrawal Amount",Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number,MinWidth = 75, DisplayIndex = 14},
                    new DataGridColumnObject { DataField = "accounWithdrawlInfoModelcs.One_Time_Withdrawal_Date", DisplayName = "One Time Withdrawal Date", Type =GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.DateTime, MinWidth = 150, DisplayIndex = 15},
                    new DataGridColumnObject { DataField = "accountDepositsInfoModelcs.Deposit_StatusValue", DisplayName = "Deposit Status" ,Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 16},
                    new DataGridColumnObject { DataField = "accountDepositsInfoModelcs.Deposit_Amount", DisplayName = "Deposit Amount" ,Type = GridColumnTypes .Number, TemplateType = GridTemplateTypes.Number, MinWidth = 75, DisplayIndex = 17},
                     new DataGridColumnObject { DataField = "accountDepositsInfoModelcs.Deposit_FrequencyValue", DisplayName = "Deposit Frequency" ,Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 18},
                    new DataGridColumnObject { DataField = "accountDepositsInfoModelcs.Deposit_Date", DisplayName = "Deposit Date" ,Type = GridColumnTypes .DateTime, TemplateType = GridTemplateTypes.DateTime, MinWidth = 150, DisplayIndex = 19},
                    new DataGridColumnObject { DataField = "accountZegaCustomFieldsModel.Zega_ConfirmedValue", DisplayName = "ZEGA Confirmed" ,Type = GridColumnTypes.Enum ,TemplateType =GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 20},
                    new DataGridColumnObject { DataField = "accountZegaCustomFieldsModel.Zega_Alert_Date", DisplayName = "ZEGA Alert Date" ,Type = GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.DateTime ,DisplayIndex = 21},
                    new DataGridColumnObject { DataField = "accountZegaCustomFieldsModel.Zega_Notes", DisplayName = "ZEGA Notes" ,Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 22},
                    new DataGridColumnObject { DataField = "accountBasicDetails.OBuyingPower", DisplayName = "OBuyingPower" ,Type = GridColumnTypes.Number,TemplateType = GridTemplateTypes.Number,MinWidth = 75, DisplayIndex = 23},
                    new DataGridColumnObject { DataField = "accountBasicDetails.SBuyingPower", DisplayName = "SBuyingPower", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number ,MinWidth = 75, DisplayIndex = 24},
                    new DataGridColumnObject { DataField = "accountBasicDetails.CashEq", DisplayName = "Cash Equivalent", Type = GridColumnTypes.Number ,TemplateType = GridTemplateTypes.Number,MinWidth = 75, DisplayIndex = 25},
                    new DataGridColumnObject { DataField = "accountBasicDetails.AccountValue", DisplayName = "Account Value" ,Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number, MinWidth = 150, DisplayIndex = 26},
                    new DataGridColumnObject { DataField = "accountBasicDetails.AdvisorsName" ,DisplayName = "Advisor" ,Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 27},
                    new DataGridColumnObject { DataField = "accountBasicDetails.RepCode.Name" ,DisplayName = "RepCode" ,Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 28},
                    new DataGridColumnObject { DataField = "accountBasicDetails.BrokerValue" ,DisplayName = "Broker" ,Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 29},
            };

            var accountGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.AccountListing.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.AccountListing,
                IsDefault = true
            };
            return accountGridConfig;
        }

        private GridConfig CreateDefaultStrategyListingHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Name", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "ModelsCount", DisplayName = "No of Models", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number, MinWidth = 75, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "Description", DisplayName = "Description", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 2},
                };

            var modelGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.StrategyListing.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.StrategyListing,
                IsDefault = true
            };
            return modelGridConfig;
        }

        private GridConfig CreateDefaultModelListingHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Name", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "Description", DisplayName = "Description", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "StrategyNames", DisplayName = "Strategy", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 2 },
                };

            var modelGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.ModelListing.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.ModelListing,
                IsDefault = true
            };
            return modelGridConfig;
        }

        private GridConfig CreateDefaultModelListingSubGridHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Name", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 100, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "Description", DisplayName = "Description", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 100, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "AllocationUI", DisplayName = "Allocation (%)", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number, MinWidth = 50, DisplayIndex = 2, IsEditable = true, DataValidators = new List<string> { "required", "percentage"} }
                };

            var modelSubGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.ModelListingSubGrid.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.ModelListingSubGrid,
                IsDefault = true
            };
            return modelSubGridConfig;
        }

        private GridConfig CreateDefaultAuditLogListingHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "EntityTypeValue", DisplayName = "Entity", Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "Message", DisplayName = "Message", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "Date", DisplayName = "Date Time", Type = GridColumnTypes.DateTime, TemplateType = GridTemplateTypes.DateTime, MinWidth = 150, DisplayIndex = 2},
                    new DataGridColumnObject { DataField = "Name", DisplayName = "User", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 3}
                };
            var sortingObject = new List<SortDescription>
            {
                new SortDescription {Field = "Date", FieldDirection = "desc", Priority = 0}
            };

            var modelGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.AuditLogListing.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.AuditLogListing,
                IsDefault = true,
                SortingJSonValue = JsonConvert.SerializeObject(sortingObject)
            };
            return modelGridConfig;
        }

        private GridConfig CreateDefaultSleeveListingHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Name", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "Description", DisplayName = "Description", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 1}
                };

            var sleeveGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.SleeveListing.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.SleeveListing,
                IsDefault = true
            };
            return sleeveGridConfig;
        }

        private GridConfig CreateDefaultAdvisorListingHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Name", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0},
                    new DataGridColumnObject { DataField = "RepCodesCount", DisplayName = "No. of RepCodes", Type = GridColumnTypes.Number, TemplateType = GridTemplateTypes.Number, MinWidth = 75, DisplayIndex = 1},
                    new DataGridColumnObject { DataField = "Login", DisplayName = "User Id", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 2},
                    new DataGridColumnObject { DataField = "PrimaryEmailId", DisplayName = "Email Id", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 3},
                    new DataGridColumnObject { DataField = "StatusValue", DisplayName = "Status", Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.UserActive, MinWidth = 140, DisplayIndex = 4},
            };

            var advisorGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.AdvisorListing.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.AdvisorListing,
                IsDefault = true
            };
            return advisorGridConfig;
        }

        private GridConfig CreateDefaultUserContactNumbersHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "CountryCodeValue", DisplayName = "Country", Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 50, DisplayIndex = 0, IsSortingEnabled = false, IsFilteringEnabled = false},
                    new DataGridColumnObject { DataField = "PhoneNo", DisplayName = "Phone No", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 100, DisplayIndex = 1, IsSortingEnabled = false, IsFilteringEnabled = false},
                    new DataGridColumnObject { DataField = "IsPrimary", DisplayName = "Primary", Type = GridColumnTypes.Bool, TemplateType = GridTemplateTypes.Favourite, MinWidth = 50, DisplayIndex = 2, IsSortingEnabled = false, IsFilteringEnabled = false}
            };
            var userContactGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.UserContactNumbers.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.UserContactNumbers,
                IsDefault = true
            };
            return userContactGridConfig;
        }

        private GridConfig CreateDefaultUserEmailsHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "Email", DisplayName = "Email", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0, IsSortingEnabled = false, IsFilteringEnabled = false},
                    new DataGridColumnObject { DataField = "IsPrimary", DisplayName = "Primary", Type = GridColumnTypes.Bool, TemplateType = GridTemplateTypes.Favourite, MinWidth = 50, DisplayIndex = 1, IsSortingEnabled = false, IsFilteringEnabled = false}
            };
            var userEmailGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.UserEmails.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.UserEmails,
                IsDefault = true
            };
            return userEmailGridConfig;
        }

        private GridConfig CreateDefaultAdvisorRepCodesHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "TypeValue", DisplayName = "Type", Type = GridColumnTypes.Enum, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0,},
                    new DataGridColumnObject { DataField = "Code", DisplayName = "Rep Code", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 1,},
            };
            var advisorRepCodeGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.RepCodeListing.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.RepCodeListing,
                IsDefault = true
            };
            return advisorRepCodeGridConfig;
        }

        private GridConfig CreateDefaultModelListingSidebarHeaders()
        {
            var dataGridColumnObject = new List<DataGridColumnObject>
                {
                    new DataGridColumnObject { DataField = "Name", DisplayName = "Model", Type = GridColumnTypes.String, TemplateType = GridTemplateTypes.String, MinWidth = 150, DisplayIndex = 0}
            };
            var modelSidebarListingGridConfig = new GridConfig()
            {
                GridName = DataRequestSource.ModelListingSidebar.ToString(),
                GridColumnJSonValue = JsonConvert.SerializeObject(dataGridColumnObject),
                GridType = DataRequestSource.ModelListingSidebar,
                IsDefault = true
            };
            return modelSidebarListingGridConfig;
        }

        public void DeleteLoginActivity()
        {
            if (_loginActivityDao.GetAll().Count > 0)
                _loginActivityDao.DeleteAllLoginActivity();
        }

        public void CreateBlendStrategy()
        {
            if (_strategyDao.GetBlendedStrategy() == null)
            {
                using (ITransaction transaction = _session.BeginTransaction())
                {
                    try
                    {
                        var blendStrategy = new Strategy();
                        blendStrategy.Name = "Blended Strategy";
                        blendStrategy.Description = "Blended Strategy";
                        blendStrategy.IsBlendedStrategy = true;
                        _strategyDao.Persist(blendStrategy);
                        transaction.Commit();
                        _logger.LogInformation("Blended Strategy Created !");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError("Strategy", ex.Message);
                    }
                }
            }
        }
    }
}


