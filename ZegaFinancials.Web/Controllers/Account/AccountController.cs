using Microsoft.AspNetCore.Mvc;
using NHibernate;
using ZegaFinancials.Services.Interfaces.Accounts;
using ZegaFinancials.Services.Models.Accounts;
using ZegaFinancials.Web.App_Start;
using ZegaFinancials.Nhibernate.Entities.Accounts;
using ZegaFinancials.Services.Impl.Support;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ZegaFinancials.Web.Controllers
{
    public class AccountController : ZegaController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService, ISession session, IMemoryCache memoryCache) : base(session, memoryCache)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public JsonResult GetAccountDropdowns()
        {
            var accountsDropdownModel = new AccountDropdownModel()
            {
                AccountStatus = Utility.ConvertEnumToDictionary<AccountStatus>(),
                AccountType = Utility.ConvertEnumToDictionary<AccountType>(),
                Frequency = Utility.ConvertEnumToDictionary<Frequency>(),
                Broker = Utility.ConvertEnumToDictionary<Broker>(),
                RepCodes = _accountService.GetAllRepCodes()
            };
            return Json(new { success = true, response = accountsDropdownModel });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetAccountInformation([FromBody] int accountId)
        {
            var account = _accountService.GetAccountById(accountId, UserContext);
            return Json(new { success = true, Response = account });
        }
        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveAccountBasicDetails(AccountInfoModelcs accountBasicDetails)
        {
            var result = _accountService.SaveAccountBasicInfo(accountBasicDetails, UserContext);
            return Json(new { success = true, message = "Account Basic Details Data Saved!", Response = result });
        }
        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveAccountModelDetails(AccountInfoModelcs accountBasicDetails)
        {
            _accountService.SaveAccountModelDetails(accountBasicDetails, UserContext);
            return Json(new { success = true, message = "Account Model Data Saved !" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveAccountAdditionalWithdrawals(AccountInfoModelcs accountAdditionalWithdrawals)
        {
            _accountService.SaveAccountAdditionalWithdrawals(accountAdditionalWithdrawals, UserContext);
            return Json(new { success = true, message = "Account Additional Withdrawals Data Saved!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveAccountAdditionalDeposits(AccountInfoModelcs accountAdditionalDeposits)
        {
            _accountService.SaveAccountAdditionalDeposits(accountAdditionalDeposits, UserContext);
            return Json(new { success = true, message = "Account Additional Deposits Data Saved!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult SaveAccountZegaCustomFields(AccountInfoModelcs accountZegaCustomFields)
        {
            _accountService.SaveAccountZegaCustomFields(accountZegaCustomFields, UserContext);
            return Json(new { success = true, message = "Account Zega Custom Fields Data Saved!" });
        }
        [HttpPost, ZegaExceptionFilter]
        public JsonResult DeleteAccounts(int[] AccountIds)
        {
            _accountService.DeleteAccountByIds(AccountIds, UserContext);
            return Json(new { success = true, message = "Selected Accounts deleted succesfully!" });
        }

        [HttpPost, ZegaExceptionFilter]
        public JsonResult GetAdvisor([FromBody] int repCodeId)
        {
            var repCodeDropDownList = _accountService.GetAdvisorsNameByRepCodeId(repCodeId,UserContext);
            return Json(new { success = true, Response = repCodeDropDownList });
        }

        [HttpGet, ZegaExceptionFilter, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult LoadAll()
        {
            var accounts = _accountService.LoadAllAccounts();
            return Json(new { success = true, response = accounts });
        }

        [HttpPost, ZegaExceptionFilter, Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult GetByFilterWithSleevePercent(string[] sleeveIds)
        {
            var result = _accountService.GetByFilterWithSleevePercent(sleeveIds);
            return Json(new { success = true, response = result });
        }
        [HttpPost, ZegaExceptionFilter,Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public JsonResult Get([FromBody]int id)
        {
            var result = _accountService.GetBrokerByAccountId(id);
            return Json(new { success = true, response = result });
        }
    }
}
