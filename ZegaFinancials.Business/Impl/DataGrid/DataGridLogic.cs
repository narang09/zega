using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ZegaFinancials.Business.Interfaces.DataGrid;
using ZegaFinancials.Business.Shared.Models;
using ZegaFinancials.Nhibernate.Dao.Interface.DataGrid;
using ZegaFinancials.Nhibernate.Entities.Config;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Support;

namespace ZegaFinancials.Business.Impl.DataGrid
{
    public class DataGridLogic : ZegaLogic, IDataGridLogic
    {
        public class DataGridExportColumn
        {
            public string Header { get; set; }
            public string ColumnName { get; set; }
            public bool Visible { get; set; }
            public int DisplayIndex { get; set; }
        }
        private const string SEPARATOR = ",";
        private const string END_OF_LINE = "\r\n";
        private const string QUOTES = "\"";
        private readonly IDataGridDao _dataGridDao;
        public DataGridLogic(IDataGridDao dataGridDao)
        {
            _dataGridDao = dataGridDao;
        }

        public byte[] ExportToCsv(IEnumerable<object> entityList, IEnumerable<DataGridColumnObject> columns, HashSet<string> unsupportedList, GetCustomFieldValue handler)
        {
            var sortedColumns = columns.Where(c => c.IsVisible && !unsupportedList.Contains(c.DataField) && !c.DataField.Equals("Select")).ToArray();

            byte[] rawData = null;
            using var outputMemStream = new MemoryStream();
            var stringWriter = new StringBuilder();
            {
                ExportHeaders(stringWriter, sortedColumns);
                foreach (var entity in entityList)
                    ExportItem(stringWriter, sortedColumns, entity, handler);
                rawData = Encoding.UTF8.GetBytes(stringWriter.ToString());
            }
            return rawData;
        }

