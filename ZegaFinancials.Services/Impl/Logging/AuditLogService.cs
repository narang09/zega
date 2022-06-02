using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Business.Interfaces.Logging;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Interfaces.Logging;
using ZegaFinancials.Services.Models.Logging;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Impl.Logging
{
    public class AuditLogService : ZegaService, IAuditLogService
    {
        private readonly IAuditLogLogic _auditLogLogic;
        public AuditLogService(IAuditLogLogic auditLogLogic, IUserLogic userLogic) : base(userLogic)
        {
            _auditLogLogic = auditLogLogic;
        }
        public DataGridModel LoadAuditLogByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext)
        {
            int count;
            CheckUserContext(userContext);
            var logs = _auditLogLogic.GetAuditLogByFilter(dataGridFilterModel, userContext.Login, userContext.IsAdmin, out count).Select(o => new AuditLogModel
            {
                Id = o.Id,
                Message = o.Message,
                EntityId = o.EntityId,
                EntityType = o.EntityType,
                Name = o.UserLogin,
                Date = o.Date
            });
            var dataGridModel = new DataGridModel();
            dataGridModel.AuditLogs = logs.ToArray();
            dataGridModel.TotalRecords = count;

            return dataGridModel;
        }

        public LogModel[] FilterNSortClassesTime(DataGridFilterModel filter)
        {
            int count;

            var logs = _auditLogLogic.GetAuditLogByFilter(filter, null, true, out count).Select(o => new LogModel
            {
                Type = o.EntityType,
                Message = o.Message,
                Date = o.Date,
                User = new CommonModel { Name = o.UserLogin }
            }); 

            if (!logs.Any())
                return new LogModel[0];
            return convertMessage(logs).ToArray();
        }

        private IEnumerable<LogModel> convertMessage(IEnumerable<LogModel> auditLogs)
        {
            List<LogModel> tempLogs = new();
            // for replace the fields used in Zega to the desired one in Trade list
            Dictionary<string, string> valueMap = new Dictionary<string, string>
            {
                { "RepCode", "Manager"},
                { "Number","ShortName"},
                { "Name","ModelIdAndSleeveId"},
                { "Description", "Name"},
                { "AccountType", "Acct_Type"},
                { "Z3_ZEGA_Alert_Date", "Z3_ZEGA_Alert Date"},
                { "Broker","DefaultPortfolioBrokerAccount"},
                { "[Empty]", ""}
            };


            foreach (var auditLog in auditLogs)
            {
                var oldValues = valueMap.Select(x => x.Key).Where(y => auditLog.Message.Contains(y)).ToList();
                foreach (var oldValue in oldValues)
                {                   
                    string newValue;
                    if (oldValue == "Name")                   
                        newValue = auditLog.Type == EntityType.Model ? "ModelId" : auditLog.Type == EntityType.Sleeve ? "SleeveId" : null;
                    else if (oldValue == "[Empty]" && auditLog.Type == EntityType.Account && auditLog.Message.Contains("Model changed")) // We have to keep [Empty] for the model changed audit log
                        newValue = null;
                    else
                        newValue = valueMap[oldValue];

                    if (newValue != null)
                        auditLog.Message = auditLog.Message.Replace(oldValue, newValue);
                }

                if (auditLog.Type == EntityType.Sleeve)
                    auditLog.Type = EntityType.ModelSleeve;

                tempLogs.Add(auditLog);

                IList<string> customFields = new List<string> { "AccountValue", "Acct_Type", "B2_Allocation_Start_Date", "C1_Withdrawal_Amount", "C2_Withdrawal_Date", "C3_Withdrawal_Status", "CashEq", "CashNetBal", "D1_Deposit", "D2_Deposit_Date", "D3_Deposit_Status", "OBuyingPower", "SBuyingPower", "VEOImportDate", "Z2_ZEGA_Confirmed", "Z3_ZEGA_Alert Date", "Z4_ZEGA_Notes" };

                if (auditLog.Type == EntityType.Account && auditLog.Message.Contains("AccountDetails") && !auditLog.Message.Contains("New AccountDetails"))
                {
                    var splittedLogs = auditLog.Message.Split('(', ',', ')');
                    string[] nextSplittedLogs;
                    string portfolio = null, customFieldName = null, previousValue = "", currentValue = "";
                    for (int index = 0; index < splittedLogs.Length - 1; index++)
                    {
                        if (index == 0)
                        {
                            nextSplittedLogs = splittedLogs[index].Split('"', '=');
                            portfolio = nextSplittedLogs[2].TrimStart();
                        }
                        else
                        {
                            nextSplittedLogs = splittedLogs[index].Split('"');
                            customFieldName = customFields.Select(x => x).Where(x => nextSplittedLogs[0].Contains(x))?.FirstOrDefault();
                            previousValue = nextSplittedLogs[1];
                            currentValue = nextSplittedLogs[3];
                        }
                        if (portfolio != null && customFieldName != null)
                        {
                            var msg = string.Format("The custom field \"{0}\"  for Portfolio \"{1}\" updated from \"{2}\" to \"{3}\".", customFieldName, portfolio, previousValue, currentValue);
                            LogModel logModel = new()
                            {
                                Type = EntityType.CustomField,
                                Message = msg,
                                Date = auditLog.Date,
                                User = new CommonModel { Name = auditLog.User.Name }
                            };
                            tempLogs.Add(logModel);
                        }
                    }
                }
            }
            return tempLogs;
        }

    }
}

