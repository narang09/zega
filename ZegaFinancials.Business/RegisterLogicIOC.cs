using Microsoft.Extensions.DependencyInjection;
using ZegaFinancials.Business.Impl;
using ZegaFinancials.Business.Impl.Accounts;
using ZegaFinancials.Business.Impl.DataGrid;
using ZegaFinancials.Business.Impl.Logging;
using ZegaFinancials.Business.Impl.Models;
using ZegaFinancials.Business.Impl.Strategies;
using ZegaFinancials.Business.Impl.SpecificationsImport;
using ZegaFinancials.Business.Impl.Users;
using ZegaFinancials.Business.Interfaces;
using ZegaFinancials.Business.Interfaces.Accounts;
using ZegaFinancials.Business.Interfaces.DataGrid;
using ZegaFinancials.Business.Interfaces.Logging;
using ZegaFinancials.Business.Interfaces.Models;
using ZegaFinancials.Business.Interfaces.Strategies;
using ZegaFinancials.Business.Interfaces.SpecificationsImport;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Business.Support.SpecificationsImport.Parsers;
using ZegaFinancials.Business.Support.TDVeo;
using Quartz.Spi;
using ZegaFinancials.Business.Support.Scheduler;
using Quartz;
using Quartz.Impl;
using ZegaFinancials.Business.Interfaces.Admin;
using ZegaFinancials.Business.Impl.Admin;

namespace ZegaFinancials.Business
{
    public static class RegisterLogicIOC
    {
        public static IServiceCollection RegisterLogicDI(this IServiceCollection service)
        {
            service.AddScoped<IUserLogic, UserLogic>();
            service.AddScoped<IAccountLogic, AccountLogic>();
            service.AddScoped<IModelLogic, ModelLogic>();
            service.AddScoped<ISleeveLogic, SleeveLogic>();
            service.AddScoped<IInitialDataLogic, InitialDataLogic>();
            service.AddScoped<IDataGridLogic, DataGridLogic>();
            service.AddScoped<IAuditLogLogic, AuditLogLogic>();
            service.AddScoped<IStrategyLogic, StrategyLogic>();
            service.AddScoped<ISpecificationsImportLogic, SpecificationsImportLogic>();
            service.AddScoped<IImportPersister, ImportPersister>();
            service.AddScoped<ITDVeoXmlImportParser, TDVeoXmlImportParser>();
            service.AddScoped<ITDVeoImportProvider, TDVeoImportProvider>();
            service.AddScoped<ITdVeoImportWebService, TdVeoImportWebService>();
            service.AddScoped<ITDVeoImportService, TDVeoImportService>();
            service.AddSingleton<IJobFactory, CustomQuartzJobFactory>();
            service.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            service.AddSingleton<ImportJob>();
            service.AddSingleton<IZegaScheduler, ZegaScheduler>();
            service.AddHostedService<ZegaScheduler>();
            service.AddScoped<IGlobalSettingsLogic, GlobalSettingsLogic>();
            return service;
        }
    }
}
