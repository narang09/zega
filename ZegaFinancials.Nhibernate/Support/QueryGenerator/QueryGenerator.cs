using NHibernate;
using NHibernate.Multi;
using System;
using System.Collections.Generic;
using System.Linq;
using ZegaFinancials.Nhibernate.Entities.Shared;
using ZegaFinancials.Nhibernate.Shared.Enums;
using ZegaFinancials.Nhibernate.Shared.Models;
using ZegaFinancials.Services.Shared.Utils;

namespace ZegaFinancials.Nhibernate.Support.QueryGenerator
{
    public class QueryGenerator
    {
        private readonly DataGridFilterModel _dataGridFilterModel;
        private List<Type> _enumTypes = new List<Type>();
        private Dictionary<string, string> _enumsColumnsToReplace = new Dictionary<string, string>();
        private Dictionary<string, string> _sqlParts = new Dictionary<string, string>();
        private readonly QueryReplace[] _replaces;
        private readonly string _prefix;
        private string _tempListStringSql;
        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public QueryGenerator(string prefix, DataGridFilterModel dataGridFilterModel)
         : this(prefix, dataGridFilterModel, new QueryReplace[0], new Dictionary<string, string>())
        {
        }

        public QueryGenerator(string prefix, DataGridFilterModel dataGridFilterModel, QueryReplace[] replaces)
        : this(prefix, dataGridFilterModel, replaces, new Dictionary<string, string>())
        { }

        public QueryGenerator(string prefix, DataGridFilterModel dataGridFilterModel, QueryReplace[] replaces, Dictionary<string, string> enumsColumnsToReplace)
        {
            _dataGridFilterModel = dataGridFilterModel;
            _replaces = replaces;
            _prefix = prefix;
            _enumsColumnsToReplace = enumsColumnsToReplace;
        }

