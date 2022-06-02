import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DataGridColumnModel, DropdownListModel } from '../../models/DataGrid/DataGridColumnModel/data-grid-column-model';
import { AdvancedFilterCommunicationModel, FilterExpressionModel } from '../../models/DataGrid/FilterExpressionModel/filter-expression-model';
import { AuthMessageService } from '../../services/AuthMessageService/auth-message.service';
import { DataGridColumnType } from '../../support/enums/data-grid.enum';
import { DateTimeConditionTypes, NumericConditionTypes, StringConditionTypes } from '../../support/enums/filter.enum';
import { Utility } from '../../support/utility/utility';

@Component({
  selector: 'zega-advanced-filter',
  templateUrl: './advanced-filter.component.html',
  styleUrls: ['./advanced-filter.component.less']
})
export class AdvancedFilterComponent implements OnInit {

  constructor(private dialogRef: MatDialogRef<AdvancedFilterComponent>, @Inject(MAT_DIALOG_DATA) private dialogData: AdvancedFilterCommunicationModel, private messageService: AuthMessageService) {
    dialogRef.disableClose = true;
  }

  GridColumns: Array<DataGridColumnModel> = [];
  SelectedColumn?: DataGridColumnModel = undefined;
  SelectedFilterExpression: FilterExpressionModel = new FilterExpressionModel({});

  // For Dropdowns
  GridColumnTypes = DataGridColumnType;
  StringConditionTypes = StringConditionTypes;
  NumericConditionTypes = NumericConditionTypes;
  DateTimeConditionTypes = DateTimeConditionTypes;

  ngOnInit(): void {
    if (this.dialogData.GridColumns && this.dialogData.GridColumns.length)
      this.GridColumns = this.dialogData.GridColumns;
  }

  get GridColumnsWithFilters() {
    return this.GridColumns.filter(col => col.IsFilterAdded);
  }

  resetFilterExpressionValues(value: any) {
    this.SelectedFilterExpression.Values = { Value1: '', Value2: '', Value3: '', Value4: '', Value5: '' };
    //this.SelectedFilterExpression.DateConfig = { StartDate: new Date(), EndDate: new Date() }; // To discuss with Backend Team 
  }

  addFilterExpression(checkValidation: boolean) {
    let isValid = checkValidation ? this.validateFilterExpression() : true;
    if (isValid) {
      let gridCol = this.GridColumns.find(col => col.DataField === this.SelectedColumn?.DataField);
      if (gridCol) {
        if (!this.SelectedFilterExpression.ExpressionGuid)
          this.SelectedFilterExpression.ExpressionGuid = Utility.getRandomIdentifier().toString();
        if ((gridCol.Type === this.GridColumnTypes.Enum || gridCol.Type === this.GridColumnTypes.Bool) && this.SelectedFilterExpression.SelectedValues.length)
          this.resetFilterExpressionValues(null);
        gridCol.IsFilterAdded = true;
        gridCol.FilterExpressions.push(this.SelectedFilterExpression);
        this.SelectedColumn = undefined;
        this.SelectedFilterExpression = new FilterExpressionModel({});
      }
    } else
      this.messageService.showErrorPopup({ message: "InValid Filter Expression" });
  }

  removeFilterExpression(column: DataGridColumnModel, expression: FilterExpressionModel) {
    var ind = column.FilterExpressions.findIndex(exp => exp === expression);
    if (ind > -1)
      column.FilterExpressions.splice(ind, 1);
    column.IsFilterAdded = column.FilterExpressions.length ? true : false;
  }

