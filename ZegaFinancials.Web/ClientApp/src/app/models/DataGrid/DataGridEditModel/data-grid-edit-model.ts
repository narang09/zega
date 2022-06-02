import { DataGridNames } from "../../../support/enums/data-grid.enum";

export interface DataGridEditModel {
  Grid: DataGridNames;
  EditRowKey: any;
  UpdatedValues: any;
}