        public IQueryBatch ApplyBasicMultiFilters<T>(ISession session, string sql, Dictionary<String, object> param = null)
        {
            foreach (var filterCol in _dataGridFilterModel.DataGridColumnObjects.Where(x => x.IsFilterAdded  && x.IsFilteringEnabled))
                GetFilterDescriptionInternal(filterCol, _replaces.FirstOrDefault(o => o.PropertyName == filterCol.DataField));

            if (_sqlParts.Count > 0)
            {
                sql += " AND (";
                sql += string.Join(" AND ", _sqlParts.Select(o => "(" + o.Value + ")"));
                sql += ")";
            }
            _sqlParts.Clear();

            sql += GetSearchSql();

            string countSql = string.Empty;

            if (sql.Replace("\r\n", " ").Trim().StartsWith("FROM"))
                countSql = "SELECT COUNT(*) " + sql.Replace("FETCH", string.Empty);
            else if (sql.StartsWith("\r\n\tSELECT"))
                countSql = "SELECT COUNT(*) " + sql.Substring(sql.IndexOf("\r\n\tFROM")).Replace("FETCH", string.Empty);

            sql += GetSortDescription();

            if (param != null)
                _parameters.Append(param);

            var mq = session.CreateQueryBatch();

            if (_dataGridFilterModel.IsPaginationActive)
            {
                mq.Add<T>(session.CreateQuery(sql)
                  .SetFirstResult(_dataGridFilterModel.StartIndex)
                  .SetMaxResults(_dataGridFilterModel.PaginationSize)
                  .SetParameters(_parameters));

            }
            else
                mq.Add<T>(session.CreateQuery(sql).SetParameters(_parameters));

            mq.Add<long>(session.CreateQuery(countSql).SetParameters(_parameters));

            return mq;
        }
        private string GetSortDescription()
        {
            var sql = string.Empty;
            var sorts = new List<string>();

            if (_dataGridFilterModel.SortDescriptions.Count > 0)
            {
                var enumCols = _dataGridFilterModel.DataGridColumnObjects.Where(o => o.Type == GridColumnTypes.Enum).Select(x => x.DataField).ToList();
                if (_enumTypes == null || _enumTypes.Count <= 0)
                    _enumTypes = EnumFunctions.GetAllEnumTypes();
                foreach (var sorting in _dataGridFilterModel.SortDescriptions.OrderBy(o => o.Priority))
                {
                    var replace = _replaces.FirstOrDefault(o => o.PropertyName == sorting.Field);
                    string field = replace == null ? _prefix + "." + sorting.Field : (!string.IsNullOrEmpty(replace.Field) ? replace.Field : replace.PropertyName);
                    if (enumCols.Contains(sorting.Field))
                    {
                        var enumValues = new Dictionary<string, int>();
                        if (_enumsColumnsToReplace.Any(x => x.Key == sorting.Field))
                            enumValues = EnumFunctions.getEnumMembers(EnumFunctions.GetEnumTypeByName(_enumsColumnsToReplace[sorting.Field], _enumTypes));
                        else
                            enumValues = EnumFunctions.getEnumMembers(EnumFunctions.GetEnumTypeByName(sorting.Field, _enumTypes));
                        string enumtempSql = "";
                        foreach (var ele in enumValues)
                        {
                            enumtempSql = enumtempSql + "WHEN " + ele.Value + " THEN '" + ele.Key + "' ";
                        }
                        sorts.Add("( CASE " + field + " " + enumtempSql + " END )" + (sorting.Direction == ListSortDirection.Ascending ? "ASC" : "DESC"));
                    }
                    else
                        sorts.Add(field + " " + (sorting.Direction == ListSortDirection.Ascending ? "ASC" : "DESC"));
                }
            }

            if (sorts.Count > 0)
                sql += " ORDER BY " + string.Join(",", sorts.ToArray());
            return sql;
        }
        private string GetSearchSql()
        {
            string searchSql = string.Empty;

            if (string.IsNullOrEmpty(_dataGridFilterModel.SearchText))
                return searchSql;
            if (_enumTypes == null || _enumTypes.Count <= 0)
                _enumTypes = EnumFunctions.GetAllEnumTypes();

            string param = "GridSearchVal";
            string value = string.Format("%{0}%", _dataGridFilterModel.SearchText);
            Dictionary<string, string> searchSqlParts = new Dictionary<string, string>();

            foreach (var filterCol in _dataGridFilterModel.DataGridColumnObjects.Where(x => x.Type != GridColumnTypes.None &&
            x.Type != GridColumnTypes.None && x.IsFilteringEnabled && _dataGridFilterModel.VisibleColumns != null && _dataGridFilterModel.VisibleColumns.Any() && _dataGridFilterModel.VisibleColumns.Contains(x.DataField)))
            {
                QueryReplace replace = _replaces.FirstOrDefault(o => o.PropertyName == filterCol.DataField);
                string field = replace == null ? _prefix + "." + filterCol.DataField : replace.Field;

                var likeSql = "";
                if (filterCol.TemplateType == GridTemplateTypes.Decimal || filterCol.TemplateType == GridTemplateTypes.Number || filterCol.TemplateType == GridTemplateTypes.Currency)
                {
                    likeSql = string.Format("STR({0}) LIKE :{1}", field, param);
                }
                else if (filterCol.TemplateType == GridTemplateTypes.Percentage)
                {
                    likeSql = string.Format("STR({0} * 100) LIKE :{1}", field, param);
                }
                else if (filterCol.Type == GridColumnTypes.Enum)
                {
                    var enumValues = new List<int>();
                    if (_enumsColumnsToReplace.Any(x => x.Key == filterCol.DataField))
                        enumValues = EnumFunctions.GetEnumValuesBySearchVal(EnumFunctions.GetEnumTypeByName(_enumsColumnsToReplace[filterCol.DataField], _enumTypes), _dataGridFilterModel.SearchText);
                    else
                        enumValues = EnumFunctions.GetEnumValuesBySearchVal(EnumFunctions.GetEnumTypeByName(filterCol.DataField, _enumTypes), _dataGridFilterModel.SearchText);

                    string enumSearchParam = ReplacePointFromParam(filterCol.DataField + "enumSearchParam");

                    if (enumValues.Count > 0)
                    {

                        likeSql = string.Format("ISNULL({0},'') IN (:{1})", field, enumSearchParam);
                        _parameters.Add(enumSearchParam, enumValues.ToArray());
                    }
                }
                else if (filterCol.Type == GridColumnTypes.DateTime)
                {
                    likeSql = string.Format("CONVERT(varchar, {0}, 101) LIKE :{1}", field, param);
                }
                else if (filterCol.Type == GridColumnTypes.Bool)
                {
                    if (_dataGridFilterModel.SearchText.ToLower().Equals("false"))
                        likeSql = string.Format("{0} = 0", field);
                    else if (_dataGridFilterModel.SearchText.ToLower().Equals("true"))
                        likeSql = string.Format("{0} = 1", field);
                }
                else
                    likeSql = string.Format("{0} LIKE :{1}", field, param);

                if (!string.IsNullOrEmpty(likeSql))
                    searchSqlParts.Add(filterCol.DataField, likeSql);
            }

            if (searchSqlParts.Count > 0)
            {
                searchSql += " AND (";
                searchSql += string.Join(" OR ", searchSqlParts.Select(o => "(" + o.Value + ")").ToArray());
                searchSql += ")";
                _parameters.Add(param, value);

            }
            return searchSql;
        }

