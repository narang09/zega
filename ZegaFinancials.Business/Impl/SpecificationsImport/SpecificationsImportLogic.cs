using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ZegaFinancials.Nhibernate.Entities.Logging;
using ZegaFinancials.Business.Interfaces.Logging;
using ZegaFinancials.Business.Interfaces.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport;
using ZegaFinancials.Business.Support.SpecificationsImport.ImportData;
using ZegaFinancials.Business.Support.SpecificationsImport.Model;
using ZegaFinancials.Business.Support.TDVeo;
using ZegaFinancials.Nhibernate.Dao.Interface.SpecificationsImport;
using ZegaFinancials.Nhibernate.Dao.Interface.Users;
using ZegaFinancials.Nhibernate.Entities.Import;
using ZegaFinancials.Nhibernate.Entities.Import.ImportSettings;
using ZegaFinancials.Nhibernate.Entities.Shared;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using NHibernate;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using ZegaFinancials.Business.Support.Parser;

namespace ZegaFinancials.Business.Impl.SpecificationsImport
{
    public class SpecificationsImportLogic : ZegaLogic, ISpecificationsImportLogic
    {
        private readonly IImportProfileDao _importProfileDao;
        private readonly ITDVeoImportProvider _TDVeoImportProvider;
        private readonly ITDVeoSettingsDao _TDVeoSettingsDao;
        private readonly ITDVeoXmlImportParser _TDVeoXmlImportParser;
        private readonly IImportPersister _importPersister;
        private readonly IRepCodeDao _repCodeDao;
        private readonly IImportHistoryDao _importHistoryDao;
        private readonly IAuditLogLogic _auditLogLogic;
        private readonly ILogger<SpecificationsImportLogic> _logger;
        private readonly ISession _session;
        private readonly IAdvisorRepCodeDao _advisorRepCodeDao;

        readonly object _mSyncObject = new object();
        bool _importIsBusy;
        public SpecificationsImportLogic(IImportProfileDao importprofileDao, ITDVeoImportProvider TDVeoImportProvider, ITDVeoSettingsDao TDVeoSettingsDao, ITDVeoXmlImportParser TDVeoXmlImportParser, IImportPersister importPersister, IRepCodeDao repCodeDao, IImportHistoryDao importHistoryDao, IAuditLogLogic auditLogLogic, ILogger<SpecificationsImportLogic> logger, IAdvisorRepCodeDao advisorRepCodeDao, ISession session)
        {
            _importProfileDao = importprofileDao;
            _TDVeoImportProvider = TDVeoImportProvider;
            _TDVeoSettingsDao = TDVeoSettingsDao;
            _TDVeoXmlImportParser = TDVeoXmlImportParser;
            _importPersister = importPersister;
            _repCodeDao = repCodeDao;
            _importHistoryDao = importHistoryDao;
            _auditLogLogic = auditLogLogic;
            _logger = logger;
            _advisorRepCodeDao = advisorRepCodeDao;
            _session = session;
        }