  getExpressionDescription(column: DataGridColumnModel, expression: FilterExpressionModel) {
    let descriptionParts: Array<string> = [];
    descriptionParts.push(column.DisplayName);
    switch (column.Type) {
      case DataGridColumnType.String:
        descriptionParts.push(StringConditionTypes[expression.StringConditionType], expression.Values.Value1);
        break;
      case DataGridColumnType.Number:
        descriptionParts.push(NumericConditionTypes[expression.NumericConditionType]);
        switch (expression.NumericConditionType) {
          case NumericConditionTypes.Equals:
          case NumericConditionTypes.DoesNotEqual:
            descriptionParts.push(expression.Values.Value1);
            break;
          case NumericConditionTypes.GreaterThanOrEqual:
            descriptionParts.push(expression.Values.Value2);
            break;
          case NumericConditionTypes.LessThanOrEqual:
            descriptionParts.push(expression.Values.Value3);
            break;
          case NumericConditionTypes.RangeBetween:
            descriptionParts.push(expression.Values.Value4, ' TO ', expression.Values.Value5);
            break;
        }
        break;
      case DataGridColumnType.DateTime:
        descriptionParts.push(DateTimeConditionTypes[expression.DateTimeConditionType]);
        switch (expression.DateTimeConditionType) {
          case DateTimeConditionTypes.WithInLast:
            descriptionParts.push(expression.Values.Value1, ' Days');
            break;
          case DateTimeConditionTypes.MoreThan:
            descriptionParts.push(expression.Values.Value2, ' Days');
            break;
          case DateTimeConditionTypes.Between:
            descriptionParts.push(expression.DateConfig.StartDate.toLocaleDateString(), ' TO ', expression.DateConfig.EndDate.toLocaleDateString());
            break;
          case DateTimeConditionTypes.InRange:
            descriptionParts.push(expression.Values.Value4, ' TO ', expression.Values.Value5, ' Days');
            break;
        }
        break;
      case DataGridColumnType.List:
        if (expression.SelectedValues.length)
          descriptionParts.push(' IN ( ', this.getDescriptionForSelectedItems(column.DropdownList, expression.SelectedValues), ' )');
        else
          descriptionParts.push(StringConditionTypes[expression.StringConditionType], expression.Values.Value1);
        break;
      case DataGridColumnType.Enum:
        descriptionParts.push(' IN ( ', this.getDescriptionForSelectedItems(column.DropdownList, expression.SelectedValues), ' )');
        break;
      case DataGridColumnType.Bool:
        descriptionParts.push(' IS ', this.getDescriptionForSelectedItems(column.DropdownList, expression.SelectedValues));
        break;

    }
    return descriptionParts.join(' ');
  }


  private getDescriptionForSelectedItems(dropdownList: any, selectedItems: Array<number>) {
    let selectedVKeys = Object.keys(dropdownList).filter(li => selectedItems.includes(dropdownList[li])).map(si => si);
    return selectedVKeys.join(', ');
  }


  resetFilter() {
    this.GridColumns.filter(col => col.IsFilterAdded).forEach(col => { col.FilterExpressions = []; col.IsFilterAdded = false });
    let retData: AdvancedFilterCommunicationModel = {
      GridColumns: this.GridColumns,
      ApplyFilter: true
    }
    this.dialogRef.close(retData);
  }

  cancelAdvFilter() {
    this.dialogRef.close(null);
  }

  applyAdvFilter() {
    if (this.SelectedFilterExpression && this.validateFilterExpression())
      this.addFilterExpression(false);
    let gridCols = this.GridColumns;
    if (gridCols.filter(g => g.IsFilterAdded).length) {
      let retData: AdvancedFilterCommunicationModel = {
        GridColumns: gridCols,
        ApplyFilter: true
      }
      this.dialogRef.close(retData);
    }
    else
      this.messageService.showErrorPopup({ message: "Please add a filter expression" });
  }

  private isNullOrEmpty(value: any) {
    return value === '' || value === null || value === undefined;
  }

  private validateFilterExpression() {
    let isInvalid: boolean = true;
    if (this.SelectedColumn && this.SelectedColumn.IsFilteringEnabled) {
      switch (this.SelectedColumn.Type) {
        case DataGridColumnType.String: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value1); break;
        case DataGridColumnType.Number:
          switch (this.SelectedFilterExpression.NumericConditionType) {
            case NumericConditionTypes.Equals:
            case NumericConditionTypes.DoesNotEqual: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value1); break;
            case NumericConditionTypes.GreaterThanOrEqual: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value2); break;
            case NumericConditionTypes.LessThanOrEqual: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value3); break;
            case NumericConditionTypes.RangeBetween: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value4) && this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value5); break;
          }
          break;
        case DataGridColumnType.DateTime:
          switch (this.SelectedFilterExpression.DateTimeConditionType) {
            case DateTimeConditionTypes.WithInLast: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value1); break;
            case DateTimeConditionTypes.MoreThan: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value2); break;
            case DateTimeConditionTypes.Between: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.DateConfig.StartDate) && this.isNullOrEmpty(this.SelectedFilterExpression.DateConfig.EndDate); break;
            case DateTimeConditionTypes.InRange: isInvalid = this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value4) && this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value5); break;
          }
          break;
        case DataGridColumnType.List:
          isInvalid = this.SelectedFilterExpression.SelectedValues.length == 0 || this.isNullOrEmpty(this.SelectedFilterExpression.Values.Value1);
          break;
        case DataGridColumnType.Enum:
        case DataGridColumnType.Bool:
          isInvalid = this.SelectedFilterExpression.SelectedValues.length == 0;
          break;
      }
    }
    return !isInvalid;
  }




}