        private string ReplacePointFromParam(string param)
        {
            if (!string.IsNullOrEmpty(param))
                return param.Replace(".", "_");
            else
                return param;
        }

        private void GetFilterDescriptionInternal(DataGridColumnObject dataGridCol, QueryReplace replace)
        {           
            switch (dataGridCol.Type)
            {
                case GridColumnTypes.Bool:
                    GetBoolColumnDescription(dataGridCol, replace);
                    break;
                case GridColumnTypes.String:
                    GetStringColumnDescription(dataGridCol, replace);
                    break;
                case GridColumnTypes.Number:
                    GetNumberColumnDescription(dataGridCol, replace);
                    break;
                case GridColumnTypes.List:
                case GridColumnTypes.ListWithNames:
                    GetListColumnDescription(dataGridCol, replace);
                    break;
                case GridColumnTypes.Enum:
                    GetEnumColumnDescription(dataGridCol, replace);
                    break;
                case GridColumnTypes.DateTime:
                    GetDateColumnDescription(dataGridCol, replace);
                    break;
            }
        }

        private void GetBoolColumnDescription(DataGridColumnObject dataGridCol, QueryReplace replace)
        {
            var field = replace == null ? _prefix + "." + dataGridCol.DataField : replace.Field;
            var sql = string.Empty;
            foreach (var expression in dataGridCol.FilterExpressions)
            {
                var param = ReplacePointFromParam(dataGridCol.DataField + expression.ExpressionGuid);
                if (expression.SelectedValues == null || expression.SelectedValues.Count == 0)
                    continue;

                var tempsql = string.Format("ISNULL({0},0) IN (:{1})", field, param);

                if (string.IsNullOrEmpty(sql))
                    sql = sql + "(" + tempsql + ")";
                else
                    sql = sql + " AND (" + tempsql + ")";
                _parameters.Add(param, expression.SelectedValues.Select(x => x == 0 ? false : true).ToArray());
            }
            if (!string.IsNullOrEmpty(sql))
                _sqlParts.Add(dataGridCol.DataField, sql);
        }
        private void GetStringColumnDescription(DataGridColumnObject dataGridCol, QueryReplace replace, bool IsfromList = false, List<FilterExpressionModel> FilterExpression = null)
        {

            var field = replace == null ? _prefix + "." + dataGridCol.DataField : replace.Field;
            var sql = string.Empty;
            foreach (var expression in IsfromList ? FilterExpression : dataGridCol.FilterExpressions)
            {
                var param = ReplacePointFromParam(dataGridCol.DataField + expression.ExpressionGuid);
                var notSign = expression.StringConditionType == StringConditionTypes.DoesNotContain ? "NOT" : "";

                string value;

                if (!expression.Values.ContainsKey("Value1"))
                    throw new NotImplementedException("Value not known");

                if (expression.StringConditionType == StringConditionTypes.Contains || expression.StringConditionType == StringConditionTypes.DoesNotContain)
                    value = string.Format("%{0}%", expression.Values["Value1"]);
                else if (expression.StringConditionType == StringConditionTypes.EndsWith)
                    value = string.Format("%{0}", expression.Values["Value1"]);
                else if (expression.StringConditionType == StringConditionTypes.Equals)
                    value = string.Format("{0}", expression.Values["Value1"]);
                else if (expression.StringConditionType == StringConditionTypes.StartsWith)
                    value = string.Format("{0}%", expression.Values["Value1"]);
                else
                    throw new NotImplementedException("Unknown mode.");

                if (value == "%%")
                    continue;

                var tempsql = string.Format("ISNULL({0},'') {1} LIKE :{2}", field, notSign, param);

                if (string.IsNullOrEmpty(sql))
                    sql = sql + "(" + tempsql + ")";
                else
                    sql = sql + " AND (" + tempsql + ")";
                _parameters.Add(param, value);
            }
           
            if (IsfromList)
                _tempListStringSql = sql;

            if (!string.IsNullOrEmpty(sql) && !IsfromList)
                _sqlParts.Add(dataGridCol.DataField, sql);
        }