        public virtual ImportResult ImportProfile(int profileId)
        {
            if (_importIsBusy)
                return new ImportResult { ImportIsBusy = true, Message = "Auto Import is running" };

            lock (_mSyncObject)
            {
                return ImportProfileById(profileId, false);
            }
        }
        public virtual void  ImportAll()
        {
            ImportResult result = new ImportResult();
            using (ITransaction transaction = _session.BeginTransaction())
            {
                try
                {
                    _session.Clear();
                    var profileId = _importProfileDao.GetProfileId();
                    lock (_mSyncObject)
                    {
                        _importIsBusy = true;
                        result = ImportProfileById(profileId, true);
                        _importIsBusy = false;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message);
                    transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// Imports the profile.
        /// </summary>
        /// <param name=" profileId">The profile id.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private ImportResult ImportProfileById(int profileId, bool isAutoImport)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            var importResult = new ImportResult();
            var profile = _importProfileDao.GetBy(profileId);
            WriteImportProfileInitialStageToAuditLog(profile);
            _logger.LogInformation($"The import profile was started on {DateTime.Now}: {profile.Name}.");
            ImportTDVeoProfile(profile, importResult, isAutoImport);
            WriteProfileFullLog(importResult,profile,isAutoImport);
            return importResult;
        }

        private void ImportTDVeoProfile(ImportProfile profile, ImportResult resultItems ,bool isAutoImport= false)
        {
            var importData = new ImportDataItem<AccountImportData>();
            var tdVeoSettings = _TDVeoSettingsDao.GetByProfileId(profile.Id);
            if (tdVeoSettings != null)
            {
                //Authenticate
                var token = _TDVeoImportProvider.AuthenticateProfile(tdVeoSettings.UserId, tdVeoSettings.Password, resultItems, profile.Name);
                if (token != null)
                {

                    if (isAutoImport)
                    {
                        var batches = JsonConvert.DeserializeObject<List<List<int>>>(tdVeoSettings.Batches);
                        int batchNo = 1;
                        foreach (var repcodeInBatch in batches)
                        {
                            var batchImportResult = new ImportResult();
                            var repcodes = _repCodeDao.GetRepCodesByIds(repcodeInBatch);
                            var repcodesMessegeString = string.Join(",",repcodes);
                            _logger.LogInformation($"The import batch was started on {DateTime.Now}: {batchNo}.");
                            WriteImportBatchInitialStageToAuditLog(batchNo,repcodesMessegeString);
                            var elements = _TDVeoImportProvider.GetAccounts(repcodes, batchImportResult, profile.Name, token, batchNo, batches.Count, isAutoImport);
                            if (elements != null)
                                importData = ImportDataFromTDVeo(elements);
                            ProcessImportData(importData, batchImportResult, profile.Name, batchNo, isAutoImport);
                            WriteBatchFullLog(batchImportResult, batchNo,repcodesMessegeString);
                            _logger.LogInformation($"The import batch  was completed on {DateTime.Now}: {batchNo}.");
                            batchNo++;

                        }

                    }
                    else
                    {
                        var repcodes = _repCodeDao.GetRepCodesByIds(JsonConvert.DeserializeObject<List<int>>(tdVeoSettings.RepCodeIds));
                        var elements = _TDVeoImportProvider.GetAccounts(repcodes, resultItems, profile.Name, token);
                        if (elements != null)
                            importData = ImportDataFromTDVeo(elements);
                        ProcessImportData(importData, resultItems, profile.Name);
                    }

                }
                else
                {
                    ImportHistory importHistory = new ImportHistory();
                    importHistory.ImportType = isAutoImport ? ImportType.Auto : ImportType.Manual;
                    importHistory.ImportMessage = string.Join(',', resultItems.ErrorMsg.Select(x => x));
                    importHistory.Status = ImportStatus.Failed;
                    _importHistoryDao.Persist(importHistory);
                }
                _logger.LogInformation($"The import profile was Completed on {DateTime.Now}: {profile.Name}.");
            }

        }
        void ProcessImportData(ImportDataItem<AccountImportData> importData, ImportResult importResult,string profileName,int batchNo = 0, bool isAutoImport = false)
        {
            var importHistory = new ImportHistory();
            if (importData != null)
            {
                bool isDataPersist;
                var importPersistResult = _importPersister.Persist(importData, out isDataPersist);
                if (importPersistResult != null && isDataPersist)
                {
                    importResult.SuccessfullyCount = importPersistResult.SuccessfullyCount;
                    importResult.FailingSaved = importPersistResult.TotalCount - importPersistResult.SuccessfullyCount;
                    importResult.ErrorMsg = importPersistResult.ErrorMsg;
                }
            }
            if (importResult.ErrorMsg == null || !importResult.ErrorMsg.Any())
            {
                importHistory.ImportType = isAutoImport ? ImportType.Auto : ImportType.Manual;
                importHistory.ImportMessage =isAutoImport ? string.Format("Batch No. {0} imported Successfully. Error Count: {1}, Accounts Imported: {2}.", batchNo, importResult.FailingSaved, importResult.SuccessfullyCount) : string.Format("{0} imported Successfully. Error Count: {1}, Accounts imported: {2}.", profileName, importResult.FailingSaved, importResult.SuccessfullyCount);
                importHistory.Status = ImportStatus.Success;
            }
            else
            {
                importHistory.ImportType = isAutoImport ? ImportType.Auto : ImportType.Manual;
                importHistory.ImportMessage = isAutoImport ? string.Format("Batch No. {0}, {1}", batchNo, string.Join(',', importResult.ErrorMsg.Select(x => x))) : string.Join(',', importResult.ErrorMsg.Select(x => x));
                importHistory.Status = ImportStatus.Failed;
            }
            importHistory.Timestamp = DateTime.Now;

            //persist the Import History
            _importHistoryDao.Persist(importHistory);
        }
        private void WriteImportProfileInitialStageToAuditLog(ImportProfile profile)
        {
            var message = $"The following import profile started on {DateTime.Now} : {profile.Name}.";
            _auditLogLogic.Log(EntityType.Import, message);
        }
        private void WriteImportBatchInitialStageToAuditLog(int batchNo,string repcodes)
        {
            var message = $"The following import Batch  started on {DateTime.Now} : {batchNo} :{repcodes}.";
            _auditLogLogic.Log(EntityType.Import, message);
        }
        public static ImportResult CreateResult(string file, string message)
        {
            var model = new ImportResult
            {
                FileName = file,
                FailingSaved = 1,
                ErrorMsg = new List<string> { message }
            };
            return model;
        }
        
        internal ImportDataItem<AccountImportData> ImportDataFromTDVeo(IEnumerable<XElement> elements)
        {
            var accountMapping = MappingFieldsForImport.GetMapping(SpecificFieldsForImport.GetTDVeoAccount());
            _TDVeoXmlImportParser.SetFields(accountMapping);

            var importData = _TDVeoXmlImportParser.Parse(elements);

            return importData;
        }

        /// <summary>
        /// Writes the import log. 
        /// </summary>
        /// <param name="importResult">The import result.</param>
        /// <remarks></remarks>
        private void WriteProfileFullLog(ImportResult importResult ,ImportProfile profile,bool isAutoImport = false)
        {
            if (importResult == null)
                 return;
            if (isAutoImport)
            {
                var msg = new StringBuilder();
                msg.Append("Import results: ");
                msg.AppendFormat("Auto Import {0} Completed on {1}", profile.Name, DateTime.Now);
                _auditLogLogic.Log(EntityType.Import, msg.ToString());
                if (importResult.ErrorMsg != null && importResult.ErrorMsg.Any())
                    _auditLogLogic.Log(EntityType.Import, string.Format("\"{0}\" import file. {1}", importResult.FileName, string.Join(',', importResult.ErrorMsg)));
       
            }
            else
            {
                var msg = new StringBuilder();
                msg.Append("Import results: ");
                msg.AppendFormat("error count: {0}, successfully: {1}.", importResult.FailingSaved, importResult.SuccessfullyCount);
                _auditLogLogic.Log(EntityType.Import, msg.ToString());
                if (importResult.ErrorMsg != null && importResult.ErrorMsg.Any())
                    _auditLogLogic.Log(EntityType.Import, string.Format("\"{0}\" import file. {1}", importResult.FileName, string.Join(',', importResult.ErrorMsg)));
            }

        }
        private void WriteFileFullLog(ImportResult importResult)
        {
            if (importResult == null)
                return;
                var msg = new StringBuilder();
                msg.Append("File Import results: ");
                msg.AppendFormat("error count: {0}, successfully: {1}.", importResult.FailingSaved, importResult.SuccessfullyCount);
                _auditLogLogic.Log(EntityType.Import, msg.ToString());
                if (importResult.ErrorMsg != null && importResult.ErrorMsg.Any())
                    _auditLogLogic.Log(EntityType.Import, string.Format("\"{0}\" import file Error message. {1}", importResult.FileName, string.Join(',', importResult.ErrorMsg)));

        }
        private void  WriteBatchFullLog(ImportResult importResult ,int batchNo, string repcodes)
        {
            if (importResult == null)
                return;
            var msg = new StringBuilder();
            msg.AppendFormat("Import results Batch No. {0} ({1}),", batchNo,repcodes);
            msg.AppendFormat("error count: {0}, successfully: {1}.", importResult.FailingSaved, importResult.SuccessfullyCount);
            _auditLogLogic.Log(EntityType.Import, msg.ToString());
            if (importResult.ErrorMsg != null && importResult.ErrorMsg.Any())
                _auditLogLogic.Log(EntityType.Import, string.Format("\"{0}\" import file. {1}", importResult.FileName, string.Join(',', importResult.ErrorMsg)));

        }

        public IEnumerable<ImportHistory> GetImportHistoryByFilter(DataGridFilterModel model, out int count)
        {
            return _importHistoryDao.GetByFilter(model, out count);
        }

        public IEnumerable<RepCode> GetAllRepCodesList()
        {
            return _advisorRepCodeDao.GetAllRepCodes();
        }       
        public TDAmertideSettings GetTDVeoProfile()
        {
            return _TDVeoSettingsDao.GetAll().FirstOrDefault() ?? new();
        }
        
        public TDAmertideSettings GetTDVeoProfileById(int profileId)
        {
            return _TDVeoSettingsDao.GetByProfileId(profileId);
        }
        public TDAmertideSettings CreateTDVeoSettingsEntity()
        {
            return _TDVeoSettingsDao.Create();
        }        

        public void TDAmertideSettingsPersist(TDAmertideSettings tDAmertideSettings)
        {
            if (tDAmertideSettings == null)
                throw new ArgumentNullException("TDAmertideSettings");
            
            _TDVeoSettingsDao.Persist(tDAmertideSettings);
        }
        public bool IsTDVeoProfileExist()
        {
            var profiles = _TDVeoSettingsDao.GetAll();
            if (profiles != null && profiles.Any())
                return true;
            else
                return false;
        }
        public ImportResult ReadFileAndImportAccounts(Stream s , string fileName)
        {
            ImportResult resultItem = new ImportResult() { FileName = fileName };
            if (s.Length == 0)
                throw new Exception("Uploaded File Is Empty!");
            var goodrecords = new List<AccountFileRowData>();
            var badRecords = new List<string>(); 
            var ImportData = new ImportDataItem<AccountFileRowData>();
            using (var stramReader = new StreamReader(s))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture) {
                    IgnoreBlankLines = true,
                    DetectColumnCountChanges = true,
                    Quote = '"',
                    MissingFieldFound = null,
                };
                using (var csvReader = new CsvReader(stramReader,csvConfig))
                {
                    _logger.LogInformation("Reading File..");
                    _logger.LogInformation("Parsing Account From File..");
                    csvReader.Read();
                    csvReader.ReadHeader();
                    string[] headerRow = csvReader.HeaderRecord;
                    if (!headerRow.Contains("Account No."))
                        throw new Exception("File Must be Contain Account Numbers.");
                    csvReader.Context.RegisterClassMap<AccountFileRowDataMap>();
                    while(csvReader.Read())
                    {
                        try
                        {
                            var record = csvReader.GetRecord<AccountFileRowData>();
                            goodrecords.Add(record);
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException is CsvParserException)
                            {
                                badRecords.Add(string.Format("Line no. {0}, Error: {1}", csvReader.Context.Parser.RawRow, ex.InnerException.Message));
                            }
                            else
                                throw;
                        }
                    }
                    _logger.LogInformation(string.Format("File Account {0},Parsing Completed..", goodrecords.Count));
                    var importDate = DateTime.Now;
                    ImportData = new ImportDataItem<AccountFileRowData>() { Data = goodrecords, FileDate = importDate , Messages = badRecords };
                    ProcessFileImportData(ImportData, resultItem, goodrecords.Count + badRecords.Count);
                    WriteFileFullLog(resultItem);
                }
            }
            return resultItem;
        }
        private void ProcessFileImportData(ImportDataItem<AccountFileRowData> importData, ImportResult importResult, int totalRows)
        {
            var importHistory = new ImportHistory();
            if (importData != null)
            {
                bool isDataPersist;
                var importPersistResult = _importPersister.Persist(importData, out isDataPersist, totalRows);
                if (importPersistResult != null)
                {
                    importResult.SuccessfullyCount = importPersistResult.SuccessfullyCount;
                    importResult.FailingSaved = importPersistResult.TotalCount - importPersistResult.SuccessfullyCount;
                    importResult.ErrorMsg= importPersistResult.ErrorMsg;
                }
            }
            importHistory.ImportType = ImportType.File;
            importHistory.ImportMessage = string.Format("File Imported Successfully. Error Count: {0}, Accounts Imported: {1}", importResult.FailingSaved, importResult.SuccessfullyCount);
            importHistory.Status = ImportStatus.Success;
            importHistory.Timestamp = DateTime.Now;
            //persist the Import History
            _importHistoryDao.Persist(importHistory);
        }
    }
}
