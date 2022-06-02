using System.Collections.Generic;
using System.IO;
using ZegaFinancials.Business.Support.SpecificationsImport.Model;
using ZegaFinancials.Nhibernate.Entities.Import;
using ZegaFinancials.Nhibernate.Entities.Import.ImportSettings;
using ZegaFinancials.Nhibernate.Entities.Shared;

namespace ZegaFinancials.Business.Interfaces.SpecificationsImport
{
    public interface ISpecificationsImportLogic
    {
		/// <summary>
		/// Imports the profile.
		/// </summary>
		/// <param name="profileId">The profile id.</param>
		/// <returns></returns>
		/// <remarks></remarks>
		ImportResult ImportProfile(int profileId);

		/// <summary>
		/// Imports profile which AutoImport property is 'true'.
		/// </summary>
		/// <remarks></remarks>
		void ImportAll();

		/// <summary>
		/// Get All RepCodes List
		/// </summary>
		/// <returns></returns>
		IEnumerable<RepCode> GetAllRepCodesList();
		
		/// <summary>
		/// Get the TDVeo Profile
		/// </summary>
		/// <returns></returns>
		TDAmertideSettings GetTDVeoProfile();

		/// <summary>
		/// Get the TDVeo Profile by Id.
		/// </summary>
		/// <param name="profileId"> Profile Id</param>
		/// <returns></returns>
		TDAmertideSettings GetTDVeoProfileById(int profileId);

		/// <summary>
		/// Create TDVeo Settings Entity
		/// </summary>
		/// <returns></returns>
		TDAmertideSettings CreateTDVeoSettingsEntity();

		/// <summary>
		/// Persiste the TDVeo Settings
		/// </summary>
		/// <param name="tDAmertideSettings">TDVeo Settings</param>
		void TDAmertideSettingsPersist(TDAmertideSettings tDAmertideSettings);

		/// <summary> 
		/// Get ImportHistory By DataGrid Filter model
		/// </summary>
		/// <param name="model">Filter model</param>
		/// <param name="count">Total records count</param>
		/// <returns></returns>
		IEnumerable<ImportHistory> GetImportHistoryByFilter(DataGridFilterModel model, out int count);
		bool IsTDVeoProfileExist();
		ImportResult ReadFileAndImportAccounts(Stream s, string fileName);
	}
}