        private void GetNumberColumnDescription(DataGridColumnObject dataGridCol, QueryReplace replace)
        {
            var field = replace == null ? _prefix + "." + dataGridCol.DataField : replace.Field;
            var sql = string.Empty;
            foreach (var expression in dataGridCol.FilterExpressions)
            {
                var param1 = ReplacePointFromParam(dataGridCol.DataField + expression.ExpressionGuid + "_From");
                var param2 = ReplacePointFromParam(dataGridCol.DataField + expression.ExpressionGuid + "_To");
                string value = "0", value1 = "0", sign;

                var filterValues = expression.Values;

                if (expression.NumericConditionType == NumericConditionTypes.GreaterThanOrEqual)
                {
                    sign = ">=";
                    value = filterValues["Value2"];
                }
                else if (expression.NumericConditionType == NumericConditionTypes.LessThanOrEqual)
                {
                    sign = "<=";
                    value = filterValues["Value3"];
                }
                else if (expression.NumericConditionType == NumericConditionTypes.Equals || expression.NumericConditionType == NumericConditionTypes.DoesNotEqual)
                {
                    sign = expression.NumericConditionType == NumericConditionTypes.DoesNotEqual ? "!=" : "=";
                    value = filterValues["Value1"];
                }
                else if (expression.NumericConditionType == NumericConditionTypes.RangeBetween)
                {
                    sign = "BETWEEN";
                    value = filterValues["Value4"];
                    value1 = filterValues["Value5"];
                }
                else
                    throw new NotImplementedException("Unknown comparision.");

                if (string.IsNullOrEmpty(value) && (expression.NumericConditionType != NumericConditionTypes.RangeBetween || (expression.NumericConditionType == NumericConditionTypes.RangeBetween && string.IsNullOrEmpty(value1))))
                    continue;

                var tempsql = "";

                if (dataGridCol.TemplateType == GridTemplateTypes.Percentage)
                {
                    if (expression.NumericConditionType == NumericConditionTypes.RangeBetween)
                        tempsql = string.Format("(ISNULL({0},null) * 100) {1} :{2} AND :{3}", field, sign, param1, param2);
                    else
                        tempsql = string.Format("(ISNULL({0},null) * 100) {1} :{2}", field, sign, param1);
                }
                else
                {
                    if (expression.NumericConditionType == NumericConditionTypes.RangeBetween)
                        tempsql = string.Format("ISNULL({0},null) {1} :{2} AND :{3}", field, sign, param1, param2);
                    else
                        tempsql = string.Format("ISNULL({0},null) {1} :{2}", field, sign, param1);
                }

                if (string.IsNullOrEmpty(value))
                    value = "0";

                if (string.IsNullOrEmpty(value1))
                    value1 = "0";
                if (string.IsNullOrEmpty(sql))
                    sql = sql + "(" + tempsql + ")";
                else
                    sql = sql + " AND (" + tempsql + ")";
                _parameters.Add(param1, Convert.ToDecimal(value));
                if (expression.NumericConditionType == NumericConditionTypes.RangeBetween)
                    _parameters.Add(param2, Convert.ToDecimal(value1));
            }
            if (!string.IsNullOrEmpty(sql))
                _sqlParts.Add(dataGridCol.DataField, sql);
        }

