using ZegaFinancials.Business.Interfaces.DataGrid;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Interfaces.Support;
using Newtonsoft.Json;
using ZegaFinancials.Nhibernate.Support;
using System.Collections.Generic;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Business.Impl.DataGrid;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Business.Interfaces.Users;
using System.Linq;
using ZegaFinancials.Nhibernate.Entities.Import;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Business.Shared.Models;

namespace ZegaFinancials.Services.Impl.Support
{
    public class DataGridService : ZegaService, IDataGridService
    {
        private readonly IDataGridLogic _dataGridLogic;
        public DataGridService(IDataGridLogic dataGridLogic, IUserLogic userLogic): base(userLogic)
        {
            _dataGridLogic = dataGridLogic;
        }

        public byte[] ExportToCsv(IEnumerable<object> entityList, IEnumerable<DataGridColumnObject> columns, HashSet<string> unsupportedList, DataGridLogic.GetCustomFieldValue handler, UserContextModel userContext)
        {
            return _dataGridLogic.ExportToCsv(entityList, columns, unsupportedList, null);
        }

        public DataGridPreferenceModel GetDataGridHeader(DataRequestSource gridType, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var gridHeaders = _dataGridLogic.GetGridByPreferences(gridType, userContext.Id);
            
            GetEnumValuesByGridType(gridHeaders);
            return gridHeaders;
        }
        
        public void SaveDataGridPrefernces(DataRequestSource gridType, DataGridPreferenceModel gridPreferenceModel)
        {
            var user = _userLogic.GetUserByLogin(gridPreferenceModel.UserLogin);
            if (user != null)
            {
                var HeadersInfo = _dataGridLogic.GetGridHeaders(gridType, user.Id);
                if (HeadersInfo != null)
                {
                    gridPreferenceModel.DataGridColumnObjects.ForEach(o => { o.IsFilterAdded = false; o.FilterExpressions = null; o.EnumValues = null; });
                    HeadersInfo.GridColumnJSonValue = JsonConvert.SerializeObject(gridPreferenceModel.DataGridColumnObjects);
                    HeadersInfo.SortingJSonValue = JsonConvert.SerializeObject(gridPreferenceModel.SortDescriptions);

                    if (!HeadersInfo.IsDefault)
                        _dataGridLogic.Persist(HeadersInfo);
                    else {
                        var newHeaderInfoForUser = new GridConfig() {
                            GridName = gridType.ToString(),
                            GridType = gridType,
                            GridColumnJSonValue = HeadersInfo.GridColumnJSonValue,
                            SortingJSonValue = HeadersInfo.SortingJSonValue,
                            User = new User { Id = user.Id }
                        };

                        _dataGridLogic.Persist(newHeaderInfoForUser);
                    }
                }
            }
        }
        private void GetEnumValuesByGridType(DataGridPreferenceModel gridPreferenceModel)
        {
            var dataGridColumn = gridPreferenceModel.DataGridColumnObjects?.Where(o => o.Type == GridColumnTypes.Enum);
            if(dataGridColumn != null)
            {
                foreach (var column in dataGridColumn)
                {
                    switch (column.DataField)
                    {
                        case "StatusValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<Status>();
                            break;
                        case "EntityTypeValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<EntityType>();
                            break;
                        case "TypeValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<ImportType>();
                            break;
                        case "ImportStatusValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<ImportStatus>();
                            break;
                        case "accountBasicDetails.AccountStatusValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<AccountStatus>();
                            break;
                        case "accountBasicDetails.AccountTypeValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<AccountType>();
                            break;
                        case "accounWithdrawlInfoModelcs.Withdrawl_StatusValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<WithdrawalStatus>();
                            break;
                        case "accountDepositsInfoModelcs.Deposit_StatusValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<WithdrawlorDepositStatus>();
                            break;
                        case "accountZegaCustomFieldsModel.Zega_ConfirmedValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<WithdrawlorDepositStatus>();
                            break;
                        case "accounWithdrawlInfoModelcs.Future_Withdrawal_StatusValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<WithdrawlorDepositStatus>();
                            break;
                        case "accounWithdrawlInfoModelcs.One_Time_WithdrawalValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<WithdrawlorDepositStatus>();
                            break;
                        case "accountDepositsInfoModelcs.Deposit_FrequencyValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<Frequency>();
                            break;
                        case "accounWithdrawlInfoModelcs.Withdrawal_FrequencyValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<Frequency>();
                            break;
                        case "accountBasicDetails.BrokerValue":
                            column.EnumValues = Utility.ConvertEnumToDictionary<Broker>();
                            break;
                    }
                }
            }
        }

    }
}
