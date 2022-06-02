import { Data } from "@angular/router";
import { DateTimeConditionTypes, NumericConditionTypes, StringConditionTypes } from "../../../support/enums/filter.enum";
import { DataGridColumnModel } from "../DataGridColumnModel/data-grid-column-model";

export class FilterExpressionModel {

  constructor(obj: any) {
    this.ExpressionGuid = obj?.expressionGuid ?? '';
    this.SelectedValues = obj?.selectedValues ?? [];
    this.StringConditionType = obj?.stringConditionType ?? StringConditionTypes.Contains;
    this.NumericConditionType = obj?.numericConditionType ?? NumericConditionTypes.Equals;
    this.DateTimeConditionType = obj?.dateTimeConditionType ?? DateTimeConditionTypes.WithInLast;
    this.Values = obj?.values ?? {};
    this.DateConfig = obj?.dateConfig ?? {};
  }

  ExpressionGuid: string
  SelectedValues: Array<number>;
  StringConditionType: StringConditionTypes;
  NumericConditionType: NumericConditionTypes;
  DateTimeConditionType: DateTimeConditionTypes;
  Values: FilterExpressionValues;
  DateConfig: DateConfigModel
}

interface FilterExpressionValues {
  Value1: string;
  Value2: string;
  Value3: string;
  Value4: string;
  Value5: string;
}

interface DateConfigModel {
  StartDate: Date;
  EndDate: Data;
}

export interface AdvancedFilterCommunicationModel {
  GridColumns: Array<DataGridColumnModel>
  ApplyFilter: boolean
}
