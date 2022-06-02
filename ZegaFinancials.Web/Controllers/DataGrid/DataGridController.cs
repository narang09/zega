using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using System;
using System.Collections.Generic;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Services.Interfaces.Accounts;
using ZegaFinancials.Services.Interfaces.Logging;
using ZegaFinancials.Services.Interfaces.Models;
using ZegaFinancials.Services.Interfaces.Strategies;
using ZegaFinancials.Services.Interfaces.Support;
using ZegaFinancials.Services.Interfaces.Users;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Web.App_Start;
using ZegaFinancials.Web.Models.DataGrid;
using ZegaFinancials.Services.Interfaces.Dashboard;
using ZegaFinancials.Nhibernate.Support;
using System.Linq;
using ZegaFinancials.Services.Interfaces.SpecificationsImport;
using Microsoft.Extensions.Caching.Memory;
using ZegaFinancials.Business.Shared.Models;

namespace ZegaFinancials.Web.Controllers
{
    public class DataGridController : ZegaController
    {
        private readonly IAccountService _accountService;
        private readonly IModelService _modelService;
        private readonly ISleeveService _sleeveService;
        private readonly IDataGridService _dataGridService;
        private readonly IAuditLogService _auditLogService;
        private readonly IStrategyService _strategyService;
        private readonly IDashboardService _dashboardService;
        private readonly ISpecificationsImportService _specificationsImportService;
        private readonly IUserService _userService;
        public DataGridController(IAuditLogService auditLogService, IStrategyService strategyService, IAccountService accountService, IModelService modelService, IUserService userService, ISleeveService sleeveService, IDataGridService dataGridService, ISession session, IDashboardService dashboardService, ISpecificationsImportService specificationsImportService, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _accountService = accountService;
            _modelService = modelService;
            _sleeveService = sleeveService;
            _dataGridService = dataGridService;
            _auditLogService = auditLogService;
            _strategyService = strategyService;
            _dashboardService = dashboardService;
            _specificationsImportService = specificationsImportService;
            _userService = userService;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetGridHeaders([FromBody] string dataGrid)
        {
            GridHeaderModel gridModel;
            if (Enum.TryParse(dataGrid, out DataRequestSource gridType))
            {
                var result = _dataGridService.GetDataGridHeader(gridType, UserContext);
                gridModel = new GridHeaderModel()
                {
                    GridHeaders = result?.DataGridColumnObjects,
                    SortDescriptions = result?.SortDescriptions,
                };
                return Json(new { success = true, response = gridModel });
            }
            else
            {

                return Json(new { success = false, messge = "No Grid Type Found with Enum Value : " + dataGrid });
            }
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetDataGridData(DataGridFilterModel dataGridFilterModel)
        {
            if (Enum.TryParse(dataGridFilterModel.GridName, out DataRequestSource grid))
            {
                dataGridFilterModel.RequestSource = grid;
                var gridData = new DataGridModel();
                switch (dataGridFilterModel.RequestSource)
                {
                    case DataRequestSource.AccountListing:
                    case DataRequestSource.DashboardAdvisor:
                        gridData = _accountService.LoadAccountsByFilter(dataGridFilterModel, UserContext);
                        break;
                    case DataRequestSource.ModelListing:
                    case DataRequestSource.ModelListingSidebar:
                        gridData = _modelService.LoadModelsByFilter(dataGridFilterModel, UserContext);
                        break;
                    case DataRequestSource.AdvisorListing:
                        gridData = _userService.LoadUsersByFilter(dataGridFilterModel, UserContext);
                        break;
                    case DataRequestSource.RepCodeListing:
                        gridData = _userService.LoadRepCodesByFilter(dataGridFilterModel, UserContext);
                        break;
                    case DataRequestSource.SleeveListing:
                        gridData = _sleeveService.LoadSleevesByFilter(dataGridFilterModel, UserContext);
                        break;
                    case DataRequestSource.AuditLogListing:
                        gridData = _auditLogService.LoadAuditLogByFilter(dataGridFilterModel, UserContext);
                        break;
                    case DataRequestSource.StrategyListing:
                        gridData = _strategyService.LoadStrategiesByFilter(dataGridFilterModel, UserContext);
                        break;
                    case DataRequestSource.DashboardAdmin:
                        gridData = _dashboardService.GetAdvisorsById(UserContext);
                        break;
                    case DataRequestSource.ImportHistory:
                        gridData = _specificationsImportService.LoadImportHistoryByFilter(dataGridFilterModel, UserContext);
                        break;
                }
                return Json(new { success = true, response = gridData });
            }
            else
                return Json(new { success = false, message = "No such Grid!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public ActionResult ExportGridToCsv(DataGridFilterModel dataGridFilterModel)
        {
            IEnumerable<object> entityList = new List<object>();
            var unsupportedList = new HashSet<string>();
            var gridHeaders = new DataGridPreferenceModel();
            DataRequestSource gridType;
            if (Enum.TryParse(dataGridFilterModel.GridName, out gridType))
            {
                dataGridFilterModel.RequestSource = gridType;
                switch (gridType)
                {
                    case DataRequestSource.ModelListing:
                        var models = _modelService.LoadModelsByFilter(dataGridFilterModel, UserContext).Models;
                        entityList = _modelService.GetModelListForExport(models);
                        gridHeaders.DataGridColumnObjects = dataGridFilterModel.DataGridColumnObjects;
                        if (gridHeaders.DataGridColumnObjects != null && gridHeaders.DataGridColumnObjects.Any(o => o.IsVisible))
                        {
                            var allocationColumn = new DataGridColumnObject() { 
                                DisplayIndex = gridHeaders.DataGridColumnObjects.Count,
                                DataField = "AllocationUI",
                                DisplayName = "Allocation (%)"
                            };
                            gridHeaders.DataGridColumnObjects.Add(allocationColumn);
                        }
                        break;
                    case DataRequestSource.AdvisorListing:
                        entityList = _userService.LoadUsersByFilter(dataGridFilterModel, UserContext).Advisors;
                        gridHeaders.DataGridColumnObjects = dataGridFilterModel.DataGridColumnObjects; ;
                        break;
                    case DataRequestSource.AccountListing:
                        entityList = _accountService.LoadAccountsByFilter(dataGridFilterModel, UserContext).Accounts;
                        gridHeaders.DataGridColumnObjects = dataGridFilterModel.DataGridColumnObjects;
                        break;
                    case DataRequestSource.StrategyListing:
                        entityList = _strategyService.LoadStrategiesByFilter(dataGridFilterModel, UserContext).Strategies;
                        gridHeaders.DataGridColumnObjects = dataGridFilterModel.DataGridColumnObjects;
                        break;
                    case DataRequestSource.SleeveListing:
                        entityList = _sleeveService.LoadSleevesByFilter(dataGridFilterModel, UserContext).Sleeves;
                        gridHeaders.DataGridColumnObjects = dataGridFilterModel.DataGridColumnObjects;
                        break;
                    case DataRequestSource.AuditLogListing:
                        entityList = _auditLogService.LoadAuditLogByFilter(dataGridFilterModel, UserContext).AuditLogs;
                        gridHeaders.DataGridColumnObjects = dataGridFilterModel.DataGridColumnObjects;
                        break;
                }
            }
            if (gridHeaders.DataGridColumnObjects == null)
                gridHeaders.DataGridColumnObjects = new List<DataGridColumnObject>();
            gridHeaders.DataGridColumnObjects = gridHeaders.DataGridColumnObjects.OrderBy(o => o.DisplayIndex).ToList();
            var result = _dataGridService.ExportToCsv(entityList, gridHeaders.DataGridColumnObjects, unsupportedList, null, UserContext);
            return File(result, "text/csv", "Export_" + dataGridFilterModel.GridName + ".csv");
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveGridPreferences(DataGridPreferenceModel gridPreferenceModel)
        {
            DataRequestSource gridType;
            
                if (Enum.TryParse(gridPreferenceModel.GridName, out gridType))
                {
                    _dataGridService.SaveDataGridPrefernces(gridType, gridPreferenceModel);
                    return Json(new { success = true, response = "Saved Preferences" });
                }
                else
                    throw new Exception("No Grid Type Found with Enum Value : " + gridPreferenceModel.GridName);           

        }
    }
}
