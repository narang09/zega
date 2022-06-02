using System.Linq;
using System.Collections.Generic;
using ZegaFinancials.Business.Interfaces.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport.Model;
using ZegaFinancials.Services.Interfaces.SpecificationsImport;
using ZegaFinancials.Services.Models;
using ZegaFinancials.Services.Models.Shared;
using ZegaFinancials.Nhibernate.Entities.Import;
using ZegaFinancials.Services.Models.Import;
using ZegaFinancials.Business.Interfaces.Admin;
using ZegaFinancials.Business.Support.Extensions;
using Newtonsoft.Json;
using ZegaFinancials.Nhibernate.Entities.Import.ImportSettings;
using ZegaFinancials.Business.Support.Scheduler;
using ZegaFinancials.Business.Interfaces.Users;
using ZegaFinancials.Nhibernate.Entities.Shared;
using System.IO;

namespace ZegaFinancials.Services.Impl.SpecifcationsImport
{
    public class SpecificationsImportService : ZegaService, ISpecificationsImportService
    {
        private readonly ISpecificationsImportLogic _specificationsImportLogic;
        private readonly IGlobalSettingsLogic _globalSettingsLogic;
        private readonly IZegaScheduler _zegaScheduler;
        public SpecificationsImportService(ISpecificationsImportLogic specificationsImportLogic, IGlobalSettingsLogic globalSettingsLogic, IZegaScheduler zegaScheduler, IUserLogic userLogic) : base(userLogic)
        {
            _specificationsImportLogic = specificationsImportLogic;
            _globalSettingsLogic = globalSettingsLogic;
            _zegaScheduler = zegaScheduler;
        }
        /// <summary>
        /// Imports the specified import profile.
        /// </summary>
        /// <param name="profileId">The profile id.</param>
        /// <param name="userContext">The user context.</param>
        /// <returns>Import result model</returns>
        /// <remarks></remarks>       
        public ImportResult Import(int profileId, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var importResult = _specificationsImportLogic.ImportProfile(profileId);

            if (importResult.ErrorMsg == null)
                importResult.Message = "The 'Accounts' have been imported successfully.";
            else
                importResult.Message = "There are errors during import 'Accounts'.";
            return importResult;
        }

        public IEnumerable<ZegaModel> GetRepCodesList(UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var repCodes = _specificationsImportLogic.GetAllRepCodesList();
            return repCodes != null && repCodes.Any() ? repCodes.Select(o => new ZegaModel() { Id = o.Id, Name = o.Code }).OrderBy(o =>o.Name) : new List<ZegaModel>();
        }

        public ImportProfileModel GetImportProfile(UserContextModel userContext)
        {
            CheckUserContext(userContext);
            var TDVeoProfile = _specificationsImportLogic.GetTDVeoProfile();
            var gs = _globalSettingsLogic.Get();
            ImportProfileModel importProfileModel = new();
            if (TDVeoProfile != null && TDVeoProfile.Profile != null)
            {
                importProfileModel = new()
                {
                    Id = TDVeoProfile.Profile.Id,
                    Name = TDVeoProfile.Profile.Name,
                    RepCodes = TDVeoProfile.RepCodeIds != null ? JsonConvert.DeserializeObject<List<int>>(TDVeoProfile.RepCodeIds) : new List<int>(),
                    Batches = TDVeoProfile.Batches != null ? JsonConvert.DeserializeObject<IList<List<int>>>(TDVeoProfile.Batches) : new List<List<int>>(),
                    Login = TDVeoProfile.UserId,
                    AutoImport = TDVeoProfile.Profile.AutoImport,
                    SchedulerImportTime = gs != null ? DateConverter.ConvertToStringTime(gs.SchedulerImportHour, gs.SchedulerImportMinute) : null
                };
            }
            return importProfileModel;
        }

