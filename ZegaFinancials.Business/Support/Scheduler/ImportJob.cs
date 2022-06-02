using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Threading.Tasks;
using ZegaFinancials.Business.Interfaces.SpecificationsImport;

namespace ZegaFinancials.Business.Support.Scheduler
{
    /// <summary>
    /// Scheduler import job
    /// </summary>
    /// <remarks></remarks>
    public class ImportJob : IJob
    {
        private readonly ISpecificationsImportLogic _specificationImportLogic;
        private readonly ILogger<ImportJob> _logger;
        public ImportJob(IServiceScopeFactory factory, ILogger<ImportJob> logger)
        {           
           _specificationImportLogic = factory.CreateScope().ServiceProvider.GetRequiredService<ISpecificationsImportLogic>();
            _logger = logger;
        }
        public Task Execute(IJobExecutionContext context)        
        {            
            try
            {
                _logger.LogInformation("Auto Import Job started.");
                _specificationImportLogic.ImportAll();
                _logger.LogInformation("Auto Import Job finished.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Auto Import Job");
            }
            return Task.CompletedTask;
        }
    }

}
