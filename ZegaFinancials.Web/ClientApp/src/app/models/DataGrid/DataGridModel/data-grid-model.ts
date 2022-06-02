import { DataGridNames, GridPaginationSize } from "../../../support/enums/data-grid.enum";
import { DataGridColumnModel } from "../DataGridColumnModel/data-grid-column-model";

export class DataGridModel {
  constructor() {
    this.ColumnDef = [];
    this.Grid = DataGridNames.None;
    this.PaginationSize = 0;
    this.Page = 1;
    this.TotalCount = 0;
    this.SelectedCount = 0;
    this.QuickSearch = '';
  }

  ColumnDef: DataGridColumnModel[];
  Grid: DataGridNames;
  PaginationSize: number;
  Page: number;
  TotalCount: number;
  SelectedCount: number;
  QuickSearch: string;

  get GridName() {
    return DataGridNames[this.Grid];
  }
}
