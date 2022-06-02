function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}
export class ApiURLs {
  static BaseURL: string = getBaseUrl();

  static loginAPI: string = ApiURLs.BaseURL + 'Login/GetLoginResponse';
  static forgotPasswordAPI: string = ApiURLs.BaseURL + 'Login/ForgotPassword';
  static resetPasswordAPI: string = ApiURLs.BaseURL + 'Login/ResetPassword';
  static logoutAPI: string = ApiURLs.BaseURL + 'Login/LogoutUser';

  static getGridHeaderAPI: string = ApiURLs.BaseURL + 'DataGrid/GetGridHeaders';
  static getDataGridDataAPI: string = ApiURLs.BaseURL + 'DataGrid/GetDataGridData';
  static saveGridPreferencesAPI: string = ApiURLs.BaseURL + 'DataGrid/SaveGridPreferences';
  static exportPaginatedGridToExcelAPI: string = ApiURLs.BaseURL + 'DataGrid/ExportGridToCsv';

  static deleteUsersAPI: string = ApiURLs.BaseURL + 'UserManagement/DeleteUsers';
  static saveUserAPI: string = ApiURLs.BaseURL + 'UserManagement/SaveUserInformation';
  static getUserAPI: string = ApiURLs.BaseURL + 'UserManagement/GetUserById';
  static uploadImageAPI: string = ApiURLs.BaseURL + 'UserManagement/UploadUserProfileImage';
  static getImageAPI: string = ApiURLs.BaseURL + 'UserManagement/GetUserProfileImage';
  static removeUserProfileImageAPI: string = ApiURLs.BaseURL + 'UserManagement/RemoveUserProfileImage';
  static userSettingsAPI: string = ApiURLs.BaseURL + 'UserManagement/SaveSettingsInformation';
  static getSettingsInformationAPI: string = ApiURLs.BaseURL + 'UserManagement/GetSettingsInformation';
  static passwordSettingsAPI: string = ApiURLs.BaseURL + 'UserManagement/SaveSettingsInformation'; // Duplicate
  static deleteRepCodeAPI: string = ApiURLs.BaseURL + 'UserManagement/DeleteRepCodes';
  static saveRepCodeAPI: string = ApiURLs.BaseURL + 'UserManagement/SaveRepCode';

  static getAccountAPI: string = ApiURLs.BaseURL + 'Account/GetAccountInformation';
  static addAccountAPI: string = ApiURLs.BaseURL + 'Account/SaveAccountInformation';
  static saveBasicDetailsAPI: string = ApiURLs.BaseURL + 'Account/SaveAccountBasicDetails';
  static saveModelDetailsAPI: string = ApiURLs.BaseURL + 'Account/SaveAccountModelDetails';
  static saveAddWithdrawlAPI: string = ApiURLs.BaseURL + 'Account/SaveAccountAdditionalWithdrawals';
  static saveAddDepositAPI: string = ApiURLs.BaseURL + 'Account/SaveAccountAdditionalDeposits';
  static saveZegaCustomAPI: string = ApiURLs.BaseURL + 'Account/SaveAccountZegaCustomFields';
  static addAccountDropdownAPI: string = ApiURLs.BaseURL + 'Account/GetAccountDropdowns';
  static getAdvisorsByRepCodeAPI: string = ApiURLs.BaseURL + 'Account/GetAdvisor';
  static deleteAccountAPI: string = ApiURLs.BaseURL + 'Account/DeleteAccounts';

  static deleteStrategiesAPI: string = ApiURLs.BaseURL + 'Strategy/DeleteStrategies';
  static saveStrategyAPI: string = ApiURLs.BaseURL + 'Strategy/SaveStrategyInformation';
  static getStrategyAPI: string = ApiURLs.BaseURL + 'Strategy/GetStrategyById';

  static deleteSleevesAPI: string = ApiURLs.BaseURL + 'Sleeves/DeleteSleeves';
  static saveSleeveAPI: string = ApiURLs.BaseURL + 'Sleeves/SaveSleeveInformation';
  static getSleeveAPI: string = ApiURLs.BaseURL + 'Sleeves/GetSleeveInformation';

  static deleteModelsAPI: string = ApiURLs.BaseURL + 'Model/DeleteModels';
  static getModelAPI: string = ApiURLs.BaseURL + 'Model/GetModelDetails';
  static saveModelAPI: string = ApiURLs.BaseURL + 'Model/SaveModelDetails';
  static getModelDropdownsAPI: string = ApiURLs.BaseURL + 'Model/GetDropdownsForModelDetails';

  static getBulkEditDropdownsAPI: string = ApiURLs.BaseURL + 'Sidebar/GetBulkEditDropdowns';
  static saveBulkEditInformationAPI: string = ApiURLs.BaseURL + 'Sidebar/SaveBulkEditInformation';

  static saveImportProfileAPI: string = ApiURLs.BaseURL + 'ImportProfiles/SaveImportProfile';
  static getImportDataAPI: string = ApiURLs.BaseURL + 'ImportProfiles/GetProfile';
  static getRepCodeDropdownDataAPI: string = ApiURLs.BaseURL + 'ImportProfiles/LoadRepCodeDropDown';
  static runImportAPI: string = ApiURLs.BaseURL + 'ImportProfiles/Import';
  static uploadImportFileAPI: string = ApiURLs.BaseURL + 'ImportProfiles/UploadImportFile';

  static getVersionInfoAPI: string = ApiURLs.BaseURL + 'Zega/InitialDetails';

}