        private void GetListColumnDescription(DataGridColumnObject dataGridCol, QueryReplace replace)
        {
            var field = replace == null ? _prefix + "." + dataGridCol.DataField : replace.Field;
            var sql = string.Empty;
            foreach (var expression in dataGridCol.FilterExpressions)
            {
                if (expression.SelectedValues != null && expression.SelectedValues.Count > 0)
                {
                    var tempsql = string.Empty;

                    var param = ReplacePointFromParam(dataGridCol.DataField + expression.ExpressionGuid);
                    var notSign = expression.StringConditionType == StringConditionTypes.DoesNotContain ? "NOT" : "";
                    var oldfield = field;

                    if (replace != null && replace.IsIdUsable)
                        field = field.Substring(0, field.LastIndexOf('.') + 1) + "Id";
                   
                   tempsql = string.Format("ISNULL({0},'') {1} IN (:{2})", field, notSign, param);
                    
                    if (string.IsNullOrEmpty(sql))
                        sql = sql + "(" + tempsql + ")";
                    else
                        sql = sql + " AND (" + tempsql + ")";
                    
                    _parameters.Add(param, expression.SelectedValues.Select(x => x).ToArray());
                }
                else
                {
                    GetStringColumnDescription(dataGridCol, replace, true, new List<FilterExpressionModel>() { expression });
                    if (!string.IsNullOrEmpty(_tempListStringSql))
                    {
                        if (string.IsNullOrEmpty(sql))
                            sql = sql + "(" + _tempListStringSql + ")";
                        else
                            sql = sql + " AND (" + _tempListStringSql + ")";
                    }
                }
            }
            if (!string.IsNullOrEmpty(sql))
                _sqlParts.Add(dataGridCol.DataField, sql);
        }

