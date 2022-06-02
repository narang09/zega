using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Business.Interfaces.Accounts;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Services.Interfaces.Dashboard;
using ZegaFinancials.Services.Models.Dashboard;
using ZegaFinancials.Services.Models.Shared;

namespace ZegaFinancials.Services.Impl.Dashboard
{
    public class DashboardService : ZegaService, IDashboardService
    {

        private readonly IAccountLogic _accountLogic;

        public DashboardService(IUserLogic userLogic, IAccountLogic accountLogic): base(userLogic)
        {
            _accountLogic = accountLogic;
        }


        public DataGridModel GetAdvisorsById(UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var advisors = _userLogic.GetAllAdvisors();
            if (advisors == null)
                return new();
            var dataGridModel = new DataGridModel();
            var advisorsDashboardModelList = new List<DashboardAdminModel>();

            foreach (var advisor in advisors)
            {
                var advisorDashboardModel = new DashboardAdminModel
                {

                    Name = advisor.Details != null ? advisor.Details.FirstName + advisor.Details.LastName : null,
                    ModelsCount = advisor.Models != null ? advisor.Models.Count : 0,
                    AccountsCount = GetAttachedAccountsCount(advisor.RepCodes.Select(o => o.RepCode.Id).ToArray()),
                    Status = advisor.Status
                   // ClientsCount = GetAttachedClientsCount(advisor.RepCodes.Select(o => o.RepCode.Id).ToArray()) // Removed As per Client Request
                };
                advisorsDashboardModelList.Add(advisorDashboardModel);

            }
            dataGridModel.AdvisorsList = advisorsDashboardModelList.ToArray();
            return dataGridModel;
        }
  
    public long GetAttachedAccountsCount(int[] repCodeIds)
        {
            
            if (repCodeIds != null)
                return  _accountLogic.GetAccountsCountByRepCode(repCodeIds);
            return 0;

        }
        public long GetAttachedClientsCount(int[] repCodeIds)
        {
            if (repCodeIds != null)
                return _accountLogic.GetClientsCountByRepCode(repCodeIds);
            return 0;

        }


    }
}
