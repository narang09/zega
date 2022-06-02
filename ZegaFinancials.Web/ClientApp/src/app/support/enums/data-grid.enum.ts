export enum DataGridNames {
  None = 0,
  AccountListing = 1,
  ModelListing = 2,
  AdvisorListing = 3,
  SleeveListing = 4,
  StrategyListing = 5,
  AuditLogListing = 6,
  RepCodeListing = 7,
  UserContactNumbers = 8,
  UserEmails = 9,
  ModelListingSidebar = 11,
  DashboardAdmin = 12,
  DashboardAdvisor = 13,
  ImportHistory = 14,
  ModelListingSubGrid = 15,
}

export enum DataGridColumnType {
  None = 0,
  String = 1,
  Number = 2,
  Enum = 3,
  DateTime = 4,
  List = 5,
  Bool = 6
}

export enum DataGridTemplateType {
  None = 0,
  String = 1,
  Number = 2,
  Decimal = 3,
  Percentage = 4,
  Currency = 5,
  Date = 6,
  DateTime = 7,
  Checkbox = 8,
  IpAddress = 9,
  UserActive = 10,
  Favourite = 11
}

export enum DataGridCssType {
  None = 0
}

export enum DataGridFormatterType {
  None = 0,
}

export enum GridPaginationSize {
  S = 20,
  M = 50,
  L = 100,
  XL = 200,
}

export enum ColumnSortOrder {
  ASC = 0,
  DESC = 1
}