        private void GetDateColumnDescription(DataGridColumnObject dataGridCol, QueryReplace replace)
        {
            var field = replace == null ? _prefix + "." + dataGridCol.DataField : replace.Field;
            var sql = string.Empty;
            foreach (var expression in dataGridCol.FilterExpressions)
            {
                string param = ReplacePointFromParam(dataGridCol.DataField), param1 = ReplacePointFromParam(dataGridCol.DataField + expression.ExpressionGuid + "_To");
                string value = "", value1 = "", interval, sign;
                var filterValues = expression.Values;

                var tempsql = "";

                if (DateTimeConditionTypes.WithInLast == expression.DateTimeConditionType ||
                   DateTimeConditionTypes.MoreThan == expression.DateTimeConditionType)
                {
                    interval = "DAY";

                    if (DateTimeConditionTypes.WithInLast == expression.DateTimeConditionType)
                    {
                        value = "-" + filterValues["Value1"];
                        sign = "BETWEEN";
                        tempsql = string.Format("ISNULL({0},'') {1} DATEADD({2},:{3},GETDATE()) AND GETDATE()+1", field, sign, interval, param);
                    }
                    else if (DateTimeConditionTypes.MoreThan == expression.DateTimeConditionType)
                    {
                        value = "-" + filterValues["Value2"];
                        sign = "<";
                        tempsql = string.Format("ISNULL({0},'') {1} DATEADD({2},:{3},GETDATE())", field, sign, interval, param);
                    }
                    if (value == "%%" || value == "-")
                        continue;
                    int daysVal;
                    if (!int.TryParse(value, out daysVal))
                        throw new NullReferenceException("Value not known");

                    _parameters.Add(param, daysVal);

                }
                else if (DateTimeConditionTypes.Between == expression.DateTimeConditionType)
                {
                    if (!string.IsNullOrEmpty(expression.DateConfig.startDate))
                        value = DateTime.Parse(expression.DateConfig.startDate).ToString("yyyy/MM/dd HH:mm:ss");

                    if (!string.IsNullOrEmpty(expression.DateConfig.endDate))
                        value1 = DateTime.Parse(expression.DateConfig.endDate).ToString("yyyy/MM/dd HH:mm:ss"); 


                    sign = "BETWEEN";
                    tempsql = string.Format("ISNULL({0},'') {1} :{2} AND :{3}", field, sign, param, param1);

                    DateTime daysVal, daysVal1;
                    if (!DateTime.TryParse(value, out daysVal))
                        throw new NullReferenceException("Date value not known");

                    if (!DateTime.TryParse(value1, out daysVal1))
                        throw new NullReferenceException("Date value not known");

                    _parameters.Add(param, daysVal);
                    _parameters.Add(param1, daysVal1);

                }
                else if (DateTimeConditionTypes.InRange == expression.DateTimeConditionType)
                {
                    interval = "DAY";
                    value = filterValues["Value4"];
                    value1 = filterValues["Value5"];
                    sign = "BETWEEN";
                    tempsql = string.Format("ISNULL({0},'') {1} DATEADD({2},:{3},GETDATE()) AND DATEADD({2},:{4},GETDATE()+1)", field, sign, interval, param, param1);
                    if (value == "%%")
                        continue;
                    int daysVal, daysVal1;
                    if (!int.TryParse(value, out daysVal))
                        throw new NullReferenceException("Value not known");

                    if (!int.TryParse(value1, out daysVal1))
                        throw new NullReferenceException("Value not known");

                    if (daysVal > daysVal1)
                        throw new NotImplementedException("Range is not valid.");

                    _parameters.Add(param, daysVal);
                    _parameters.Add(param1, daysVal1);
                }
                else
                    throw new NotImplementedException("Unknown mode.");

                if (string.IsNullOrEmpty(sql))
                    sql = sql + "(" + tempsql + ")";
                else
                    sql = sql + " AND (" + tempsql + ")";
            }

            if (!string.IsNullOrEmpty(sql))
                _sqlParts.Add(dataGridCol.DataField, sql);
        }

        private void GetEnumColumnDescription(DataGridColumnObject dataGridCol, QueryReplace replace)
        {
            var field = replace == null ? _prefix + "." + dataGridCol.DataField : replace.Field;
            var sql = string.Empty;
            foreach (var expression in dataGridCol.FilterExpressions)
            {
                var param = ReplacePointFromParam(dataGridCol.DataField + expression.ExpressionGuid);
               
                if (expression.SelectedValues == null || expression.SelectedValues.Count == 0)
                    continue;

                var tempsql = string.Format("ISNULL({0},0) IN (:{1})", field, param);

                if (string.IsNullOrEmpty(sql))
                    sql = sql + "(" + tempsql + ")";
                else
                    sql = sql + " AND (" + tempsql + ")";
                _parameters.Add(param, expression.SelectedValues.Select(x => Convert.ToInt32(x)).ToArray());
            }
            if (!string.IsNullOrEmpty(sql))
                _sqlParts.Add(dataGridCol.DataField, sql);
        }
    }
}
