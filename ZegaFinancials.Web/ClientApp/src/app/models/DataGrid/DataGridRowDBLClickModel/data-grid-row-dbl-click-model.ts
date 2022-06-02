import { DataGridNames } from "../../../support/enums/data-grid.enum";

export interface DataGridRowDblClickModel {
  Grid: DataGridNames;
  Key: any;
  RowData: any;
}
