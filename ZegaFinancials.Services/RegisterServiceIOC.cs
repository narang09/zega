using Microsoft.Extensions.DependencyInjection;
using ZegaFinancials.Services.Impl.Accounts;
using ZegaFinancials.Services.Impl.Dashboard;
using ZegaFinancials.Services.Impl.Logging;
using ZegaFinancials.Services.Impl.Models;
using ZegaFinancials.Services.Impl.Strategies;
using ZegaFinancials.Services.Impl.SpecifcationsImport;
using ZegaFinancials.Services.Impl.Support;
using ZegaFinancials.Services.Impl.Users;
using ZegaFinancials.Services.Interfaces.Accounts;
using ZegaFinancials.Services.Interfaces.Dashboard;
using ZegaFinancials.Services.Interfaces.Logging;
using ZegaFinancials.Services.Interfaces.Models;
using ZegaFinancials.Services.Interfaces.Strategies;
using ZegaFinancials.Services.Interfaces.SpecificationsImport;
using ZegaFinancials.Services.Interfaces.Support;
using ZegaFinancials.Services.Interfaces.Users;
using FluentValidation;
using ZegaFinancials.Services.Models.Models;
using ZegaFinancials.Services.Models.Accounts;
using ZegaFinancials.Services.Models.Strategies;
using ZegaFinancials.Services.Models.Users;
using ZegaFinancials.Services.Interfaces.CustomFields;
using ZegaFinancials.Services.Impl.CustomFields;

namespace ZegaFinancials.Services
{
    public static class RegisterServiceIOC
    {
        public static IServiceCollection RegisterServicesDI(this IServiceCollection service)
        {
            service.AddScoped<IUserService, UserService>();
            service.AddScoped<IAccountService, AccountService>();
            service.AddScoped<IModelService, ModelService>();
            service.AddScoped<ISleeveService, SleeveService>();
            service.AddScoped<IDataGridService, DataGridService>();
            service.AddScoped<IStrategyService, StrategyService>();
            service.AddScoped<IAuditLogService, AuditLogService>();
            service.AddScoped<ISpecificationsImportService, SpecificationsImportService>();
            service.AddScoped<IDashboardService, DashboardService>();
            service.AddScoped<ICustomFieldService, CustomFieldService>();
            service.AddTransient<IValidator<SleeveModel>, SleeveModelValidators>();
            service.AddTransient<IValidator<AccountBasisDetailsModel>, AccountBasicDetailsModel_Validators>();
            service.AddTransient<IValidator<AccounWithdrawlInfoModelcs>, AccountWithdrawalInfoModel_Validators>();
            service.AddTransient<IValidator<ModelModel>, ModelModelValidators>();
            service.AddTransient<IValidator<StrategyModels>, StrategyModelsValidator>();
            service.AddTransient<IValidator<AdvisorRepCodeModel>, AdvisorRepCodeModelValidators>();
            service.AddTransient<IValidator<UserDetailsModel>, UserDetailsModelValidators>();
            service.AddTransient<IValidator<UserEntityModel>, UserEntityModelValidators>();
            return service;
        }
    }
}