        public int SaveImportProfile(ImportProfileModel profileModel, UserContextModel userContext)
        {
            CheckUserContext(userContext);
            TDAmertideSettings tDVeoSettings;
            if (profileModel.AutoImport &&( profileModel.Batches == null || !profileModel.Batches.Any()))
                  throw new ZegaServiceException("Batch Can't be Null Or Empty !");
            if (profileModel.AutoImport && profileModel.Batches != null && profileModel.Batches.Count > 1)
            {
                var repeatedRepcodeIds = profileModel.Batches.SelectMany(o => o).GroupBy(o => o).Where(o => o.Count() > 1).Select(o => o.Key);
                var repcodes = _userLogic.GetRepCodeByIds(repeatedRepcodeIds);
                if (repcodes.Any())
                    throw new ZegaServiceException(string.Format("Same Repcode can't be in Mutiple Batches. Repcodes :{0}", string.Join(',', repcodes)));
            }
            if (profileModel.Id != 0)
            {
                tDVeoSettings = _specificationsImportLogic.GetTDVeoProfileById(profileModel.Id);
                if (tDVeoSettings != null && tDVeoSettings.Profile != null)
                {
                    tDVeoSettings.Profile.Name = profileModel.Name;
                    tDVeoSettings.Profile.BrokerageFirm = BrokerageFirm.TDAmertide;
                    tDVeoSettings.Profile.AutoImport = profileModel.AutoImport;
                    tDVeoSettings.Profile.AutoImportTime = profileModel.SchedulerImportTime;
                }
            }
            else
            {
                if (_specificationsImportLogic.IsTDVeoProfileExist())
                    throw new ZegaServiceException("Profile already exist, we can't create multiple profile for import.");

                tDVeoSettings = _specificationsImportLogic.CreateTDVeoSettingsEntity();
                tDVeoSettings.Profile = new()
                {
                    Name = profileModel.Name,
                    BrokerageFirm = BrokerageFirm.TDAmertide,
                    AutoImport = profileModel.AutoImport,
                    AutoImportTime = profileModel.SchedulerImportTime
                };
            }
            if (tDVeoSettings != null)
            {
                if (profileModel.Password == null)
                    throw new ZegaServiceException("Password Can't be null or Empty!");
                tDVeoSettings.Batches = profileModel.Batches != null ?  JsonConvert.SerializeObject(profileModel.Batches) : JsonConvert.SerializeObject(new List<List<int>>());
                tDVeoSettings.UserId = profileModel.Login;
                tDVeoSettings.Password = profileModel.Password;
                tDVeoSettings.RepCodeIds= profileModel.RepCodes != null ? JsonConvert.SerializeObject(profileModel.RepCodes) : JsonConvert.SerializeObject(new List<int>());   
            }

            _specificationsImportLogic.TDAmertideSettingsPersist(tDVeoSettings);

            if (profileModel.SchedulerImportTime != null)
            {
                var shedulerImportTime = DateConverter.ConvertToIntTime(profileModel.SchedulerImportTime);
                var gs = _globalSettingsLogic.Get();
                if (gs.SchedulerImportHour != shedulerImportTime[0] || gs.SchedulerImportHour != shedulerImportTime[1])
                {
                    gs.SchedulerImportHour = shedulerImportTime[0];
                    gs.SchedulerImportMinute = shedulerImportTime[1];
                    //change Import scheduler time
                    _zegaScheduler.ChangeImportTrigger(gs.SchedulerImportHour, gs.SchedulerImportMinute, profileModel.AutoImport).Wait();

                    _globalSettingsLogic.Persist(gs);
                }
            }
            return tDVeoSettings.Profile?.Id ?? 0;
        }

        public DataGridModel LoadImportHistoryByFilter(DataGridFilterModel dataGridFilterModel, UserContextModel userContext)
        {
            int count;
            CheckUserContext(userContext);
            var importHistory = _specificationsImportLogic.GetImportHistoryByFilter(dataGridFilterModel, out count).Select(x => new ImportHistoryModel
            {
                Id = x.Id,
                Type = x.ImportType,
                Timestamp = x.Timestamp,
                ImportMessage = x.ImportMessage,
                ImportStatus = x.Status
            }).ToArray();

            var dataGridModel = new DataGridModel();
            dataGridModel.ImportHistory = importHistory;
            dataGridModel.TotalRecords = count;

            return dataGridModel;
        }

        public ImportResult ReadFileAndImportAccounts(Stream ms, string fileName, UserContextModel userContext)
        {
           return _specificationsImportLogic.ReadFileAndImportAccounts(ms,fileName);
        }
    }
}
