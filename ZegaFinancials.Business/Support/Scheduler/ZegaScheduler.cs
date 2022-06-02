using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZegaFinancials.Business.Interfaces.Admin;
using ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport;

namespace ZegaFinancials.Business.Support.Scheduler
{
    public class ZegaScheduler : IHostedService, IZegaScheduler
    {        
        private const string TriggerName = "Import trigger";
        private const string GroupName = "Import group";
        private const string JobName = "Import job";
        private Type JobType = typeof(ImportJob);
                
        private string CronExpression { get; set; }

        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IGlobalSettingsLogic _globalsettingLogic;
        private readonly IImportProfileDao _importProfileDao;
        private readonly ILogger<ZegaScheduler> _logger;
        public ZegaScheduler(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IServiceScopeFactory factory, ILogger<ZegaScheduler> logger)
        {
            _schedulerFactory = schedulerFactory;
            _jobFactory = jobFactory;
            _globalsettingLogic = factory.CreateScope().ServiceProvider.GetRequiredService<IGlobalSettingsLogic>();
            _importProfileDao = factory.CreateScope().ServiceProvider.GetRequiredService<IImportProfileDao>();
            _logger = logger;
        }
        public IScheduler Scheduler { get; set; }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var gs = _globalsettingLogic.Get();
            var importProfile = _importProfileDao.GetAll().FirstOrDefault();
            var autoImportEnable = importProfile != null ? importProfile.AutoImport : false;
            int hour = 0, minute = 0;
            if (gs != null)
            {
                hour = gs.SchedulerImportHour;
                minute = gs.SchedulerImportMinute;
            }

            CronExpression = $"0 {minute} {hour} ? * *";
            await ChangeImportTrigger(CronExpression, autoImportEnable, cancellationToken);
            await Scheduler.Start(cancellationToken);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler?.Shutdown(cancellationToken);
        }
        private ITrigger CreateTrigger()
        {
            return TriggerBuilder.Create()
            .WithIdentity(TriggerName, GroupName)
            .WithCronSchedule(CronExpression)
            .WithDescription(JobName)
            .Build();
        }
        private IJobDetail CreateJob()
        {
            return JobBuilder
            .Create(JobType)
            .WithIdentity(TriggerName, GroupName)
            .WithDescription(JobName)
            .Build();
        }

        public async Task ChangeImportTrigger(int hour, int minute, bool autoImportEnable)
        {
            CronExpression = $"0 {minute} {hour} ? * *";
            await ChangeImportTrigger(CronExpression, autoImportEnable, default);
        }

        private async Task ChangeImportTrigger(string cronExpressionString, bool autoImportEnable, CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler();
            Scheduler.JobFactory = _jobFactory;
            TriggerKey triggerKey = new TriggerKey(TriggerName, GroupName);
            if (!autoImportEnable)
            {
                if (await Scheduler.UnscheduleJob(triggerKey))
                {
                    _logger.LogInformation("Auto import is disabled.");
                }
                return;
            }

            var trigger = await Scheduler.GetTrigger(triggerKey) as ICronTrigger;
            
            var isNewTrigger = trigger == null;
            ITrigger newTrigger;
            if (isNewTrigger)
            {
                var importJob = CreateJob();
                newTrigger = CreateTrigger();

                await Scheduler.ScheduleJob(importJob, newTrigger, cancellationToken);
            }
            else
            {
                if (trigger.CronExpressionString != cronExpressionString)
                {
                    newTrigger = CreateTrigger();
                    await Scheduler.RescheduleJob(triggerKey, newTrigger, cancellationToken);
                }
            }

            if (isNewTrigger)
            {
                _logger.LogInformation("Auto import is enabled.");
            }           
        }

    }
}
