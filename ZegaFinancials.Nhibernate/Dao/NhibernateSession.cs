using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;
using ZegaFinancials.Nhibernate.Dao.Impl;
using ZegaFinancials.Nhibernate.Dao.Impl.Accounts;
using ZegaFinancials.Nhibernate.Dao.Impl.Advisors;
using ZegaFinancials.Nhibernate.Dao.Impl.DataGrid;
using ZegaFinancials.Nhibernate.Dao.Impl.Logging;
using ZegaFinancials.Nhibernate.Dao.Impl.Models;
using ZegaFinancials.Nhibernate.Dao.Impl.Strategies;
using ZegaFinancials.Nhibernate.Dao.Impl.SpecificationsImport;
using ZegaFinancials.Nhibernate.Dao.Impl.Users;
using ZegaFinancials.Nhibernate.Dao.Interface;
using ZegaFinancials.Nhibernate.Dao.Interface.Accounts;
using ZegaFinancials.Nhibernate.Dao.Interface.Advisors;
using ZegaFinancials.Nhibernate.Dao.Interface.DataGrid;
using ZegaFinancials.Nhibernate.Dao.Interface.Logging;
using ZegaFinancials.Nhibernate.Dao.Interface.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.Strategies;
using ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Dao.Interface.Admin;
using ZegaFinancials.Nhibernate.Dao.Impl.Admin;
using ZegaFinancials.Nhibernate.Support.EventListeners;
using NHibernate.Event;
using NHibernate.Cfg;

namespace ZegaFinancials.Nhibernate.Dao
{
    public static class NhibernateSession
    {
        public static IServiceCollection AddNhibernate(this IServiceCollection service, string connectionString)
        {
            var auditEventListener = new AuditLogEventListener();
            var sessionFactory = Fluently.Configure()
               .Database(MsSqlConfiguration.MsSql2012.Dialect<MsSql2012DialectExtension>().ConnectionString(connectionString).ShowSql)
               .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.Load("ZegaFinancials.Nhibernate"))).
                ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true)).
                ExposeConfiguration(cfg => cfg.EventListeners.PreInsertEventListeners =
                new IPreInsertEventListener[] { new InsertEventListener(auditEventListener) }).
                ExposeConfiguration(cfg => cfg.EventListeners.PostInsertEventListeners =
                new IPostInsertEventListener[] { new InsertEventListener(auditEventListener) }).
                ExposeConfiguration(cfg => cfg.EventListeners.PreUpdateEventListeners =
                new IPreUpdateEventListener[] { new UpdateEventListener(auditEventListener) }).
                ExposeConfiguration(cfg => cfg.EventListeners.PostUpdateEventListeners =
                new IPostUpdateEventListener[] { new UpdateEventListener(auditEventListener) }).
                ExposeConfiguration(cfg => cfg.EventListeners.PostDeleteEventListeners =
                new IPostDeleteEventListener[] { new DeleteEventListener(auditEventListener) }).
                BuildSessionFactory();
            service.AddSingleton(sessionFactory);
            service.AddScoped(factory => sessionFactory.OpenSession());
            return service;
        }
        public static IServiceCollection RegisterDaoDI(this IServiceCollection service)
        {            
            service.AddScoped(typeof(INHibernateDao<>), typeof(NHibernateDao<>));
            service.AddScoped<IUserDao, UserDao>();
            service.AddScoped<IAccountDao, AccountDao>();
            service.AddScoped<IModelDao, ModelDao>();
            service.AddScoped<IDataGridDao, DataGridDao>();
            service.AddScoped<ISleeveDao, SleeveDao>();
            service.AddScoped<IRepCodeDao, RepCodeDao>();
            service.AddScoped<IAdvisorRepCodeDao,AdvisorRepCodeDao>();
            service.AddScoped<IAuditLogDao, AuditLogDao>();
            service.AddScoped<IImportProfileDao, ImportProfileDao>();
            service.AddScoped<ITDVeoSettingsDao, TDVeoSettingsDao>();
            service.AddScoped<IAdvisorEmailDao,AdvisorEmailDao>();
            service.AddScoped<IAdvisorPhoneDao, AdvisorPhoneDao>();
            service.AddScoped<IAdvisorModelDao, AdvisorModelDao>();
            service.AddScoped<IGlobalSettingsDao, GlobalSettingsDao>();
            service.AddScoped<IStrategyDao, StrategyDao>();
            service.AddScoped<IImportHistoryDao, ImportHistoryDao>();
            service.AddScoped<ILoginActivityDao, LoginActivityDao>();
        return service;
        }
    }
}
