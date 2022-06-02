import { DataGridColumnType, DataGridCssType, DataGridFormatterType, DataGridTemplateType } from "../../../support/enums/data-grid.enum";
import { Utility } from "../../../support/utility/utility";
import { FilterExpressionModel } from "../FilterExpressionModel/filter-expression-model";

export class DataGridColumnModel {
  constructor(gridCol: any) {
    this.DisplayName = gridCol.displayName ?? '';
    this.DataField = gridCol.dataField ?? '';
    this.Type = gridCol.type ?? DataGridColumnType.None;
    this.TemplateType = gridCol.templateType ?? DataGridTemplateType.None;
    this.CSSType = gridCol.cSSType ?? DataGridCssType.None;
    this.MinWidth = gridCol.minWidth ?? 0;
    this.IsVisible = gridCol.isVisible ?? false;
    this.IsEditable = gridCol.isEditable ?? false;
    this.IsLocked = gridCol.isLocked ?? false;
    this.IsSortingEnabled = gridCol.isSortingEnabled ?? false;
    this.IsFilteringEnabled = gridCol.isFilteringEnabled ?? false;
    this.DataFormatter = gridCol.dataFormatter ?? DataGridFormatterType.None;
    this.IsManualColumn = gridCol.isManualColumn ?? false;
    this.Validators = gridCol.validators ?? [];
    this.DisplayIndex = gridCol.displayIndex ?? 0;
    this.DropdownList = gridCol.enumValues ?? {};
    this.DataValidators = gridCol.isEditable && gridCol.dataValidators.length ? gridCol.dataValidators : [];
    this.UIGridValidators = this.getUIGridValidators();
    this.SortIndex = 0;
    this.SortOrder = '';
    this.FilterExpressions = [];
    this.IsFilterAdded = false;
  }

  DisplayName: string;
  DataField: string;
  Type: DataGridColumnType;
  TemplateType: DataGridTemplateType;
  CSSType: DataGridCssType;
  MinWidth: number;
  IsVisible: boolean;
  IsEditable: boolean;
  IsLocked: boolean;
  IsSortingEnabled: boolean;
  IsFilteringEnabled: boolean;
  DataFormatter: DataGridFormatterType;
  IsManualColumn: boolean;
  Validators: string[];
  DisplayIndex: number;
  SortIndex: number;
  SortOrder: string;
  FilterExpressions: Array<FilterExpressionModel>
  DropdownList: object;
  IsFilterAdded: boolean;
  DataValidators: Array<string>;
  UIGridValidators: Array<any>;


  get DataFieldUI() {
    return this.DataField ? this.covertColumnNameToCamelCase(this.DataField) : '';
  }

  private covertColumnNameToCamelCase(colName: string) {
    return colName.indexOf('.') > -1 ? colName.split('.').map(colPart => Utility.convertToCamelCase(colPart)).join('.') : Utility.convertToCamelCase(colName);
  }

  private getUIGridValidators() {
    let UIValidators: Array<any> = [];
    this.DataValidators.forEach(dv => {
      if (dv === 'required')
        UIValidators.push({
          type: 'required',
          message: 'This value is Required!'
        });
      if (dv === 'percentage')
        UIValidators.push({
          type: 'range',
          min: 0,
          max: 100,
          message: 'This value should be between 0 & 100!'
        });
    })
    return UIValidators;
  }

  get DisplayType() {
    // 'object' 
    let dxColType = '';
    switch (this.TemplateType) {
      case DataGridTemplateType.String:
        dxColType = 'string';
        break;
      case DataGridTemplateType.Number:
      case DataGridTemplateType.Decimal:
      case DataGridTemplateType.Percentage:
      case DataGridTemplateType.Currency:
        dxColType = 'number';
        break;
      case DataGridTemplateType.Date:
        dxColType = 'date';
        break;
      case DataGridTemplateType.DateTime:
        dxColType = 'datetime';
        break;
      case DataGridTemplateType.Checkbox:
        dxColType = 'boolean';
        break;
      default:
        dxColType = '';
    }
    return dxColType;
  }

  get CSSClass() {
    let dxCSSClass = '';
    switch (this.CSSType) {
      case DataGridCssType.None:
        dxCSSClass = '';
        break;
    }
    return dxCSSClass;
  }


  get UICellTemplete() {
    let customCellTemplete = null;
    switch (this.TemplateType) {
      case DataGridTemplateType.UserActive: customCellTemplete = 'userActiveCellTemplate'; break;
      case DataGridTemplateType.Favourite: customCellTemplete = 'favouriteCellTemplate'; break;
    }
    return customCellTemplete;
  }

  get UIFormatter() {
    let formallter: any = '';
    switch (this.TemplateType) {
      case DataGridTemplateType.Currency: formallter = 'currency'; break;
      case DataGridTemplateType.Percentage: formallter = 'percent'; break;
      case DataGridTemplateType.Decimal: formallter = 'decimal'; break;
    }
    return formallter;
  }

}

export interface DropdownListModel {
  Key: string,
  Value: number
}
