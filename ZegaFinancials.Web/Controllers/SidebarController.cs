using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Entities.Users;
using ZegaFinancials.Services.Impl.Support;
using ZegaFinancials.Services.Interfaces.Accounts;
using ZegaFinancials.Services.Interfaces.Models;
using ZegaFinancials.Services.Interfaces.Strategies;
using ZegaFinancials.Services.Interfaces.Users;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Services.Models.Support;
using ZegaFinancials.Web.App_Start;

namespace ZegaFinancials.Web.Controllers
{
    public class SidebarController : ZegaController
    {
        private readonly IAccountService _accountService;
        private readonly IModelService _modelService;
        private readonly IStrategyService _strategyService;
        private readonly IUserService _userService;

        public SidebarController(IUserService userService, IAccountService accountService, IModelService modelService, IStrategyService strategyService, ISession session, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _modelService = modelService;
            _accountService = accountService;
            _strategyService = strategyService;
            _userService = userService;
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetBulkEditDropdowns( BulkEditModel bulkEditRequestModel)
        {
            var sidebarDropdowns = new DropdownModel();
            if (Enum.TryParse(bulkEditRequestModel.GridName, out DataRequestSource requestSource))
            {
                switch (requestSource)
                {
                    case DataRequestSource.AccountListing:
                        GetAccountListingDropdowns(sidebarDropdowns, bulkEditRequestModel.DataStoreIds);
                        break;
                    case DataRequestSource.AdvisorListing:
                        GetAdvisorListingDropdowns(sidebarDropdowns);
                        break;
                    case DataRequestSource.ModelListing:
                        GetModelListingDropdown(sidebarDropdowns);
                        break;
                    default:
                        throw new Exception("DataRequestSource does not found.");

                }
            }
            return Json(new { success = true, response = sidebarDropdowns });
        }

        private void GetAccountListingDropdowns(DropdownModel sidebarDropdowns, List<int> accountIds)
        {
            sidebarDropdowns.AccountStatus = Utility.ConvertEnumToDictionary<AccountStatus>();
            sidebarDropdowns.AccountType = Utility.ConvertEnumToDictionary<AccountType>();
            sidebarDropdowns.Models = _modelService.GetModelsByAccountIds(UserContext, accountIds.ToArray());
        }

        private void GetAdvisorListingDropdowns(DropdownModel sidebarDropdowns)
        {
            sidebarDropdowns.Status = Utility.ConvertEnumToDictionary<Status>();
        }

        private void GetModelListingDropdown(DropdownModel sidebarDropdowns)
        {
            sidebarDropdowns.Strategies = _strategyService.GetAllStrategies(UserContext);
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveBulkEditInformation(BulkEditModel bulkChanges)
        {
            if (Enum.TryParse(bulkChanges.GridName, out DataRequestSource requestSource))
            {
                switch (requestSource)
                {
                    case DataRequestSource.AccountListing:
                        _accountService.BulkEditAccounts(bulkChanges, UserContext);
                        break;
                    case DataRequestSource.AdvisorListing:
                        _userService.BulkEditUsers(bulkChanges, UserContext);
                        break;
                    case DataRequestSource.ModelListing:
                        _modelService.BulkEditModels(bulkChanges, UserContext);
                        break;
                    default:
                        throw new System.ServiceModel.FaultException("Unable to save details.");
                }
            }
            return Json(new { success = true, message = "Data Updated!" });
        }

    }
}