        private static void ExportItem(StringBuilder stringWriter, DataGridColumnObject[] sortedColumns, object entity, GetCustomFieldValue handler)
        {
            bool futureWithrawal = true, futureDeposit = true;
            for (var columnIndex = 0; columnIndex < sortedColumns.Length; columnIndex++)
            {                
                var column = sortedColumns[columnIndex];
                if (column.DataField == "accounWithdrawlInfoModelcs.Future_Withdrawal_StatusValue" || column.DataField == "accountDepositsInfoModelcs.Deposit_StatusValue")
                {
                    var propValue = GetPropertyValue(entity, column.DataField);
                    if (object.Equals(propValue, "No"))
                    {
                        if (column.DataField == "accounWithdrawlInfoModelcs.Future_Withdrawal_StatusValue")
                            futureWithrawal = false;
                        else
                            futureDeposit = false;
                    }

                }
                object value;
                if ((column.DataField == "accounWithdrawlInfoModelcs.Withdrawal_FrequencyValue" || column.DataField == "accounWithdrawlInfoModelcs.Withdrawl_StatusValue") && !futureWithrawal)
                    value = null;
                else if (column.DataField == "accountDepositsInfoModelcs.Deposit_FrequencyValue" && !futureDeposit)
                    value = null;
                else
                    value = !column.DataField.StartsWith("Custom_") ? GetPropertyValue(entity, column.DataField) : handler(entity, column.DataField.Replace(" ", string.Empty));

                if (value != null)
                    stringWriter.Append(QUOTES + value.ToString() + QUOTES);
                if (columnIndex < (sortedColumns.Length - 1))
                    stringWriter.Append(SEPARATOR);
            }
            stringWriter.Append(END_OF_LINE);
        }
        public static object GetPropertyValue(object obj, string propertyCombinedPath)
        {
            propertyCombinedPath = propertyCombinedPath.Replace(',', '.');
            var propertyPath = propertyCombinedPath.Split('.');
            var value = obj;
            for (var i = 0; i < propertyPath.Length; i++)
            {
                if (value == null)
                    break;

                var propertyName = propertyPath[i];
                var pi = GetPropertyOrBaseProperty(value.GetType(), propertyName);
                if (pi != null)
                {
                    value = pi.GetValue(value, null);
                }
                else
                {
                    throw new ArgumentException(string.Format("Property path '{0}' is invalid", propertyCombinedPath));
                }
            }
            return value;
        }
        public static PropertyInfo GetPropertyOrBaseProperty(Type type, string propertyName)
        {
            PropertyInfo foundProperty = null;

            if (!string.IsNullOrEmpty(propertyName))
            {
                while (type != null && foundProperty == null)
                {
                    try
                    {
                        foundProperty = type.GetProperty(
                            propertyName,
                            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    }
                    catch (AmbiguousMatchException)
                    {
                        //no common property name can rise this exception here because BindingFlags.DeclaredOnly flag used.
                        //Therefore overloaded indexers found. Use the indexer with the one integer parameter if found
                        var properties = type.GetProperties(
                            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

                        foreach (var property in properties)
                        {
                            var indexParams = property.GetIndexParameters();
                            if (indexParams.Length == 1 && indexParams[0].ParameterType == typeof(int))
                            {
                                foundProperty = property;
                                break;
                            }
                        }
                    }

                    if (foundProperty == null)
                        type = type.BaseType;
                }
            }
            return foundProperty;
        }
        private static void ExportHeaders(StringBuilder stringWriter, DataGridColumnObject[] sortedColumns)
        {
            for (var columnIndex = 0; columnIndex < sortedColumns.Length; columnIndex++)
            {
                var column = sortedColumns[columnIndex];
                if (column.DisplayName != null)
                    stringWriter.Append(column.DisplayName);
                if (columnIndex < (sortedColumns.Length - 1))
                    stringWriter.Append(SEPARATOR);
            }
            stringWriter.Append(END_OF_LINE);
        }

        public delegate object GetCustomFieldValue(object p, string propName);

        public GridConfig GetGridHeaders(DataRequestSource grid, int userId)
        {
            var gridConfigs = _dataGridDao.GetGridConfig(grid, userId);
            GridConfig gridConfig;
            if (gridConfigs.Count() > 1 && gridConfigs.Any(x => !x.IsDefault))
                gridConfig = gridConfigs.Where(x => !x.IsDefault).FirstOrDefault();
            else
                gridConfig = gridConfigs.Where(x => x.IsDefault).FirstOrDefault();

            return gridConfig;
        }

        public DataGridPreferenceModel GetGridByPreferences(DataRequestSource grid, int userId)
        {
            var gridConfigs = _dataGridDao.GetGridConfig(grid, userId);
            var userGridConfig = gridConfigs.Where(x => !x.IsDefault).FirstOrDefault();
            var defaultGridConfig = gridConfigs.Where(x => x.IsDefault).FirstOrDefault();

            DataGridPreferenceModel headerInfo;
            var defaultColumnconfig = JsonConvert.DeserializeObject<List<DataGridColumnObject>>(defaultGridConfig.GridColumnJSonValue);
            var defaultSortingConfig = !string.IsNullOrEmpty(defaultGridConfig.SortingJSonValue) ? JsonConvert.DeserializeObject<List<SortDescription>>(defaultGridConfig.SortingJSonValue) : null;

            if(gridConfigs.Count() > 1 && userGridConfig != null)
            {
                var userColumnConfig = JsonConvert.DeserializeObject<List<DataGridColumnObject>>(userGridConfig.GridColumnJSonValue);
                var userSortingConfig = !string.IsNullOrEmpty(userGridConfig.SortingJSonValue) ? JsonConvert.DeserializeObject<List<SortDescription>>(userGridConfig.SortingJSonValue) : null;

                foreach (var defaultGridcol in defaultColumnconfig)
                {
                    var userGridCol = userColumnConfig.Where(x => x.DataField == defaultGridcol.DataField).FirstOrDefault();
                    if (userGridCol != null)
                    {
                        if (defaultGridcol.DisplayIndex != userGridCol.DisplayIndex)
                            defaultGridcol.DisplayIndex = userGridCol.DisplayIndex;
                        if (defaultGridcol.IsVisible != userGridCol.IsVisible)
                            defaultGridcol.IsVisible = userGridCol.IsVisible;
                    }
                }

                if (defaultSortingConfig != null)
                {
                    foreach (var defaultGridSort in defaultSortingConfig)
                    {
                        var userGridSort = userSortingConfig?.Where(x => x.Field == defaultGridSort.Field)?.FirstOrDefault();
                        if (userGridSort != null)
                        {
                            if (defaultGridSort.FieldDirection != userGridSort.FieldDirection)
                                defaultGridSort.FieldDirection = userGridSort.FieldDirection;
                            if (defaultGridSort.Priority != userGridSort.Priority)
                                defaultGridSort.Priority = userGridSort.Priority;
                        }
                    }
                }

                if ((defaultSortingConfig == null || !defaultSortingConfig.Any()) && userSortingConfig != null && userSortingConfig.Any())
                    defaultSortingConfig = userSortingConfig;
            }

            headerInfo = new DataGridPreferenceModel()
            {
                DataGridColumnObjects = defaultColumnconfig,
                SortDescriptions = defaultSortingConfig
            };

            return headerInfo;
        }

        public void Persist(GridConfig headerInfo)
        {
            if (headerInfo == null)
                throw new ArgumentNullException("headerInfo");
            _dataGridDao.Persist(headerInfo);
        }
    }
}
