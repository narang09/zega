import { Component, OnInit } from '@angular/core';
import { DataGridNames } from '../../support/enums/data-grid.enum';

@Component({
  selector: 'zega-import-history',
  templateUrl: './import-history.component.html',
  styleUrls: ['./import-history.component.less']
})
export class ImportHistoryComponent implements OnInit {

  constructor() { }

  refreshGrid: boolean = false;
  gridName: DataGridNames = DataGridNames.ImportHistory;

  ngOnInit(): void {
  }

  refreshDataGrid() {
    this.refreshGrid = !this.refreshGrid;
  }

}
