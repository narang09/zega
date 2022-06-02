import { DataGridNames } from "../../../support/enums/data-grid.enum";

export class GridRowSelectionModel {
  constructor() {
    this.Grid = DataGridNames.None;
    this.Key = '';
    this.SelectedRows = [];
  }

  Grid: DataGridNames;
  SelectedRows: Array<any>;
  Key: string;

  get SelectedKeys(): Array<any> {
      return this.Key ? [...this.SelectedRows.map(item => item[this.Key])] : [];
  }

}
